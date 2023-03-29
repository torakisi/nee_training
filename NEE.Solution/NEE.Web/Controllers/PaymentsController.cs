using NEE.Core.Contracts.Enumerations;
using NEE.Service;
using NEE.Web.Models.Payments;
using NEE.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static NEE.Service.PaymentService;
using NEE.Core.BO;
using NEE.Web.Code;

namespace NEE.Web.Controllers
{
    [Authorize]
    public class PaymentsController : NEEBaseController
    {
        private AppService _gsAppService;
        private PaymentService _gsPaymentService;

        public PaymentsController(PaymentService gsPaymentService, AppService gsAppService, UserService gsUserService)
            : base(gsUserService)
        {
            _gsAppService = gsAppService;
            _gsPaymentService = gsPaymentService;
        }

        // GET: Payments
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> Index(string id)
        {
            PaymentsViewModel model = new PaymentsViewModel();

            GetPaymentsDataRequest req = new GetPaymentsDataRequest()
            {
                AFM = (this.UserInfo == null) ? null : this.UserInfo.AFM
            };

            if (!string.IsNullOrEmpty(id))
            {
                req.Id = id;
                model.CanGoBackToApplication = true;
            }

            if (UserInfo != null)
            {
                await PerformPaymentSearch(model, req);
            }
            else
            {
                if (!string.IsNullOrEmpty(id))
                {
                    await PerformPaymentSearch(model, req);
                }
            }

            if (UserInfo == null)
            {
                model.CanPerformActions = true;
            }

            return View(model);
        }

        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(PaymentsViewModel model)
        {
            GetPaymentsDataRequest req = new GetPaymentsDataRequest()
            {
                AFM = model.AFM,
                Id = model.Id
            };

            await PerformPaymentSearch(model, req);

            return View("Index", model);
        }

        private async Task PerformPaymentSearch(PaymentsViewModel model, GetPaymentsDataRequest req)
        {
            var resp = await _gsPaymentService.GetPaymentsDataAsync(req);

            model.CanAddCredit = resp.CanAddCredit;
            model.CanAddDebit = resp.CanAddDebit;

            if (!resp._IsSuccessful)
            {
                foreach (var error in resp.UIDisplayedErrors)
                {
                    ModelState.AddModelError("", error.Message);
                }
            }

            foreach (PaymentTransactions paymentTransaction in resp.PaymentTransactions)
            {
                model.PaymentTransactions.Add(paymentTransaction.Map());
            }

            foreach (KeyValuePair<string, List<PaymentTransactions>> groupItem in resp.PaymentTransactionsById)
            {
                model.PaymentTransactionsById
                        .Add(groupItem.Key, groupItem.Value.MapList());
            }

            foreach (PaymentsWebView paymentTransactionViewModel in resp.PaymentsWebView)
            {
                model.PaymentsWebView.Add(paymentTransactionViewModel.Map());
            }

            model.Id = resp.Id;
            model.AFM = resp.AFM;

            if (UserInfo == null)
            {
                model.CanPerformActions = true;
            }
        }

    }
}
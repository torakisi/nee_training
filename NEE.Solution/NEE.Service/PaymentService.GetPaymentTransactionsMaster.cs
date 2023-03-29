using NEE.Core.BO;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Service
{
    public partial class PaymentService
    {
        private async Task<GetPaymentTransactionsMasterResponse> GetPaymentTransactionsMasterAsync(GetPaymentTransactionsMasterRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.GetPaymentTransactionsMaster,
                InitialRequest = JsonHelper.Serialize(req, false)
            };

            this.SetServiceContext(context);

            GetPaymentTransactionsMasterResponse response = new GetPaymentTransactionsMasterResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                List<PaymentsWebView> paymentTransactionsMasterView = await repository.GetPaymentTransactionsMasterView(req.AFM, req.Id);

                response.PaymentsWebView = paymentTransactionsMasterView;
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return response;
        }


        public class GetPaymentTransactionsMasterRequest
        {
            public string Id { get; set; }
            public string AFM { get; set; }
            public bool CheckInput { get; set; } = true;
        }
        public class GetPaymentTransactionsMasterResponse : NEEServiceResponseBase
        {
            public GetPaymentTransactionsMasterResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
            {
            }

            public static GetPaymentTransactionsMasterResponse InvalidArgumentsCall()
            {
                var res = new GetPaymentTransactionsMasterResponse { _IsSuccessful = false };
                res.AddError(ErrorCategory.UIDisplayed, "Δεν είναι δυνατή η επεξεργασία του αιτήματος");
                return res;
            }

            public string Id { get; set; }
            public string AFM { get; set; }
            public List<PaymentsWebView> PaymentsWebView { get; set; } = new List<PaymentsWebView>();
        }
    }
}

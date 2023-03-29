using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Service.Authorization;
using NEE.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEE.Core.BO;

namespace NEE.Service
{
    public partial class PaymentService
    {
        public async Task<GetPaymentsDataResponse> GetPaymentsDataAsync(GetPaymentsDataRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.GetPaymentsData,
                InitialRequest = JsonHelper.Serialize(req, false)
            };

            this.SetServiceContext(context);

            GetPaymentsDataResponse response = new GetPaymentsDataResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                if (this._currentUserContext.IsKKUser)
                {
                    if (this._currentUserContext.IsInRole("DebitManagers"))
                    {
                        response.CanAddDebit = true;
                    }

                    if (this._currentUserContext.IsInRole("CreditManagers"))
                    {
                        response.CanAddCredit = true;
                    }
                }

                string afmToUse = req.AFM;

                if (string.IsNullOrEmpty(req.Id) && string.IsNullOrEmpty(req.AFM))
                {
                    return GetPaymentsDataResponse.InvalidArgumentsCall();
                }

                if (!string.IsNullOrEmpty(req.Id))
                {
                    var getApp = await repository.Load(req.Id);

                    if (getApp == null)
                        return GetPaymentsDataResponse.InvalidArgumentsCall();

                    if (!this._currentUserContext.IsKKUser)
                    {
                        if (!ServiceAuthorization.AccessApplicationShouldBeApplicantAuthorized(_currentUserContext, getApp))
                            throw new UnauthorizedAccessException(ServiceAuthorization.NOT_AUTHORISED_MESSAGE);
                    }

                    if (!string.IsNullOrEmpty(req.AFM))
                    {
                        if (getApp.AFM != req.AFM)
                            return GetPaymentsDataResponse.InvalidArgumentsCall();
                    }
                    else
                    {
                        afmToUse = getApp.AFM;
                    }
                }

                GetPaymentTransactionsRequest reqTransactions = new GetPaymentTransactionsRequest()
                {
                    AFM = afmToUse,
                    Id = req.Id,
                };

                var respTransactions = await this.GetPaymentTransactionsAsync(reqTransactions);

                if (!respTransactions._IsSuccessful)
                {
                    response.AddErrors(respTransactions._Errors);
                    return response;
                }

                response.PaymentTransactions = respTransactions.PaymentTransactions;
                response.PaymentTransactionsById = respTransactions.PaymentTransactionsById;

                GetPaymentTransactionsMasterRequest reqTransactionMaster = new GetPaymentTransactionsMasterRequest()
                {
                    AFM = afmToUse,
                    Id = req.Id
                };

                var respTransactionMaster = await this.GetPaymentTransactionsMasterAsync(reqTransactionMaster);

                if (!respTransactionMaster._IsSuccessful)
                {
                    response.AddErrors(respTransactionMaster._Errors);
                    return response;
                }

                response.PaymentsWebView = respTransactionMaster.PaymentsWebView;

                response.AFM = afmToUse;
                response.Id = req.Id;
            }
            catch (UnauthorizedAccessException ex)
            {
                response.AddError(ErrorCategory.UIDisplayed, null, ex);
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return response;
        }


        public class GetPaymentsDataRequest
        {
            public string Id { get; set; }
            public string AFM { get; set; }
        }
        public class GetPaymentsDataResponse : NEEServiceResponseBase
        {
            public GetPaymentsDataResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
            {
            }

            public static GetPaymentsDataResponse InvalidArgumentsCall()
            {
                var res = new GetPaymentsDataResponse { _IsSuccessful = false };
                res.AddError(ErrorCategory.UIDisplayed, "Δεν είναι δυνατή η επεξεργασία του αιτήματος");
                return res;
            }

            public string Id { get; set; }
            public string AFM { get; set; }
            public List<PaymentTransactions> PaymentTransactions { get; set; } = new List<PaymentTransactions>();
            public Dictionary<string, List<PaymentTransactions>> PaymentTransactionsById { get; set; } = new Dictionary<string, List<PaymentTransactions>>();

            public List<PaymentsWebView> PaymentsWebView { get; set; } = new List<PaymentsWebView>();

            public bool CanAddDebit { get; set; } = false;
            public bool CanAddCredit { get; set; } = false;
        }
    }
}

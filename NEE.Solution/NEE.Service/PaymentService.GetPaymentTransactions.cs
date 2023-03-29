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
        private async Task<GetPaymentTransactionsResponse> GetPaymentTransactionsAsync(GetPaymentTransactionsRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.GetPaymentTransactions,
                InitialRequest = JsonHelper.Serialize(req, false)
            };

            this.SetServiceContext(context);

            GetPaymentTransactionsResponse response = new GetPaymentTransactionsResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                List<PaymentTransactions> paymentTransactions = await repository.GetPaymentTransactions(req.AFM, req.Id);

                response.PaymentTransactions = paymentTransactions;

                var lookupPaymentTransactionsPerAppId = paymentTransactions.ToLookup(p => p.Id, p => p.TransactionId);
                foreach (var paymentTransaction in paymentTransactions)
                {
                    if (string.IsNullOrEmpty(paymentTransaction.Id))
                    {
                        paymentTransaction.Id = "Χωρίς σύνδεση με αίτηση";
                    }

                    if (!response.PaymentTransactionsById.ContainsKey(paymentTransaction.Id))
                    {
                        response.PaymentTransactionsById
                            .Add(paymentTransaction.Id, new List<PaymentTransactions>() { paymentTransaction });
                    }
                    else
                    {
                        response.PaymentTransactionsById[paymentTransaction.Id].Add(paymentTransaction);
                    }
                }
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return response;
        }


        public class GetPaymentTransactionsRequest
        {
            public string Id { get; set; }
            public string AFM { get; set; }
            public bool CheckInput { get; set; } = true;
        }
        public class GetPaymentTransactionsResponse : NEEServiceResponseBase
        {
            public GetPaymentTransactionsResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
            {
            }

            public static GetPaymentTransactionsResponse InvalidArgumentsCall()
            {
                var res = new GetPaymentTransactionsResponse { _IsSuccessful = false };
                res.AddError(ErrorCategory.UIDisplayed, "Δεν είναι δυνατή η επεξεργασία του αιτήματος");
                return res;
            }

            public bool DefaultAFMUsed { get; set; } = false;

            public string Id { get; set; }
            public string AFM { get; set; }
            public List<PaymentTransactions> PaymentTransactions { get; set; } = new List<PaymentTransactions>();
            public Dictionary<string, List<PaymentTransactions>> PaymentTransactionsById { get; set; } = new Dictionary<string, List<PaymentTransactions>>();
        }
    }
}

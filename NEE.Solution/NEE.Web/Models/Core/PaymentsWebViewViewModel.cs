using NEE.Core.Contracts.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NEE.Web.Models.Core
{
    public class PaymentsWebViewViewModel
    {
        public string MasterTransactionId { get; set; }
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string IBAN { get; set; }
        public string AFM { get; set; }
        public string AMKA { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Reason { get; set; }
        public bool? Confirmed { get; set; }
        public bool? Processed { get; set; }
        public string ProcessedInPayment { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string ProcessResult { get; set; }
        public int? State { get; set; }
        public string Description { get; set; }

        public List<PaymentTransactionsViewModel> PaymentTransactions = new List<PaymentTransactionsViewModel>();

        public PaymentResult PaymentResult
        {
            get
            {
                if (Processed.HasValue && Processed.Value && !Confirmed.HasValue)
                {
                    return PaymentResult.SendToDIAS;
                }
                else if (Processed.HasValue && Processed.Value && Confirmed.HasValue && !Confirmed.Value)
                {
                    return PaymentResult.PaymentFailed;
                }
                else if (Processed.HasValue && Processed.Value && Confirmed.HasValue && Confirmed.Value)
                {
                    return PaymentResult.PaymentSucceded;
                }
                else if (!Processed.HasValue && !string.IsNullOrEmpty(ProcessedInPayment))
                {
                    return PaymentResult.PaymentNotIncluded;
                }

                return PaymentResult.Unknown;
            }
        }
    }
}
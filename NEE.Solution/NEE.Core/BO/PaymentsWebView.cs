using System;
using System.Collections.Generic;

namespace NEE.Core.BO
{
    public class PaymentsWebView
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

        public List<PaymentTransactions> PaymentTransactions = new List<PaymentTransactions>();
    }
}

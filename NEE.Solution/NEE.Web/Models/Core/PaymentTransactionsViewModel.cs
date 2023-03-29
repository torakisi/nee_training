using NEE.Core.Contracts.Enumerations;
using System;

namespace NEE.Web.Models.Core
{
    public class PaymentTransactionsViewModel
    {
        public string TransactionId { get; set; }
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string IBAN { get; set; }
        public string AFM { get; set; }
        public string AMKA { get; set; }
        public DateTime ReferenceMonth { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public PaymentTransactionType Type { get; set; }
        public string Reason { get; set; }
        public bool Confirmed { get; set; }
        public bool Processed { get; set; }
        public string ProcessedInPayment { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
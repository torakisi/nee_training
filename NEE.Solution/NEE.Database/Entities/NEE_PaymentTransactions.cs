using NEE.Core.Contracts.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NEE.Database.Entities
{
    public class NEE_PaymentTransactions
    {
        [Key, Column(Order = 0)]
        [Required]
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

        public List<NEE_PaymentsWebView> PaymentsWebViews { get; set; } = new List<NEE_PaymentsWebView>();
    }
}

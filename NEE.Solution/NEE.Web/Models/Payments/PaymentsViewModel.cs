using NEE.Core.Contracts.Enumerations;
using NEE.Web.Models.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NEE.Web.Models.Payments
{
    public class PaymentsViewModel
    {
        public enum PaymentsViewModelValidationType
        {
            InsertPistosiXreosi = 1,
            Search = 2
        }

        [Display(Name = "Κωδικός Αίτησης")]
        public string Id { get; set; }

        [Display(Name = "ΑΦΜ")]
        public string AFM { get; set; }

        public bool CanPerformActions { get; set; } = false;
        public bool CanGoBackToApplication { get; set; } = false;

        public bool CanAddDebit { get; set; } = false;
        public bool CanAddCredit { get; set; } = false;

        public List<PaymentTransactionsViewModel> PaymentTransactions { get; set; } = new List<PaymentTransactionsViewModel>();
        public Dictionary<string, List<PaymentTransactionsViewModel>> PaymentTransactionsById { get; set; } = new Dictionary<string, List<PaymentTransactionsViewModel>>();

        public List<PaymentsWebViewViewModel> PaymentsWebView { get; set; } = new List<PaymentsWebViewViewModel>();

        public InsertPistosiXreosi InsertPistosiXreosiFormData { get; set; } = new InsertPistosiXreosi();

        public bool ValidateModel(ModelStateDictionary modelState, PaymentsViewModelValidationType type)
        {
            if (type == PaymentsViewModelValidationType.InsertPistosiXreosi)
            {
                if (InsertPistosiXreosiFormData == null)
                {
                    modelState.AddModelError("", "Δεν ήταν δυνατή η επεξεργασία του αιτήματος.");
                    return false;
                }

                if (string.IsNullOrEmpty(InsertPistosiXreosiFormData.Reason))
                {
                    modelState.AddModelError("InsertPistosiXreosiFormData.Reason", "Πρέπει να εισάγετε 'Αιτιολογία'");
                    return false;
                }
                if (InsertPistosiXreosiFormData.PaymentTransactionType == PaymentTransactionType.Xreosi)
                {
                    if (!InsertPistosiXreosiFormData.Amount.HasValue || InsertPistosiXreosiFormData.Amount.Value <= 0)
                    {
                        modelState.AddModelError("InsertPistosiXreosiFormData.Amount", "Πρέπει να εισάγετε 'Ποσό' μεγαλύτερο του μηδενός.");
                        return false;
                    }
                }
                if (InsertPistosiXreosiFormData.PaymentTransactionType == PaymentTransactionType.Pistosi)
                {
                    if (!InsertPistosiXreosiFormData.Amount.HasValue || InsertPistosiXreosiFormData.Amount.Value < 0)
                    {
                        modelState.AddModelError("InsertPistosiXreosiFormData.Amount", "Πρέπει να εισάγετε 'Ποσό' μεγαλύτερο ή ίσο του μηδενός.");
                        return false;
                    }
                }

                if (InsertPistosiXreosiFormData.AFM != AFM)
                {
                    modelState.AddModelError("", "Δεν ήταν δυνατή η επεξεργασία του αιτήματος.");
                    return false;
                }

                if (InsertPistosiXreosiFormData.Id != Id)
                {
                    modelState.AddModelError("", "Δεν ήταν δυνατή η επεξεργασία του αιτήματος.");
                    return false;
                }
            }

            return true;
        }

    }

    public class InsertPistosiXreosi
    {
        [Display(Name = "Κωδικός Αίτησης")]
        public string Id { get; set; }

        [Display(Name = "ΑΦΜ")]
        public string AFM { get; set; }

        [Display(Name = "Ποσό")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Required]
        public decimal? Amount { get; set; }

        [Display(Name = "Αιτιολογία")]
        [Required]
        public string Reason { get; set; }

        public bool ServiceResultSuccess { get; set; } = false;
        public PaymentTransactionType PaymentTransactionType { get; set; }
    }
}
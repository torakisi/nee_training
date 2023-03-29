using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Contracts.Enumerations
{
    public enum PaymentTransactionType
    {
        [Display(Name = "Πίστωση")]
        Pistosi = 1,

        [Display(Name = "Χρέωση")]
        Xreosi = 2,

    }
}

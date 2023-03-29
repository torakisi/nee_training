using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Contracts.Enumerations
{
    public enum PaymentResult
    {
        [Display(Name = "Αποστολή σε ΔΙΑΣ")]
        SendToDIAS = 1,

        [Display(Name = "Η πληρωμή απέτυχε")]
        PaymentFailed = 2,

        [Display(Name = "Η πληρωμή πραγματοποιήθηκε")]
        PaymentSucceded = 3,

        [Display(Name = "Δεν συμπεριλαμβάνεται στην πληρωμή")]
        PaymentNotIncluded = 4,

        [Display(Name = "")]
        Unknown = 0,

    }
}

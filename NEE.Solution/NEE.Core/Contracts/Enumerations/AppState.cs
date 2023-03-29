using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Contracts.Enumerations
{
    public enum AppState
    {
        [Display(Name = "Νέα")]
        Draft = 0,

        [Display(Name = "Ακυρωμένη")]
        Canceled = 1,

        [Display(Name = "Υποβληθείσα")]
        Submitted = 3,

        [Display(Name = "Απορριφθείσα")]
        Rejected = 4,

        [Display(Name = "Εγκεκριμένη")]
        Approved = 5,

        [Display(Name = "Αναμονή ελέγχου εγγράφων")]
        PendingDocumentsApproval = 6,

        [Display(Name = "Εκκρεμότητα εγγράφων")]
        RejectedDocuments = 7,

        [Display(Name = "Σε Αναστολή")]
        Suspended = 10,

        [Display(Name = "Σε Ανάκληση")]
        Recalled = 11,

        // Να προστεθεί "Οριστικοποιημένη" (για το σενάριο υποβολής από τα κέντρα κοινότητας)
    }
}

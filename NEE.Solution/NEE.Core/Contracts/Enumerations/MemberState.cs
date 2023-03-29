using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Contracts.Enumerations
{
    public enum MemberState
    {

        // Normal, include this member
        [Display(Name = "Κανονικό")]
        Normal = 0,

        // Deleted (logical, in case of System added member), exclude this member
        [Display(Name = "Διεγραμμένο")]
        Deleted = 1,

        /// <summary>
        /// Στην περίπτωση που μέλος περιθάλπετε σε κλειστή μονάδα τρέχουμε τις διασταυρώσεις για τα υπόλοιπα μέλη του νοικοκυριού
        /// </summary>
        [Display(Name = "Εξαιρείται")]
        Excluded = 2,

        [Display(Name = "Απεβίωσε")]
        Dead = 3,

        [Display(Name = "Εντοπίστηκε αλλά δεν συμμετέχει")]
        NotAvailable = 4,

        [Display(Name = "Απαιτείται Συναίνεση")]
        NeedConcent = 5,

        [Display(Name = "Μη Πιστοποιημένη Σχέση")]
        RelationshipNotValidated = 6,
        /// <summary>
        /// Κατάσταση για την περίπτωση του αλλοδαπού
        /// </summary>
        [Display(Name = "Ελλιπή Στοιχεία")]
        FoundButNotConfirmed = 7,
        /// <summary>
        /// Κατάσταση για την περίπτωση του Ειδικού δικαιώματος
        /// </summary>
        [Display(Name = "Χωρίς δικαίωμα συμμετοχής")]
        NoRightToParticipate = 8,
    }
}

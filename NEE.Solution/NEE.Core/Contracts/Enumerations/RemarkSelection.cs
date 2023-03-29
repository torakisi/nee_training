using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Contracts.Enumerations
{
    public enum RemarkSelection
    {
        [Display(Name = "Μη Αποδοχή")]
        Reject = 0,

        [Display(Name = "Αποδοχή")]
        Approve = 1,

        [Display(Name = "Ειδική Αποδοχή")]
        Approve2 = 2

    }
}

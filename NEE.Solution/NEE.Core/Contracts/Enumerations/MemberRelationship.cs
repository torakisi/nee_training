using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Contracts.Enumerations
{
    public enum MemberRelationship
    {
        [Display(Name = "Αιτών/Αιτούσα")]
        Applicant = 0,

        [Display(Name = "Σύζυγος")]
        Spouse = 1
    }
}

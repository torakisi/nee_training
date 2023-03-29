using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Contracts.Enumerations
{
    public enum DeathStatus
    {
        [Display(Name = "άλλο status")]
        OtherStatus = 1,

        [Display(Name = "Θ: έχει αναγγελθεί θάνατος")]
        AnnouncedDead = 2,

        [Display(Name = "Τ: έχει αναγγελθεί θάνατος από φορέα")]
        AnnouncedDeadByInstitution = 2,

        [Display(Name = "Λ: έχει αναγγελθεί θάνατος με Ληξιαρχική πράξη")]
        RegisteredDead = 4,

    }
}
using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Contracts.Enumerations
{
    public enum OpekaDistricts
    {
        [Display(Name = "Κεντρική υπηρεσία - Αθήνα")]
        Athens = 0,

        [Display(Name = "Κεντρικής Μακεδονίας")]
        KentrikiMakedonia = 1,

        [Display(Name = "Δυτικής Μακεδονίας")]
        DitikiMakedonia = 3,

        [Display(Name = "Ανατολικής Μακεδονίας Θράκης")]
        AnatolikiMakedonia = 4,

        [Display(Name = "Δυτικής Ελλάδας")]
        DitikiEllada = 5,

        [Display(Name = "Κρήτης")]
        Crete = 6,

        [Display(Name = "Θεσσαλίας")]
        Thessalia = 7,

        [Display(Name = "Στερεάς Ελλάδας")]
        StereaEllada = 10,

        [Display(Name = "Ηπείρου")]
        Hpeiros = 11,

        [Display(Name = "Πελοποννήσου")]
        Peloponnisos = 12,
    }
}

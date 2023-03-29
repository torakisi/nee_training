using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Contracts.Enumerations
{
	public enum MaritalStatus
	{
        [Display(Name = "Άγνωστο")]
        Unknown = -1,

        [Display(Name = "Άγαμος/η")]
		Single = 1,

		[Display(Name = "Έγγαμος/η")]
		Married = 2,

		[Display(Name = "Διαζευγμένος/η")]
		Divorsed = 3,

		[Display(Name = "Χήρος/α")]
		Widoed = 4,

		[Display(Name = "Σύμφωνο Συμβίωσης")]
		CivilUnion = 5,

        [Display(Name = "Λύση Σύμφωνου Συμβίωσης")]
        CivilUnionBreak = 6,

        [Display(Name = "Χήρος/α ΣΣ")]
        CivilUnionWidoed = 7
    }
}

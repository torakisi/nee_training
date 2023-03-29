using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Contracts.Enumerations
{
	public enum IdentificationNumberType
	{
		[Display(Name = "Αριθμός Δελτίου Ταυτότητας")]
		ADT = 1,

		[Display(Name = "Διαβατήριο")]
		Passport = 2
	}
}

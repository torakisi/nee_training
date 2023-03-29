using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Contracts.Enumerations
{
	public enum Gender
	{
		[Display(Name = "Άντρας")]
		Male = 1,

		[Display(Name = "Γυναίκα")]
		Female = 2,

	}
}

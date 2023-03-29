using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.Contracts.Enumerations
{
	public enum IbanValidationServiceResult
	{
		Valid = 0,
		IncorrectCombination = 1,
		UnsupportedBank = 2,
		ServiceFailed = 3,
	}
}

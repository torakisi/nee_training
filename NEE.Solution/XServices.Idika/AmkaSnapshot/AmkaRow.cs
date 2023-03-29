using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;

namespace XService.Idika
{
	public class AmkaRow
	{
		public string AMKA { get; set; }
		public string AFM { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string FatherName { get; set; }
		public string MotherName { get; set; }
		public string LastNameEN { get; set; }
		public string FirstNameEN { get; set; }
		public string Gender { get; set; }
		public Gender GetGender() => Gender == "M" ? NEE.Core.Contracts.Enumerations.Gender.Male : NEE.Core.Contracts.Enumerations.Gender.Female;
		public DateTime DOB { get; set; }
		public DateTime? DOD { get; set; }
		public string BirthCountry { get; set; }
        public string CitizenCountry { get; set; }
        public DateTime ModifiedAt { get; set; }
	}
}

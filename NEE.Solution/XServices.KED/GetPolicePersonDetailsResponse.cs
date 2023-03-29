using NEE.Core.Contracts;
using System;

namespace XServices.Edto
{
    public class GetPolicePersonDetailsResponse : XServiceResponseBase
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string BirthDate { get; set; }
        public string PermitNumber { get; set; }
        public DateTime? AdministrationDate { get; set; }
    }
}

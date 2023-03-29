using NEE.Core.Contracts;
using System;

namespace XServices.Edto
{
    public class GetPolicePersonDetailsRequest : XServiceRequestBase
    {
        public string ApplicationId { get; set; }
        public string Afm { get; set; }
        public string Amka { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string BirthDate { get; set; }
    }
}

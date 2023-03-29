using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.CitizenRegistry
{
    public class GetMaritalStatusRequest : XServiceRequestBase
    {
        public string ApplicationId { get; set; }
        public string Afm { get; set; }
        public string Amka { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string BirthDate { get; set; }
    }
}

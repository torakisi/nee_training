using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.Ergani
{
    public class GetEmploymentDetailsRequest: XServiceRequestBase
    {
        public string ApplicationId { get; set; }
        public string Afm { get; set; }
        public string Amka { get; set; }
        public string RefDate { get; set; }
    }
}

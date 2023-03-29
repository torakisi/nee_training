using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.Efka
{
    public class PensionsOpekaRequest : XServiceRequestBase
    {
        public string ApplicationId { get; set; }
        public string Afm { get; set; }
        public string Amka { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}

using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.Gsis
{
    public class GetPropertyValueE9Request : XServiceRequestBase
    {
        public string ApplicationId { get; set; }
        public string Afm { get; set; }
        public string Amka { get; set; }
        public int ReferenceYear { get; set; }
    }
}

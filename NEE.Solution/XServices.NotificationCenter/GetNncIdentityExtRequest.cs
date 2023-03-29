using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.NotificationCenter
{
    public class GetNncIdentityExtRequest : XServiceRequestBase
    {
        public string Afm { get; set; }
        public string Amka { get; set; }
        public string ApplicationId { get; set; }
    }
}

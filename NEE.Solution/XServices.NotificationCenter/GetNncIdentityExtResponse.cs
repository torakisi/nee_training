using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.NotificationCenter
{
    public class GetNncIdentityExtResponse : XServiceResponseBase
    {
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }
        public string AddressStreet { get; set; }
        public string AddressNumber { get; set; }
        //Πόλη/Χωριό (locality)
        public string AddressCity { get; set; }
        public string AddressZip { get; set; }
        public string AddressPostalNumber { get; set; }
        //Περιφέρεια
        public string Region { get; set; }
        //Περιφέρεια Ενότητα
        public string RegionalUnit { get; set; }
        //Δήμος
        public string Municipality { get; set; }
        //Δημοτική Ενότητα
        public string MunicipalUnit { get; set; }
        //Δημοτική Κοινότητα
        public string Commune { get; set; }
    }
}

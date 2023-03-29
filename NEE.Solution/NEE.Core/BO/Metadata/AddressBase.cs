using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.BO.Metadata
{
    public class AddressBase
    {
        public string Country { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string District { get; set; }
    }
}

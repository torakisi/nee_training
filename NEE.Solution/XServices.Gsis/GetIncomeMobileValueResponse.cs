using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.Gsis
{
    public class GetIncomeMobileValueResponse : XServiceResponseBase
    {
        public string SpouseAfm { get; set; }
        public string SpouseAmka { get; set; }
        public decimal? FamilyIncome { get; set; }
        public decimal? Income { get; set; }
        public decimal? VehiclesValue { get; set; }
    }
}

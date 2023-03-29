using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.Efka
{
    public class PensionsOpekaResponse: XServiceResponseBase
    {
        public string ApplicationId { get; set; }
        public string Afm { get; set; }
        public List<Pension> Pensions { get; set; }

    }

    public class Pension
    {        
        public string Amka { get; set; }
        public decimal? GrossAmountBasic { get; set; }
        public decimal? GrossAmountAdditional { get; set; }
        public decimal? Year { get; set; }
        public decimal? Month { get; set; }
    }
}

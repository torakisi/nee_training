using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.Gsis
{
    public class GetPropertyValueE9Response : XServiceResponseBase
    {
        public decimal? AssetsValue { get; set; }
    }
}

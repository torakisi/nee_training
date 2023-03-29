using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.CitizenRegistry
{
    public class GetMaritalStatusResponse : XServiceResponseBase
    {
        public MaritalStatus MaritalStatus { get; set; }
    }
}

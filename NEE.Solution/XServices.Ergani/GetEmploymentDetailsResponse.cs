using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.Ergani
{
    public class GetEmploymentDetailsResponse: XServiceResponseBase
    {
        public bool IsEmployed { get; set; }
    }
}

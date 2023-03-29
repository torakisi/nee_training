using NEE.Core.Contracts;
using System.Collections.Generic;

namespace NEE.Service.Core
{
    public abstract class ApplicationIdentityRequest : NEEServiceRequestBase
    {
        public string Id { get; set; }
        public int Revision { get; set; }

        public override List<string> IsValid()
        {
            return null;
        }
    }
}

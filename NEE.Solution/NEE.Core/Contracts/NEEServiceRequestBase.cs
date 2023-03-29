using NEE.Core.BO;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NEE.Core.Contracts
{
    public abstract class NEEServiceRequestBase
    {
        abstract public List<string> IsValid();
    }
}

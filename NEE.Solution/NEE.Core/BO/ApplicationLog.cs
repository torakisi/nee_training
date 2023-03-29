using NEE.Core.Contracts.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.BO
{
    public class ApplicationLog
    {
        public int Revision { get; set; }
        public DateTime OccuredAt { get; set; }
        public AppState EventType { get; set; }
        public string UserName { get; set; }

    }
}

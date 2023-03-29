using NEE.Core.Contracts.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.BO
{
    public class StateChange
    {
        public string ChangeId { get; set; }

        public string Id { get; set; }
        public AppState StateFrom { get; set; }
        public AppState StateTo { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; }
        public string ChangeReason { get; set; }
        public bool IsFromDb { get; set; } = false;
        public DateTime? ReferenceDate { get; set; }
        public int SearchForAxreostitos { get; set; }
    }
}

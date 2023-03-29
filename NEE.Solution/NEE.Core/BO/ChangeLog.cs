using NEE.Core.Contracts.Enumerations;
using System;

namespace NEE.Core.BO
{
    public class ChangeLog
    {
        public string Id { get; set; }
        public string ChangeLogId { get; set; }
        public string User { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ChangedField { get; set; }
        public string OriginalValue { get; set; }
        public string CurrentValue { get; set; }
        public string EntityType { get; set; }
        public int? Revision { get; set; }
        public string EntityId { get; set; }
        public ChangeLogTypes Type { get; set; }
    }
}
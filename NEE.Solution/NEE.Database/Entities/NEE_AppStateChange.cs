using NEE.Core.Contracts.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEE.Database
{

    public class NEE_AppStateChange
    {
        [Key, Column(Order = 0)]
        [Required]
        public string ChangeId { get; set; }

        public string Id { get; set; }
        public AppState StateFrom { get; set; }
        public AppState StateTo { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; }
        public string ChangeReason { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public int SearchForAxreostitos { get; set; }
    }
}
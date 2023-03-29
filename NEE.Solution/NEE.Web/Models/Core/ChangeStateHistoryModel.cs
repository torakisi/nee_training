using NEE.Core.Contracts.Enumerations;
using System;

namespace NEE.Web.Models.Core
{
    public class ChangeStateHistoryModel
    {
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; }
        public AppState ChangedState { get; set; }
        public string ChangeReason { get; set; }
        public string FullUsername { get; set; }
        public bool ShowUserFullName { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public int SearchForAxreostitos { get; set; }
    }
}
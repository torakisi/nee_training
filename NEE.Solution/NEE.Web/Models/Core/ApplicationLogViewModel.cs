using NEE.Core.Contracts.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NEE.Web.Models.Core
{
    public class ApplicationLogViewModel : ViewModelBase
    {
        public string Id { get; set; }
        public int Revision { get; set; }
        public DateTime OccuredAt { get; set; }
        public AppState EventType { get; set; }
        public string UserName { get; set; }
    }
}
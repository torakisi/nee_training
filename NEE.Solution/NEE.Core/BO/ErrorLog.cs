using NEE.Core.Contracts.Enumerations;
using System;

namespace NEE.Core.BO
{
    public class ErrorLog
    {
        public string User { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Exception { get; set; }
        public string ApplicationId { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
        public ErrorLogSource ErrorLogSource { get; set; } = ErrorLogSource.Unknown;
    }
}

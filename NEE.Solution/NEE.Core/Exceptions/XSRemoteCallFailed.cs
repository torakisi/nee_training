using System;

namespace NEE.Core.Exceptions
{
    public class XSRemoteCallFailed : Exception
    {
        public XSRemoteCallFailed(string xsRemoteService)
            : base($"XS: Call to {xsRemoteService} failed")
        { }
        public XSRemoteCallFailed(string xsRemoteService, Exception innerException)
            : base($"XS: Call to {xsRemoteService} failed: {innerException.Message}", innerException)
        { }
    }
}

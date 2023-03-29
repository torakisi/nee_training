using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.Contracts
{
    public class XServiceBase
    {
        public event EventHandler<XServiceCallMadeEventArgs> XServiceCallMade;
        public event EventHandler<XServiceCallReturnedEventArgs> XServiceCallReturned;

        protected virtual void RaiseCallMadeEvent(XServiceCallMadeEventArgs eventArgs)
        {
            XServiceCallMade?.Invoke(this, eventArgs);
        }

        protected virtual void RaiseCallReturnedEvent(XServiceCallReturnedEventArgs eventArgs)
        {
            XServiceCallReturned?.Invoke(this, eventArgs);
        }

        public class XServiceCallCommonEventArgs
        {
            public string Id { get; set; }

            public string ServiceName { get; set; }
            public string MethodCall { get; set; }
        }

        public class XServiceCallMadeEventArgs : XServiceCallCommonEventArgs
        {
            public string RequestJson { get; set; }
        }

        public class XServiceCallReturnedEventArgs : XServiceCallCommonEventArgs
        {
            public string RequestJson { get; set; }
            public string ResponseJson { get; set; }
            public DateTime RequestCallTimestamp { get; set; }
            public DateTime ResponseCallTimestamp { get; set; }
        }
    }
}

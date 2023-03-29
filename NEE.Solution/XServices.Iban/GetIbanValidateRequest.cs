using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.Iban
{
    public class GetIbanValidateRequest : XServiceRequestBase
    {
        public string IBAN { get; set; }
        public string AFM { get; set; }




        // ----   Those fields added for the WS GSISKED   ------------
        public string RequestId { get; set; }

        public string auditTransactionId { get; set; }

        public System.DateTime? auditTransactionDate { get; set; }

        public string auditUnit { get; set; }

        public string auditProtocol { get; set; }

        public string auditUserId { get; set; }

        public string auditUserIp { get; set; }
    }
}

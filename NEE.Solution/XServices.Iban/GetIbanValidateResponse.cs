using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServices.Iban
{
    public class GetIbanValidateResponse : XServiceResponseBase
    {
        public string RequestId { get; set; }
        public string AFM { get; set; }
        public string IBAN { get; set; }
        public bool? CorrectCombination { get; set; }
        public bool? UsageAllowed { get; set; }
        public DateTime? ValidityTimestamp { get; set; }
        public DateTime CheckTimestamp { get; set; }
        public bool WsHasConsumedSuccessfully { get; set; }
        public string WsStatus { get; set; }
        public string WsErrorMessage { get; set; }
        public bool HasExecutedSuccessfuly { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionInnerMessage { get; set; }
        public int WsResponseTimeMS { get; set; }
        public bool? Cached { get; set; }
        public IbanValidationResponseErrorCode ErrorCode { get; set; }
        public string InnerExceptionMessage { get; set; }
    }

    public class IbanValidationResponseErrorCode
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}

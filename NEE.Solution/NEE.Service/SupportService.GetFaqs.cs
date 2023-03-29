using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;
using System.Collections.Generic;

namespace NEE.Service
{
    public partial class SupportService
    {
        public GetFaqsResponse GetFaqs(GetFaqsRequest req)
        {
            GetFaqsResponse resp = new GetFaqsResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                var faqs = _supportRepository.GetAllFaqs();

                resp.Faqs = faqs;
            }
            catch (Exception ex)
            {
                resp.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return resp;
        }
    }





    public class GetFaqsRequest
    {
        //public string AMKA { get; set; }

        //public Person Person { get; set; }


    }
    public class GetFaqsResponse : NEEServiceResponseBase
    {
        public GetFaqsResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {

        }

        public List<Faq> Faqs { get; set; } = new List<Faq>();
    }
}

using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XServices.Efka;

namespace NEE.Service
{
	partial class PersonService
	{
		public async Task<GetEfkaInfoResponse> GetEfkaInfoAsync(GetEfkaInfoRequest req)
		{
            GetEfkaInfoResponse resp = new GetEfkaInfoResponse();

            var xsReq = new PensionsOpekaRequest
            {
                ApplicationId = req.ApplicationId,
                Afm = req.AFM,
                Amka = req.AMKA,
                DateFrom = req.DateFrom,
                DateTo = req.DateTo,
            };

            try
			{
                // Λαμβάνω πληροφορίες του ατόμου από Έφκα
                var xsRes = await _xsEfkaService.GetPensionsOpekaInfoAsync(xsReq);

                if (xsRes._IsSuccessful)
                {
                    // return efka info
                    resp.Pensions = xsRes.Pensions;
                }
				else
				{
					resp.AddErrors(xsRes._Errors);
				}
			}
			catch (Exception ex)
			{
				resp.AddError(ErrorCategory.Unhandled, null, ex);
			}

			return resp;
		}

        public class GetEfkaInfoRequest
        {
            public string ApplicationId { get; set; }
            public string AMKA { get; set; }
            public string AFM { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }
        public class GetEfkaInfoResponse : NEEServiceResponseBase
        {
            public GetEfkaInfoResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
            {
            }
            public List<Pension> Pensions { get; set; }
        }
    }
}

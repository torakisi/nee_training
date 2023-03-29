using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;
using System.Threading.Tasks;
using XServices.Edto;
using XServices.Ergani;

namespace NEE.Service
{
	partial class PersonService
	{
		public async Task<GetErganiInfoResponse> GetErganiInfoAsync(GetErganiInfoRequest req)
		{
            GetErganiInfoResponse resp = new GetErganiInfoResponse();

			try
			{
                var xsReq = new GetEmploymentDetailsRequest
                {
                    ApplicationId = req.ApplicationId,
                    Afm = req.Afm,
                    Amka = req.Amka,
                    RefDate = req.RefDate
                };

                // Λαμβάνω πληροφορίες του ατόμου από Εργάνη
                var xsRes = await _xsErganiService.GetEmploymentDetailsAsync(xsReq);

                if (xsRes._IsSuccessful)
                {
                    // return ergani info
                    resp.IsEmployed = xsRes.IsEmployed;
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

        public class GetErganiInfoRequest
        {
            public string ApplicationId { get; set; }
            public string Afm { get; set; }
            public string Amka { get; set; }
            public string RefDate { get; set; }
        }
        public class GetErganiInfoResponse : NEEServiceResponseBase
        {
            public GetErganiInfoResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
            {
            }
            public bool IsEmployed { get; set; }
        }
    }
}

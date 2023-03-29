using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;
using System.Threading.Tasks;
using XServices.CitizenRegistry;

namespace NEE.Service
{
	partial class PersonService
	{
		public async Task<GetMaritalStatusInfoResponse> GetMaritalStatusAsync(GetMaritalStatusInfoRequest req)
		{
            GetMaritalStatusInfoResponse resp = new GetMaritalStatusInfoResponse();

			try
			{
                var xsReq = new GetMaritalStatusRequest
                {
                    ApplicationId = req.ApplicationId,
                    Afm = req.Afm,
                    Amka = req.Amka,
                    FirstName = req.FirstName,
                    LastName = req.LastName,
                    FatherName = req.FatherName,
                    MotherName = req.MotherName,
                    BirthDate = req.BirthDate
                };

                // Λαμβάνω πληροφορίες του ατόμου από μητρώο πολιτών
                var xsRes = await _citizenRegistryService.GetMaritalStatusCertificateAsync(xsReq);

                if (xsRes._IsSuccessful)
                {
                    resp.MaritalStatus = xsRes.MaritalStatus;
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

        public class GetMaritalStatusInfoRequest
        {
            public string ApplicationId { get; set; }
            public string Afm { get; set; }
            public string Amka { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string FatherName { get; set; }
            public string MotherName { get; set; }
            public string BirthDate { get; set; }
        }

        public class GetMaritalStatusInfoResponse : NEEServiceResponseBase
        {
            public GetMaritalStatusInfoResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
            {
            }
            public MaritalStatus MaritalStatus { get; set; }
        }
    }
}

using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;
using System.Threading.Tasks;
using XServices.Edto;

namespace NEE.Service
{
	partial class PersonService
	{
		public async Task<GetEDTOResponse> GetEDTOInfoAsync(GetEDTORequest req)
		{
            GetEDTOResponse resp = new GetEDTOResponse();

			try
			{
                var xsReq = new GetPolicePersonDetailsRequest
                {
                    FirstName = req.FirstName,
                    LastName = req.LastName,
                    FathersName = req.FathersName,
                    MothersName = req.MothersName,
                    BirthDate = req.BirthDate,
                    ApplicationId = req.ApplicationId,
                    Afm = req.Afm,
                    Amka = req.Amka
                };

                // Λαμβάνω πληροφορίες του ατόμου για ΕΔΤΟ
                var xsRes = await _xsKedService.GetPolicePersonDetailsAsync(xsReq);

                resp.GsisServiceResponse = xsRes;


                if (xsRes._IsSuccessful)
                {
                    resp.PermitNumber = xsRes.PermitNumber;
                    resp.AdministrationDate = xsRes.AdministrationDate;                   
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

        public class GetEDTORequest
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string FathersName { get; set; }
            public string MothersName { get; set; }
            public string BirthDate { get; set; }
            public string ApplicationId { get; set; }
            public string Afm { get; set; }
            public string Amka { get; set; }
        }
        public class GetEDTOResponse: NEEServiceResponseBase
        {
            public GetEDTOResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
            {
            }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string FathersName { get; set; }
            public string MothersName { get; set; }
            public string BirthDate { get; set; }
            public string PermitNumber { get; set; }
            public DateTime? AdministrationDate { get; set; }
            public GetPolicePersonDetailsResponse GsisServiceResponse { get; set; }
        }
    }
}

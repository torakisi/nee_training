using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;
using System.Threading.Tasks;
using XServices.Edto;
using XServices.Gsis;

namespace NEE.Service
{
	partial class PersonService
	{
		public async Task<GetIncomeMobileValueInfoResponse> GetIncomeMobileValueAsync(GetIncomeMobileValueInfoRequest req)
		{
            GetIncomeMobileValueInfoResponse resp = new GetIncomeMobileValueInfoResponse();

			try
			{
                var xsReq = new GetIncomeMobileValueRequest
                {
                    ApplicationId = req.ApplicationId,
                    Afm = req.Afm,
                    Amka = req.Amka,
                    ReferenceYear = req.ReferenceYear
                };

                // Λαμβάνω πληροφορίες του ατόμου από ΑΑΔΕ
                var xsRes = await _gsisInfoService.GetIncomeMobileValueAsync(xsReq);

                if (xsRes._IsSuccessful)
                {
                    resp.SpouseAfm = xsRes.SpouseAfm;
                    resp.SpouseAmka = xsRes.SpouseAmka;
                    resp.FamilyIncome = xsRes.FamilyIncome;
                    resp.Income = xsRes.Income;
                    resp.VehiclesValue = xsRes.VehiclesValue;
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

        public class GetIncomeMobileValueInfoRequest
        {
            public string ApplicationId { get; set; }
            public string Afm { get; set; }
            public string Amka { get; set; }
            public int ReferenceYear { get; set; }
        }
        public class GetIncomeMobileValueInfoResponse : NEEServiceResponseBase
        {
            public GetIncomeMobileValueInfoResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
            {
            }
            public string SpouseAfm { get; set; }
            public string SpouseAmka { get; set; }
            public decimal? FamilyIncome { get; set; }
            public decimal? Income { get; set; }
            public decimal? VehiclesValue { get; set; }
        }
    }
}

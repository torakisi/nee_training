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
		public async Task<GetPropertyValueInfoResponse> GetPropertyValueInfoAsync(GetPropertyValueInfoRequest req)
		{
            GetPropertyValueInfoResponse resp = new GetPropertyValueInfoResponse();

			try
			{
                var xsReq = new GetPropertyValueE9Request
                {
                    ApplicationId = req.ApplicationId,
                    Afm = req.Afm,
                    Amka = req.Amka,
                    ReferenceYear= req.ReferenceYear
                };

                // Λαμβάνω πληροφορίες του ατόμου από στοιχεία κατοικίας ΑΑΔΕ
                var xsRes = await _gsisPropertyService.GetPropertyValueInfoAsync(xsReq);
                
                if (xsRes._IsSuccessful)
                {
                    resp.AssetsValue = xsRes.AssetsValue;
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

        public class GetPropertyValueInfoRequest
        {
            public string ApplicationId { get; set; }
            public string Afm { get; set; }
            public string Amka { get; set; }
            public int ReferenceYear { get; set; }
        }
        public class GetPropertyValueInfoResponse : NEEServiceResponseBase
        {
            public GetPropertyValueInfoResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
            {
            }
            public decimal? AssetsValue { get; set; }
        }
    }
}

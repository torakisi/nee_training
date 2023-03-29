using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;
using System.Threading.Tasks;
using XServices.NotificationCenter;
using XServices.Edto;

namespace NEE.Service
{
	partial class PersonService
	{
		public async Task<GetNotificationCenterInfoResponse> GetNotificationCenterInfoAsync(GetNotificationCenterInfoRequest req)
		{
            GetNotificationCenterInfoResponse resp = new GetNotificationCenterInfoResponse();

			try
			{
                var xsReq = new GetNncIdentityExtRequest
                {
                    ApplicationId = req.ApplicationId,
                    Afm = req.Afm,
                    Amka = req.Amka
                };

                // Λαμβάνω πληροφορίες του ατόμου από Μητρώο Πολιτών
                var xsRes = await _xsNotificationCenterService.GetNncIdentityExtAsync(xsReq);

                if (xsRes._IsSuccessful)
                {
                    // return citizen registry info
                    resp.Email = xsRes.Email;
                    resp.MobilePhone = xsRes.MobilePhone;
                    resp.HomePhone = xsRes.HomePhone;
                    resp.AddressStreet = xsRes.AddressStreet;
                    resp.AddressNumber = xsRes.AddressNumber;
                    resp.AddressCity = xsRes.AddressCity;
                    resp.AddressZip = xsRes.AddressZip;
                    resp.AddressPostalNumber = xsRes.AddressPostalNumber;
                    resp.Region = xsRes.Region;
                    resp.RegionalUnit = xsRes.RegionalUnit;
                    resp.Municipality = xsRes.Municipality;
                    resp.MunicipalUnit = xsRes.MunicipalUnit;
                    resp.Commune = xsRes.Commune;
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

        public class GetNotificationCenterInfoRequest
        {
            public string ApplicationId { get; set; }
            public string Afm { get; set; }
            public string Amka { get; set; }
        }
        public class GetNotificationCenterInfoResponse : NEEServiceResponseBase
        {
            public GetNotificationCenterInfoResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
            {
            }
            public string Email { get; set; }
            public string MobilePhone { get; set; }
            public string HomePhone { get; set; }
            public string AddressStreet { get; set; }
            public string AddressNumber { get; set; }
            //Πόλη/Χωριό (locality)
            public string AddressCity { get; set; }
            public string AddressZip { get; set; }
            public string AddressPostalNumber { get; set; }
            //Περιφέρεια
            public string Region { get; set; }
            //Περιφέρεια Ενότητα
            public string RegionalUnit { get; set; }
            //Δήμος
            public string Municipality { get; set; }
            //Δημοτική Ενότητα
            public string MunicipalUnit { get; set; }
            //Δημοτική Κοινότητα
            public string Commune { get; set; }
        }
    }
}

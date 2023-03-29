using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Exceptions;
using NEE.Core.Helpers;
using NEE.Core.Validation;
using NEE.Database;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XService.Idika;

namespace XServices.Idika
{
	public class IDIKAService : XServiceBase
	{
		public readonly string ServiceName = "¡Ã ¡";

		private readonly AmkaServiceGateway amkaGateway;
		private readonly AmkaMaritalStatusGateway maritalStatusGateway;
        private readonly OtherBenefitsGateway otherBenefitsGateway;

        /// <summary>
        /// Initializes a new instance of the IDIKAService class.
        /// </summary>
        /// <param name="amkaGateway">The AMKA WebService gateway.</param>        
        public IDIKAService(AmkaServiceGateway amkaGateway, AmkaMaritalStatusGateway maritalStatusGateway, OtherBenefitsGateway otherBenefitsGateway)
		{
			this.amkaGateway = amkaGateway;
			this.maritalStatusGateway = maritalStatusGateway;
			this.otherBenefitsGateway = otherBenefitsGateway;
		}

		/// <summary>
		/// Initializes a new instance of the IDIKAService class with default arguments.
		/// </summary>
		/// <returns>An IDIKAService instance with default arguments.</returns>
		/// <remarks>
		/// NOTE: Use only for internal/debug reasons.
		/// </remarks>
		public static IDIKAService CreateDefault()
		{
			var amkaGateway =
				new AmkaServiceGateway(
					new AmkaWebServiceConnectionString("wsafmhb|Gdg43$#fdsBr45|https://www.idika.gov.gr/webservices/amka/AFM2DATA/Service.asmx"));
			var maritalStatusGateway =
				new AmkaMaritalStatusGateway(
					NEEDbContextFactory.CreateDefault());
            var otherBenefitsGateway =
                new OtherBenefitsGateway(
                    NEEDbContextFactory.CreateDefault());
            return new IDIKAService(amkaGateway, maritalStatusGateway, otherBenefitsGateway);
		}

		// Interface-Implementation: IXS_IDIKA      
		public async Task<GetAmkaRegistryInfoResponse> GetAmkaRegistryInfoAsync(GetAmkaRegistryInfoRequest req)
		{
			if (req.AMKA == null && req.AFM == null)
				throw new ArgumentException("ƒÂÌ ›˜ÂÈ ÔÒÈÛÙÂﬂ Ô˝ÙÂ ¡Ã ¡ Ô˝ÙÂ ¡÷Ã ÒÔÚ ·Ì·ÊﬁÙÁÛÁ");

			try
			{
				Guid id = Guid.NewGuid();
				DateTime requestCallTimestamp = DateTime.Now;

				GetAmkaRegistryInfoResponse response = null;
				if (req.AFM == null)
					response = await GetAmkaRegistryInfoByAmka(req.AMKA);
				else if (req.AMKA == null)
					response = await GetAmkaRegistryInfoByAfm(req.AFM);
				else
					response = await GetAmkaRegistryInfoByAmkaExpectingAfm(req.AMKA, req.AFM);

				return response;
			}
			catch (XSRemoteCallFailed ex)
			{
				return GetAmkaRegistryInfoResponse.Exception(ex, ServiceName);
			}
		}


        public async Task<GetOtherBenefitsInfoResponse> GetOtherBenefitInfoAsync(GetOtherBenefitsInfoRequest req)
        {
            var res = new GetOtherBenefitsInfoResponse();

            try
            {
                res.OtherBenefits = await otherBenefitsGateway.GetOtherBenefits(req.AMKA, req.YearMonth);
                res._IsSuccessful = true;
            }
            catch (Exception ex)
            {
                res.AddError(ErrorCategory.Unhandled, ex.InnerException.Message);
            }
            return res;
        }
        private async Task<GetAmkaRegistryInfoResponse> GetAmkaRegistryInfoByAmka(string amka)
		{
			var resAmka = await amkaGateway.GetAmkaRegistryInfo(amka, null);

			GetAmkaRegistryInfoResponse res;
			if (FoundNoRecords(resAmka.RECORDS, resAmka.STATUS, resAmka.MESSAGE))
				res = GetAmkaRegistryInfoResponse.NotFoundAmka(amka);
			else if (resAmka.STATUS != "0")
				res = GetAmkaRegistryInfoResponse.RemoteError($"{resAmka.STATUS} - {resAmka.MESSAGE}");
			else if (resAmka.RECORDS.Count > 1)
				res = GetAmkaRegistryInfoResponse.FoundMoreThanOneForAmka(resAmka.RECORDS.Count, amka);
			else
				res = GetAmkaRegistryResponse(amka, resAmka.RECORDS.Single());

			return res;
		}
		private async Task<GetAmkaRegistryInfoResponse> GetAmkaRegistryInfoByAfm(string afm)
		{
			var resAmka = await amkaGateway.GetAmkaRegistryInfo(null, afm);
			GetAmkaRegistryInfoResponse res;

			if (FoundNoRecords(resAmka.RECORDS, resAmka.STATUS, resAmka.MESSAGE))
				res = GetAmkaRegistryInfoResponse.NotFoundAfm(afm);
			else if (resAmka.STATUS != "0")
				res = GetAmkaRegistryInfoResponse.RemoteError($"{resAmka.STATUS} - {resAmka.MESSAGE}");
			else if (resAmka.RECORDS.Count > 1)
				res = GetAmkaRegistryInfoResponse.FoundMoreThanOneForAfm(resAmka.RECORDS.Count, afm);
			else
				res = GetAmkaRegistryResponse(null, resAmka.RECORDS.Single());

			return res;
		}
		private async Task<GetAmkaRegistryInfoResponse> GetAmkaRegistryInfoByAmkaExpectingAfm(string amka, string expectedAfm)
		{
			GetAmkaRegistryInfoResponse response = new GetAmkaRegistryInfoResponse();

			try
			{
				// var resAmka = await amkaGateway.GetAmkaRegistryInfo(amka, expectedAfm);
				var resAmka = await amkaGateway.GetAmkaRegistryInfo(amka, null);
				if (resAmka.RECORDS == null)
				{
					return GetAmkaRegistryInfoResponse.RemoteError($"{resAmka.STATUS} - {resAmka.MESSAGE}");
				}

				var rows = resAmka.RECORDS;

				if (rows.Any(x => x.AFM != null))
					rows = rows.Where(x => this.TranslateAFM(x.AFM) == expectedAfm).ToList();

				if (FoundNoRecords(rows, resAmka.STATUS, resAmka.MESSAGE))
					return GetAmkaRegistryInfoResponse.NotFoundAmkaAfmCombination(amka, expectedAfm);
				else if (resAmka.STATUS != "0")
					return GetAmkaRegistryInfoResponse.RemoteError($"{resAmka.STATUS} - {resAmka.MESSAGE}");
				else if (rows.Count > 1)
					return GetAmkaRegistryInfoResponse.FoundMoreThanOneForAmkaAfmCombination(rows.Count, amka, expectedAfm);
				else
					response = GetAmkaRegistryResponse(amka, resAmka.RECORDS.Single());
			}
			catch (Exception ex)
			{
				response.AddError(ErrorCategory.Unhandled, null, ex);
				response.AddError(ErrorCategory.UIDisplayedServiceCallFailure, String.Format(ServiceErrorMessages.UnableToCommunicateWithService, ServiceName));
			}


			return response;
		}




		private static bool FoundNoRecords(IEnumerable<XService.Idika.AMKA_ServiceReference.Details> rows, string status, string message)
		{
			if (rows != null && rows.Count() == 0)
				return true;
			else if (status == "1" && message == "ƒÂÌ ‚Ò›ËÁÍ·Ì Â„„Ò·ˆ›Ú ÏÂ ·ıÙ‹ Ù· ÍÒÈÙﬁÒÈ·.")
				return true;
			else
				return false;
		}


		private GetAmkaRegistryInfoResponse GetAmkaRegistryResponse(string amkaRequested, XService.Idika.AMKA_ServiceReference.Details row)
		{    

			GetAmkaRegistryInfoResponse res = new GetAmkaRegistryInfoResponse();
			res.AMKA = row.AMKA_ISXYWN;
			res.AFM = this.TranslateAFM(row.AFM);
			res.LastName = row.EPWN_TREX_GR;
			res.FirstName = row.ONOM_GR;
			res.LastNameBirth = row.EPWN_GEN_GR;
			res.LastNameEN = row.EPWN_TREX_LAT;
			res.FirstNameEN = row.ONOM_LAT;
			res.FatherName = row.PATR_GR;
			res.MotherName = row.MHTR_GR;
            res.FatherNameEN = row.PATR_LAT;
            res.MotherNameEN = row.MHTR_LAT;
            res.Gender = this.TranslateGender(row.KWD_FYLOY);
			res.DOB = row.HMNIA_GEN;
			res.BirthCountry = row.XWRA_GR_GEN;
            res.CitizenCountry = row.XWRA_GR_YPHKOOT;
            res.AMKARegistationDate = row.HMNIA_KATAXWR;

			var identiticationNumberType = TranslateIdentificationType(row.KWD_EIDOYS_TAYTOT);
			if (identiticationNumberType.HasValue)
			{
				res.IdentificationNumber = row.ARIQ_TAYTOT;
				res.IdentificationNumberType = identiticationNumberType;
			}

			var deathStatus = this.TranslateDeathStatus(row.END_QANATOY);
			var dod = row.HMNIA_QANATOY;

			// do we have any death-information?
			if ((deathStatus != null) && (dod != null))
			{
				if ((res.DOB == null) || (dod >= res.DOB))
				{
					// death-information seems ok, return it...
					res.DeathStatus = deathStatus;
					res.DOD = dod;
				}
			}

			res._IsSuccessful = true;
			return res;
		}


		// Support-Code: TranslateAFM, TranslateGender

		/// <summary>
		/// Translate AFM returned by AFM2D (AMKA Service)
		/// </summary>
		/// <remarks>
		/// AFM2D returns 11-digit AFM, this functions returns last 9 digits (only if 11-digits and starts with "00")
		/// </remarks>
		private string TranslateAFM(string AFM)
		{
			var ret = AFM;
			if (!string.IsNullOrWhiteSpace(ret))
			{
				if ((ret.Length == 11) && (ret.StartsWith("00")))
					ret = ret.Substring(2);
			}
			return ret;
		}

		/// <summary>
		/// Translate KWD_FYLOY returned by AFM2D (AMKA Service) to Gender
		/// </summary>
		private Gender TranslateGender(string KWD_FYLOY)
		{
			var ret = KWD_FYLOY.StartsWith("»") ? Gender.Female : Gender.Male;
			return ret;
		}

		private DeathStatus? TranslateDeathStatus(string END_QANATOY)
		{
			if (END_QANATOY == null) return null;
			if (END_QANATOY.StartsWith("»")) return DeathStatus.AnnouncedDead;
			if (END_QANATOY.StartsWith("‘")) return DeathStatus.AnnouncedDeadByInstitution;
			if (END_QANATOY.StartsWith("À")) return DeathStatus.RegisteredDead;
			return DeathStatus.OtherStatus;
		}

		private IdentificationNumberType? TranslateIdentificationType(string code)
		{
			if (code == null) return null;
			if (code.StartsWith("ƒ")) return IdentificationNumberType.Passport;
			if (code.StartsWith("‘")) return IdentificationNumberType.ADT;
			return null;
			//” ≈ÀÀ«Õ… « ”‘—¡‘…Ÿ‘… «            
			//¡   ¡ÀÀœ –¡—¡”‘¡‘… œ            
			//Ã   Ã≈—…ƒ¡ œ… œ√≈Õ≈…¡ «
			//À À«Œ…¡—◊… « –—¡Œ« √≈ÕÕ«”«”
			//œ ‘¡’‘œ‘«‘¡ œÃœ√≈Õœ’”
			//1	¡ƒ.ƒ…¡Ã.ƒ… ¡…œ’◊.ƒ…≈»Õ.–—œ”‘¡”
			//2	ƒ≈À‘.¡…‘œ’Õ‘œ” ƒ…≈».–—œ”‘¡”…¡”
			//3	ƒ≈À‘.¡…‘«”¡Õ‘œ” ¡”’Àœ’
		}
	}
}

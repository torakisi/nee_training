using NEE.Core;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using XServices.Idika;

namespace XService.Idika
{

	public class AmkaSnapshotIdikaService
	{
		public readonly string ServiceName = "Μητρώο ΑΜΚΑ";

		private readonly AmkaSnapshotGateway amkaGateway;

		public AmkaSnapshotIdikaService(AmkaSnapshotGateway amkaGateway)
		{
			this.amkaGateway = amkaGateway;
		}


		public Task<GetAmkaRegistryInfoResponse> GetAmkaRegistryInfoAsync(GetAmkaRegistryInfoRequest req)
		{
			if (req.AMKA == null && req.AFM == null)
				throw new ArgumentException("Δεν έχει οριστεί ούτε ΑΜΚΑ ούτε ΑΦΜ προς αναζήτηση");

			try
			{
				if (req.AFM == null)
					return GetAmkaRegistryInfoByAmka(req.AMKA);
				else if (req.AMKA == null)
					return GetAmkaRegistryInfoByAfm(req.AFM);
				else
					return GetAmkaRegistryInfoByAmkaExpectingAfm(req.AMKA, req.AFM);
			}
			catch (XSRemoteCallFailed ex)
			{
				return Task.FromResult(GetAmkaRegistryInfoResponse.Exception(ex, ServiceName));
			}
		}

		private async Task<GetAmkaRegistryInfoResponse> GetAmkaRegistryInfoByAmka(string amka)
		{
			var resAmka = await amkaGateway.GetAmkaRegistryInfoByAmka(amka);

			GetAmkaRegistryInfoResponse res;
			if (resAmka.FoundNoRecords())
				res = GetAmkaRegistryInfoResponse.NotFoundAmka(amka);
			else if (resAmka.Rows.Count > 1)
				res = GetAmkaRegistryInfoResponse.FoundMoreThanOneForAmka(resAmka.Rows.Count, amka);
			else
				res = GetAmkaRegistryResponse(resAmka.Rows.Single());

			return res;
		}
		private async Task<GetAmkaRegistryInfoResponse> GetAmkaRegistryInfoByAfm(string afm)
		{
			var resAmka = await amkaGateway.GetAmkaRegistryInfoByAfm(afm);
			GetAmkaRegistryInfoResponse res;

			if (resAmka.FoundNoRecords())
				res = GetAmkaRegistryInfoResponse.NotFoundAfm(afm);
			else if (resAmka.Rows.Count > 1)
				res = GetAmkaRegistryInfoResponse.FoundMoreThanOneForAfm(resAmka.Rows.Count, afm);
			else
				res = GetAmkaRegistryResponse(resAmka.Rows.Single());

			return res;
		}
		private async Task<GetAmkaRegistryInfoResponse> GetAmkaRegistryInfoByAmkaExpectingAfm(string amka, string expectedAfm)
		{
			var resAmka = await amkaGateway.GetAmkaRegistryInfoByAmka(amka);
			var rows = resAmka.Rows;

			if (rows.Any(x => x.AFM != null))
				rows = rows.Where(x => x.AFM == expectedAfm).ToList();

			GetAmkaRegistryInfoResponse res;
			if (resAmka.FoundNoRecords())
				res = GetAmkaRegistryInfoResponse.NotFoundAmkaAfmCombination(amka, expectedAfm);
			else if (rows.Count > 1)
				res = GetAmkaRegistryInfoResponse.FoundMoreThanOneForAmkaAfmCombination(rows.Count, amka, expectedAfm);
			else
				res = GetAmkaRegistryResponse(resAmka.Rows.Single());

			return res;
		}


		private GetAmkaRegistryInfoResponse GetAmkaRegistryResponse(AmkaRow row)
		{
			//if (res.AMKA != amkaRequested)
			//{
			//    //var msg = $"Ο συγκεκριμένος ΑΜΚΑ ({req.AMKA}) δεν ισχύει πλέον, χρησιμοποιήστε τον ΑΜΚΑ που τον αντικαθιστά (*********{res.AMKA.Substring(res.AMKA.Length - 2)})";
			//    //res.AddError(ErrorCategory.Error, msg);
			//    //return Task.FromResult(res);
			//}               

			GetAmkaRegistryInfoResponse res = new GetAmkaRegistryInfoResponse();
			res.AMKA = row.AMKA;
			res.AFM = row.AFM;
			res.LastName = row.LastName.Truncate(40);
			res.FirstName = row.FirstName.Truncate(40);
			res.LastNameEN = row.LastNameEN.Truncate(40);
			res.FirstNameEN = row.FirstNameEN.Truncate(40);
			res.FatherName = row.FatherName.Truncate(40);
			res.MotherName = row.MotherName.Truncate(40);
			res.Gender = row.GetGender();
			res.DOB = row.DOB;
			res.CitizenCountry = row.CitizenCountry;
            res.BirthCountry = row.BirthCountry;
            res.AMKARegistationDate = row.ModifiedAt;
			res.DOD = row.DOD;
			if (row.DOD.HasValue)
				res.DeathStatus = DeathStatus.RegisteredDead;

			res._IsSuccessful = true;
			return res;
		}
	}
}

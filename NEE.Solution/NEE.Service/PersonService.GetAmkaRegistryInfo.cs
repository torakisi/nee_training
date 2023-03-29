using NEE.Core;
using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XServices.Idika;

namespace NEE.Service
{
    partial class PersonService
    {
        public async Task<GetAmkaRegistryResponse> GetAmkaRegistryAsync(GetAmkaRegistryRequest req)
        {
            GetAmkaRegistryResponse resp = new GetAmkaRegistryResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                var person = req.Person;
                if (req.Person == null)
                {
                    person = new Person();

                    person.AFM = req.AFM;
                    person.AMKA = req.AMKA;
                }

                var xsReq = new GetAmkaRegistryInfoRequest
                {
                    AMKA = person.AMKA,
                    AFM = person.AFM,       // Special: if AMKA not specified, LookUp AFM=>AMKA and try that AMKA
                };

                var xsRes = await _xsIdikaService.GetAmkaRegistryInfoAsync(xsReq);
                resp.AmkaServiceResponse = xsRes;

                if (xsRes._IsSuccessful)
                {
                    // person info
                    person.AMKA = new Amka(xsRes.AMKA);
                    if (Afm.IsValid(person.AFM))
                    {
                        //if (string.IsNullOrEmpty(xsRes.AFM))
                        //{
                        //    resp.AddError(ErrorCategory.Error, "Δε βρέθηκε ο συγκεκριμένος ΑΦΜ στο Μητρώο του ΑΜΚΑ. Απευθυνθείτε σε ΚΕΠ για την ενημέρωση του Μητρώου ΑΜΚΑ.");
                        //    return resp;
                        //}
                        // person.AFM = new Afm(xsRes.AFM);
                    }
                    person.LastName = xsRes.LastName.Truncate(40);
                    person.LastNameBirth = xsRes.LastNameBirth.Truncate(40);
                    person.FirstName = xsRes.FirstName.Truncate(40);
                    person.LastNameEN = xsRes.LastNameEN.Truncate(40);
                    person.FirstNameEN = xsRes.FirstNameEN.Truncate(40);
                    person.FatherName = xsRes.FatherName;
                    person.MotherName = xsRes.MotherName;
                    person.FatherNameEN = xsRes.FatherNameEN;
                    person.MotherNameEN = xsRes.MotherNameEN;
                    person.CitizenCountry = xsRes.CitizenCountry;
                    person.BirthCountry = xsRes.BirthCountry;
                    person.Gender = xsRes.Gender;
                    person.DOB = xsRes.DOB;
                    person.DeathStatus = xsRes.DeathStatus;
                    person.DOD = xsRes.DOD;

                    if (string.IsNullOrEmpty(person.IdentificationNumber))
                    {
                        person.IdentificationNumber = xsRes.IdentificationNumber;
                        person.IdentificationNumberType = xsRes.IdentificationNumberType;
                    }

                    if (person.AFM == null)
                        person.AFM = xsRes.AFM;

                    resp.Person = person;
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

        public class GetAmkaRegistryRequest
        {
            public string AFM { get; set; }
            public string AMKA { get; set; }

            public Person Person { get; set; }
        }

        public class GetAmkaRegistryResponse : NEEServiceResponseBase
        {
            public GetAmkaRegistryResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
            {
            }

            public static GetAmkaRegistryResponse NotFound(string amka)
            {
                var res = new GetAmkaRegistryResponse();
                res.AddError(ErrorCategory.Not_Found, $"Δεν βρέθηκε απο τo μητρώο του ΑΜΚΑ: {amka}");
                return res;
            }

            public Person Person { get; set; }
            public GetAmkaRegistryInfoResponse AmkaServiceResponse { get; set; }
        }
    }
}

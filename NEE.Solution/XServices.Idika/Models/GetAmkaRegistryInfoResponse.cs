using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Validation;
using System;
using System.ComponentModel;

namespace XServices.Idika
{
    public class GetAmkaRegistryInfoResponse : XServiceResponseBase
    {
        public string AMKA { get; set; }
        public string AFM { get; set; }

        public string LastName { get; set; }
        public string LastNameBirth { get; set; }
        public string FirstName { get; set; }
        public string LastNameEN { get; set; }
        public string FirstNameEN { get; set; }

        public string FatherName { get; set; }
        public string FatherNameEN { get; set; }
        public string MotherName { get; set; }
        public string MotherNameEN { get; set; }

        public Gender Gender { get; set; }

        public DateTime DOB { get; set; }

        public DeathStatus? DeathStatus { get; set; }

        public DateTime? DOD { get; set; }

        public string CitizenCountry { get; set; }

        public string BirthCountry { get; set; }

        public DateTime? AMKARegistationDate { get; set; }

        public string IdentificationNumber { get; set; }
        public IdentificationNumberType? IdentificationNumberType { get; set; }


        private static GetAmkaRegistryInfoResponse Error(string message)
        {
            var res = new GetAmkaRegistryInfoResponse();
            res.AddError(ErrorCategory.UIDisplayed, message);
            return res;
        }

        private static GetAmkaRegistryInfoResponse Error(ErrorCode errorCode, string message)
        {
            var res = Error(message);
            res._ErrorCode = errorCode;
            return res;
        }

        public enum ErrorCode
        {
            [Description("Δεν βρέθηκε πρόσωπο με αυτό το ΑΜΚΑ")]
            NotFoundAmka = 1,

            [Description("Δεν βρέθηκε πρόσωπο με αυτό το ΑΦΜ")]
            NotFoundAfm = 2,

            [Description("Δεν βρέθηκε πρόσωπο με αυτό το συνδυασμό ΑΜΚΑ/ΑΦΜ")]
            NotFoundAmkaAfmCombination = 3,

            [Description("Βρέθηκαν περισσότερα από ένα πρόσωπα με αυτό το ΑΜΚΑ")]
            FoundMoreThanOneForAmka = 4,

            [Description("Βρέθηκαν περισσότερα από ένα πρόσωπα με αυτό το ΑΦΜ")]
            FoundMoreThanOneForAfm = 5,

            [Description("Βρέθηκαν περισσότερα από ένα πρόσωπα με αυτό το συνδυασμό ΑΜΚΑ/ΑΦΜ")]
            FoundMoreThanOneForAmkaAfmCombination = 6
        }
        public ErrorCode? _ErrorCode { get; set; }

        public static GetAmkaRegistryInfoResponse NotFoundAmka(string amka) =>
            Error(ErrorCode.NotFoundAmka, $"Δεν βρέθηκε στο μητρώο του ΑΜΚΑ πρόσωπο με αυτό το ΑΜΚΑ ({amka})");

        public static GetAmkaRegistryInfoResponse NotFoundAfm(string afm) =>
            Error(ErrorCode.NotFoundAfm, $"Δεν βρέθηκε στο μητρώο του ΑΜΚΑ πρόσωπο με αυτό το ΑΦΜ ({afm})");

        public static GetAmkaRegistryInfoResponse NotFoundAmkaAfmCombination(string amka, string afm) =>
            Error(ErrorCode.NotFoundAmkaAfmCombination, $"Δεν βρέθηκε στο μητρώο του ΑΜΚΑ πρόσωπο με αυτό το συνδυασμό ΑΜΚΑ/ΑΦΜ ({amka}/{afm})");

        public static GetAmkaRegistryInfoResponse FoundMoreThanOneForAmka(int numberFound, string amka)
        {
            if (numberFound <= 1)
                throw new ArgumentOutOfRangeException(nameof(numberFound), $"{nameof(numberFound)} should be a number > 1");
            return Error(ErrorCode.FoundMoreThanOneForAmka, $"Βρέθηκαν {numberFound} πρόσωπα στο μητρώο του ΑΜΚΑ με αυτό το ΑΜΚΑ ({amka})");
        }
        public static GetAmkaRegistryInfoResponse FoundMoreThanOneForAfm(int numberFound, string afm)
        {
            if (numberFound <= 1)
                throw new ArgumentOutOfRangeException(nameof(numberFound), $"{nameof(numberFound)} should be a number > 1");
            return Error(ErrorCode.FoundMoreThanOneForAfm, $"Βρέθηκαν {numberFound} πρόσωπα στο μητρώο του ΑΜΚΑ με αυτό το ΑΦΜ ({afm})");
        }
        public static GetAmkaRegistryInfoResponse FoundMoreThanOneForAmkaAfmCombination(int numberFound, string amka, string afm)
        {
            if (numberFound <= 1)
                throw new ArgumentOutOfRangeException(nameof(numberFound), $"{nameof(numberFound)} should be a number > 1");
            return Error(ErrorCode.FoundMoreThanOneForAmkaAfmCombination, $"Βρέθηκαν {numberFound} πρόσωπα στο μητρώο του ΑΜΚΑ με αυτό το συνδυασμό ΑΜΚΑ/ΑΦΜ ({amka}/{afm})");
        }

        public static GetAmkaRegistryInfoResponse RemoteError(string remoteErrorMessage)
        {
            var res = new GetAmkaRegistryInfoResponse();
            res.AddError(ErrorCategory.Remote_Error_Message, remoteErrorMessage);
            return res;
        }

        public static GetAmkaRegistryInfoResponse Exception(Exception ex, string ServiceName)
        {
            var res = new GetAmkaRegistryInfoResponse();
            res.AddError(ErrorCategory.Unhandled, null, ex);
            res.AddError(ErrorCategory.UIDisplayedServiceCallFailure, String.Format(ServiceErrorMessages.UnableToCommunicateWithService, ServiceName));
            return res;
        }


        public static GetAmkaRegistryInfoResponse FoundMoreThanOne(string amka, string afm, int numberFound)
        {
            if (amka != null && afm != null)
                return FoundMoreThanOneForAmkaAfmCombination(numberFound, amka, afm);
            else if (amka != null)
                return FoundMoreThanOneForAmka(numberFound, amka);
            else
                return FoundMoreThanOneForAfm(numberFound, afm);
        }
        public static GetAmkaRegistryInfoResponse NotFound(GetAmkaRegistryInfoRequest req)
        {
            if (req.AMKA != null && req.AFM != null)
                return NotFoundAmkaAfmCombination(req.AMKA, req.AFM);
            else if (req.AMKA != null)
                return NotFoundAmka(req.AMKA);
            else return NotFoundAfm(req.AFM);
        }
    }
}

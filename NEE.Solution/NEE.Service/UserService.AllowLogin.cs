using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Security;
using NEE.Service.Core;
using System;

namespace NEE.Service
{
    partial class UserService
    {
        public AllowLoginResponse AllowLogin(AllowLoginRequest req)
        {
            AllowLoginResponse response = new AllowLoginResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                if (req.Person == null)
                {
                    return AllowLoginResponse.Unauthorized("Το μητρώο του ΑΜΚΑ δεν επέστρεψε εγγραφή με τα συγκεκριμένα στοιχεία");
                }

                if (req.Person.AFM != req.ProvidedAfm)
                {
                    return AllowLoginResponse.Unauthorized("Ο ΑΜΚΑ που εισάγατε δεν αντιστοιχεί σε αυτόν που συσχετίζεται με το παραπάνω ΑΦΜ. Παρακαλούμε απευθυνθείτε στα ΚΕΠ προκειμένου να επικαιροποιήσετε τη διασύνδεση του ΑΜΚΑ σας με το ΑΦΜ σας", UnauthorizedReason.AfmGivenNotAfmReturned);
                }

                if (req.Person.DeathStatus.HasValue)
                {
                    return AllowLoginResponse.Unauthorized("Υπάρχει ένδειξη θανάτου στο μητρώο του ΑΜΚΑ. Παρακαλούμε απευθυνθείτε σε ΚΕΠ για έλεγχο των στοιχείων. ", UnauthorizedReason.LoginAttemptByDead);
                }

                response.LoginAllowed = true;
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return response;
        }
    }

    public class AllowLoginRequest
    {
        public Person Person { get; set; }
        public string ProvidedAfm { get; set; }
    }

    public class AllowLoginResponse : NEEServiceResponseBase
    {
        public AllowLoginResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }

        public static AllowLoginResponse Unauthorized(string message, UnauthorizedReason reason = Service.UnauthorizedReason.Other)
        {
            var res = new AllowLoginResponse();
            res.AddError(ErrorCategory.Not_Found, $"{message}");
            res.UnauthorizedReason = reason;
            res.UnauthorizedMessage = message;
            return res;
        }

        public bool LoginAllowed { get; set; } = false;
        public UnauthorizedReason? UnauthorizedReason { get; set; }
        public string UnauthorizedMessage { get; set; }
    }

    public enum UnauthorizedReason
    {
        Other = 0,
        AfmGivenNotAfmReturned = 1,
        LoginAttemptByDead = 2,
    }
}
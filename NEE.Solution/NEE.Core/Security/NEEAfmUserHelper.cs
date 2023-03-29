using System.Security.Claims;
using System.Security.Principal;

namespace NEE.Core.Security
{
    public static class NEEAfmUserHelper
    {
        public static class AspNetConstants
        {
            public const string UserName = "_opeka_afm_user_";
            public const string Email = "opeka-afm-user@idika.gr";
            public const string Role = "AFM_Users";
        }

        public static class ClaimNames
        {
            public const string AFM = "kot:aade:afm";
            public const string IsGuest = "kot:aade:is-guest";
            public const string AMKA = "kot:amka:amka";
            public const string LastName = "kot:amka:last-name";
            public const string FirstName = "kot:amka:first-name";
        }

        public class AfmUserInfo
        {
            public string AFM { get; set; }
            public string AMKA { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public bool IsGuest { get; set; }

            public string FullName => $"{this.FirstName} {this.LastName}";

            public AfmUserInfo(string afm, string amka, string lastName, string firstName, bool isGuest)
            {
                this.AFM = afm;
                this.AMKA = amka;
                this.LastName = lastName;
                this.FirstName = firstName;
                this.IsGuest = isGuest;
            }

            public string UserName => $"{this.AFM}_{this.AMKA}";            // system version (for DB, keying, etc)

            public string UserDisplayName => $"{this.FullName}";   // display version (for UI, ...)            

            public override string ToString() => this.UserName;

        }

        public static bool RemoveAfmUserInfo(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var afmClaim = claimsIdentity.FindFirst(ClaimNames.AFM);
                var amkaClaim = claimsIdentity.FindFirst(ClaimNames.AMKA);
                var lastNameClaim = claimsIdentity.FindFirst(ClaimNames.LastName);
                var firstNameClaim = claimsIdentity.FindFirst(ClaimNames.FirstName);
                var isGuestClaim = claimsIdentity.FindFirst(ClaimNames.IsGuest);

                if ((afmClaim != null) || (amkaClaim != null) || (lastNameClaim != null) || (firstNameClaim != null) && (isGuestClaim != null))
                {
                    claimsIdentity.RemoveClaim(afmClaim);
                    claimsIdentity.RemoveClaim(amkaClaim);
                    claimsIdentity.RemoveClaim(lastNameClaim);
                    claimsIdentity.RemoveClaim(firstNameClaim);
                    claimsIdentity.RemoveClaim(isGuestClaim);
                    return true;
                }
            }
            return false;
        }

        public static bool AddAfmUserInfo(this IIdentity identity, AfmUserInfo info)
        {
            identity.RemoveAfmUserInfo();   // just in case...!!

            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var afmClaim = new Claim(ClaimNames.AFM, info.AFM);
                var amkaClaim = new Claim(ClaimNames.AMKA, info.AMKA);
                var lastNameClaim = new Claim(ClaimNames.LastName, info.LastName);
                var firstNameClaim = new Claim(ClaimNames.FirstName, info.FirstName);
                var isGuestClaim = new Claim(ClaimNames.IsGuest, info.IsGuest.ToString());

                claimsIdentity.AddClaim(afmClaim);
                claimsIdentity.AddClaim(amkaClaim);
                claimsIdentity.AddClaim(lastNameClaim);
                claimsIdentity.AddClaim(firstNameClaim);
                claimsIdentity.AddClaim(isGuestClaim);
            }

            return false;
        }

        public static AfmUserInfo GetAfmUserInfo(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var afmClaim = claimsIdentity.FindFirst(ClaimNames.AFM);
                var amkaClaim = claimsIdentity.FindFirst(ClaimNames.AMKA);
                var lastNameClaim = claimsIdentity.FindFirst(ClaimNames.LastName);
                var firstNameClaim = claimsIdentity.FindFirst(ClaimNames.FirstName);
                var isGuestClaim = claimsIdentity.FindFirst(ClaimNames.IsGuest);

                if ((afmClaim != null) && (amkaClaim != null) && (lastNameClaim != null) && (firstNameClaim != null) && (isGuestClaim != null))
                {
                    var ret = new AfmUserInfo(afmClaim.Value, amkaClaim.Value, lastNameClaim.Value, firstNameClaim.Value, isGuestClaim.Value.ToLowerInvariant() == "true");
                    return ret;
                }
            }
            return null;
        }

        public static bool IsAfmUser(this IIdentity identity) => (identity.GetAfmUserInfo() != null);
    }
}

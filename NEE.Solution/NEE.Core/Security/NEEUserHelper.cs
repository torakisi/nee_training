using NEE.Core.Security;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace NEE.Core.Security
{
    public static class NEEUserHelper
    {
        public enum NEEUserType
        {
            None,
            Normal,
            Afm
        }


        public static NEEUserType GetNEEUserType(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return NEEUserType.None;
            if (identity.IsAfmUser()) return NEEUserType.Afm;
            return NEEUserType.Normal;
        }

        public static string GetNEEUserName(this IIdentity identity)
        {
            switch (identity.GetNEEUserType())
            {
                case NEEUserType.None: return null;
                case NEEUserType.Normal: return $"std:{identity.Name}";
                case NEEUserType.Afm: return identity.GetAfmUserInfo().UserName;
                default: throw new NotImplementedException();
            }
        }

        public static string GetNEEUserDisplayName(this IIdentity identity)
        {
            switch (identity.GetNEEUserType())
            {
                case NEEUserType.None: return null;
                case NEEUserType.Normal: return identity.Name;
                //case OpekaUserType.Kep: return identity.GetKepUserInfo().UserDisplayName;
                case NEEUserType.Afm: return identity.GetAfmUserInfo().UserDisplayName;
                default: throw new NotImplementedException();
            }
        }

        public static string GetNEEUserTooltip(this IIdentity identity)
        {
            switch (identity.GetNEEUserType())
            {
                case NEEUserType.None: return null;
                case NEEUserType.Normal: return null;
                //case OpekaUserType.Kep: return null;
                case NEEUserType.Afm:
                    var info = identity.GetAfmUserInfo();
                    var fullName = info.FullName;
                    var ret = $"Χρήστης: {info.FullName}\r\nΑΜΚΑ:     {info.AMKA}\r\nΑΦΜ:       {info.AFM}";
                    return ret;
                default: throw new NotImplementedException();
            }
        }

        public static bool IsSpecialUserName(string userName)
        {
            //if (string.Equals(userName, OpekaKepUserHelper.AspNetConstants.UserName, StringComparison.InvariantCultureIgnoreCase)) return true;
            if (string.Equals(userName, NEEAfmUserHelper.AspNetConstants.UserName, StringComparison.InvariantCultureIgnoreCase)) return true;
            return false;
        }


        public static bool IsEmailValid(string emailAddress)
        {
            try
            {
                return new EmailAddressAttribute().IsValid(emailAddress);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsSystemUserName(string userName)
        {
            try
            {
                return string.Equals(userName, "System_Audit", StringComparison.InvariantCultureIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}

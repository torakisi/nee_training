using NEE.Core.BO;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Security;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace NEE.Service.Authorization
{
    public class ServiceAuthorization
    {
        public static string NOT_AUTHORISED_MESSAGE = "Δεν έχετε εξουσιοδότηση για να πραγματοποιήσετε την συγκεκριμένη ενέργεια.";
        public static string NOT_AUTHORISED_CREATE_APPLICATION_MESSAGE = "Δεν έχετε εξουσιοδότηση για να δημιουργήσετε νέα αίτηση.";
        public static string MAINTAINANCE_SHUTDOWN = "Η πλατφόρμα δεν δέχεται ενέργειες αυτή τη στιγμή για λόγους συντήρησης.";

        #region Application Owner

        public bool GetApplicationOwnerAuthorized(INEECurrentUserContext currentUserContext, GetApplicationOwnerRequest request)
        {
            return true;
        }

        #endregion

        #region Application Services

        public static bool CreateApplicationAuthorized(INEECurrentUserContext currentUserContext, CreateApplicationRequest request)
        {
            // Χρήστες ΚΚ
            if (currentUserContext.IsNormalUser)
            {
                if (currentUserContext.IsReadOnlyUser)
                    return false;
                else if (currentUserContext.IsKKUser)
                    return true;
            }

            // Πολίτες
            if (currentUserContext.IsAFMUser)
            {
                var afmUserInfo = currentUserContext.User.Identity.GetAfmUserInfo();

                // Ο ΑΜΚΑ του request για δημιουργία νέου χρήστη δεν αντιστοιχεί με τον ΑΜΚΑ του συνδεδεμένου χρήστη
                if (afmUserInfo.AMKA != request.AMKA)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        public static bool AccessApplicationShouldBeOnlySystemAuditUsersAuthorized(INEECurrentUserContext currentUserContext, Application application)
        {
            // Χρήστες ΚΚ
            if (!currentUserContext.IsNormalUser)
            {
                return false;
            }

            if (currentUserContext.UserName != "System_Audit")
            {
                return false;
            }

            return true;
        }

        public static bool AccessApplicationShouldBeOnlyKKAuthorized(INEECurrentUserContext currentUserContext, Application application)
        {
            // Χρήστες ΚΚ
            if (!currentUserContext.IsNormalUser)
            {
                return false;
            }

            return true;
        }

        public static bool AccessApplicationShouldBeOnlyKKAndCertainRolesAuthorized(INEECurrentUserContext currentUserContext, List<string> roles, Application application = null)
        {
            // Χρήστες ΚΚ
            if (!currentUserContext.IsNormalUser)
            {
                return false;
            }

            var inRoles = false;
            foreach (string role in roles)
            {
                if (currentUserContext.IsInRole(role))
                    inRoles = true;
            }

            return inRoles;
        }

        public static bool AccessApplicationShouldBeApplicantAuthorized(INEECurrentUserContext currentUserContext, Application application)
        {
            // Χρήστες ΚΚ
            if (currentUserContext.IsNormalUser)
            {
                return true;
            }

            // Πολίτες
            if (currentUserContext.IsAFMUser)
            {
                var afmUserInfo = currentUserContext.User.Identity.GetAfmUserInfo();

                // Ο ΑΜΚΑ του request για δημιουργία νέου χρήστη δεν αντιστοιχεί με τον ΑΜΚΑ του συνδεδεμένου χρήστη
                if (afmUserInfo.AMKA != application.AMKA)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AccessApplicationShouldBeMemberAuthorized(INEECurrentUserContext currentUserContext, Application application)
        {
            // Χρήστες ΚΚ
            if (currentUserContext.IsNormalUser)
            {
                return true;
            }

            // Πολίτες
            if (currentUserContext.IsAFMUser)
            {
                var afmUserInfo = currentUserContext.User.Identity.GetAfmUserInfo();

                if (!application.Members
                    .Where(x => x.MemberState != MemberState.Deleted)
                    .Where(x => x.AMKA == afmUserInfo.AMKA)
                    .Any())
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AccessApplicationShouldBeForCertainStatesAuthorized(INEECurrentUserContext currentUserContext, Application application, List<AppState> permittedStates)
        {
            if (!permittedStates.Contains(application.State.Value))
            {
                return false;
            }

            return true;
        }

        public static bool AccessApplicationExcludingCertainStatesAuthorized(INEECurrentUserContext currentUserContext, Application application, List<AppState> excludedStates)
        {
            if (excludedStates.Contains(application.State.Value))
            {
                return false;
            }

            return true;
        }

        public static bool AccessUnavailableForMaintainanceAuthorized(INEECurrentUserContext currentUserContext)
        {
            bool maintainanceShutdown = ConfigurationManager.AppSettings["NewApplicationsClosed"] == "false" ? false : true;
            if (maintainanceShutdown)
            {
                return false;
            }

            return true;
        }
    }
}

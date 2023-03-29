using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;

namespace NEE.Service
{
    partial class UserService
    {
        public AuthorizeUserResponse AuthorizeUser()
        {
            AuthorizeUserResponse response = new AuthorizeUserResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                if (_currentUserContext.IsNormalUser)
                {
                    response.IsNormalUser = true;
                }
                if (_currentUserContext.IsKKUser)
                {
                    response.IsKKUser = true;
                }
                if (_currentUserContext.IsReadOnlyUser)
                {
                    response.IsReadOnlyUser = true;
                }
                response.IsUserAuthenticated = _currentUserContext.User.Identity.IsAuthenticated;
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return response;
        }
    }
    public class AuthorizeUserResponse : NEEServiceResponseBase
    {
        public AuthorizeUserResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }

        public bool IsNormalUser { get; set; } = false;
        public bool IsKKUser { get; set; } = false;
        public bool IsReadOnlyUser { get; set; } = false;
        public bool IsUserAuthenticated { get; set; }
    }
}

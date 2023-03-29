using NEE.Core.Contracts;
using NEE.Core.Security;
using NEE.Service.Core;

namespace NEE.Service
{
    public partial class UserService : NEEServiceBase
    {
        public UserService(
            IErrorLogger errorLogger,
            INEECurrentUserContext currentUserContext) : base(currentUserContext, errorLogger)
        {
            _errorLogger = errorLogger;

        }
    }
}
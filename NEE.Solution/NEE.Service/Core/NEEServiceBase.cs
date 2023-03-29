using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Security;
using System;
using System.Configuration;

namespace NEE.Service.Core
{
    public abstract class NEEServiceBase
    {
        public bool IsAuditingEnabled = ConfigurationManager.AppSettings["EnableAuditing"] == "false" ? false : true;

        public ServiceContext ServiceContext { get; set; } = new ServiceContext();
        public WebUIContext WebUIContext { get; set; } = new WebUIContext();

        protected INEECurrentUserContext _currentUserContext;
        protected IErrorLogger _errorLogger;

        public NEEServiceBase(INEECurrentUserContext currentUserContext,
            IErrorLogger errorLogger)
        {
            _currentUserContext = currentUserContext;
            _errorLogger = errorLogger;
        }

        public void LogError(ErrorLogSource source, string user, string exception, string applicationId = null)
        {
            _errorLogger.LogErrorAsync(new NEE.Core.BO.ErrorLog()
            {
                ApplicationId = applicationId,
                ErrorLogSource = source,
                Exception = exception,
                User = user,
                CreatedAt = DateTime.Now
            });
        }
    }
}

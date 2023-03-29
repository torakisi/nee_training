using NEE.Core.Contracts;
using NEE.Core.Security;
using NEE.Service.Core;

namespace NEE.Service
{
    public partial class SupportService : NEEServiceBase
    {
        private SupportRepository _supportRepository;
        public SupportService(SupportRepository supportRepository,
            IErrorLogger errorLogger,
            INEECurrentUserContext currentUserContext) : base(currentUserContext, errorLogger)
        {
            _supportRepository = supportRepository;
        }
    }
}

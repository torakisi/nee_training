using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Core.Security;
using NEE.Core;
using NEE.Service.Core;
using static NEE.Core.Security.NEEAfmUserHelper;
using XServices.Edto;

namespace NEE.Service
{
    public partial class PaymentService : NEEServiceBase
    {
        private readonly ApplicationRepository repository;
        private UserService _gsUserService;
        private KEDService _xsKEDService;
        private PersonService _personService;

        private INEEAppIdCreator _appIdGenerator;
        private readonly IClock clock;

        private AfmUserInfo UserInfo => _currentUserContext.User.Identity.GetAfmUserInfo();

        public PaymentService(
            ApplicationRepository repository,
            IErrorLogger errorLogger,
            UserService gsUserService,
            PersonService personService,
            KEDService kedService,
            INEEAppIdCreator appIdGenerator,
            INEECurrentUserContext currentUserContext,
            IClock clock) : base(currentUserContext, errorLogger)
        {
            this.repository = repository;
            _gsUserService = gsUserService;
            _xsKEDService = kedService;
            _personService = personService;
            _appIdGenerator = appIdGenerator;
            this.clock = clock;

        }       

        public PaymentService SetServiceContext(ServiceContext context)
        {
            ServiceContext = context;

            if (_personService != null)
                _personService.SetServiceContext(context);

            return this;
        }

        public PaymentService SetWebUIContext(WebUIContext context)
        {
            WebUIContext = context;

            if (_personService != null)
                _personService.SetWebUIContext(context);

            return this;
        }
    }
}

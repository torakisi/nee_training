using NEE.Core.Contracts;
using NEE.Core.Security;
using NEE.Core;
using NEE.Service.Core;
using NEE.Core.Helpers;
using System.Threading.Tasks;
using System;
using NEE.Core.BO;
using XServices.Iban;
using static NEE.Core.Security.NEEAfmUserHelper;
using XServices.Gsis;

namespace NEE.Service
{
    public partial class AppService : NEEServiceBase
    {
        private readonly ApplicationRepository repository;
        private UserService _gsUserService;

        private INEEAppIdCreator _appIdGenerator;
        private readonly IClock clock;
        private PersonService _personService;
        private IbanService _xsIbanService;

        private AfmUserInfo UserInfo => _currentUserContext.User.Identity.GetAfmUserInfo();

        public AppService(
            ApplicationRepository repository,
            IErrorLogger errorLogger,
            INEEAppIdCreator appIdGenerator,
            INEECurrentUserContext currentUserContext,
            PersonService personService,
            UserService gsUserService,
            IbanService xsIbanService,
            IClock clock) : base(currentUserContext, errorLogger)
        {
            this.repository = repository;
            _appIdGenerator = appIdGenerator;
            this.clock = clock;
            this._personService = personService;
            this._gsUserService = gsUserService;
            this._xsIbanService = xsIbanService;

            //LoadEvents();
        }

        public AppService SetServiceContext(ServiceContext context)
        {
            this.ServiceContext = context;

            if (this._personService != null)
                this._personService.SetServiceContext(context);

            return this;
        }

        public AppService SetWebUIContext(WebUIContext context)
        {
            this.WebUIContext = context;

            if (this._personService != null)
                this._personService.SetWebUIContext(context);

            return this;
        }

        private async Task CreateValidationRemarksForApplicationAsync(Application application)
        {
            if (Iban.IsValid(application.IBAN))
            {
                await ValidateIBAN(new ValidateIBANRequest()
                {
                    Application = application,
                    AFM = application.AFM,
                    IBAN = application.IBAN,
                    auditProtocol = application.Id,
                    auditUnit = "OPEKA",
                    auditUserId = _currentUserContext.UserName,
                    auditUserIp = application.RemoteHostIP
                });
            }
            else
            {
                application.IbanValidationResult = null;
            }

            application.CalculateBenefitAmount();

            var validationResp = ValidateApplication(new ValidateApplicationRequest()
            {
                Application = application
            });

            if (!validationResp._IsSuccessful)
            {
                throw new Exception("Παρουσιάστηκε σφάλμα κατά τον έλεγχο των στοιχείων της αίτησης");
            }
            application.CreateRemarks(validationResp.NewRemarks);
        }
    }
}

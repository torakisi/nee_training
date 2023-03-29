using NEE.Core.Contracts;
using NEE.Core.Security;
using NEE.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XServices.NotificationCenter;
using XServices.Edto;
using XServices.Efka;
using XServices.Ergani;
using XServices.Idika;
using XServices.Gsis;
using XServices.CitizenRegistry;

namespace NEE.Service
{
    public partial class PersonService : NEEServiceBase
    {
        private IDIKAService _xsIdikaService;
        private KEDService _xsKedService;
        private NotificationCenterService _xsNotificationCenterService;
        private ErganiService _xsErganiService;
        private EfkaService _xsEfkaService;
        private GsisPropertyService _gsisPropertyService;
        private GsisInfoService _gsisInfoService;
        private CitizenRegistryService _citizenRegistryService;

        public PersonService(
            IErrorLogger errorLogger,
            IDIKAService idikaService,
            KEDService kedService,
            NotificationCenterService notificationCenterService,
            ErganiService erganiService,
            EfkaService efkaService,
            GsisPropertyService gsisPropertyService,
            GsisInfoService gsisInfoService,
            CitizenRegistryService ctizenRegistryService,
            INEECurrentUserContext currentUserContext
            ) : base(currentUserContext, errorLogger)
        {
            _xsIdikaService = idikaService;
            _xsKedService = kedService;
            _xsNotificationCenterService = notificationCenterService;
            _xsErganiService = erganiService;
            _xsEfkaService = efkaService;
            _gsisPropertyService = gsisPropertyService;
            _gsisInfoService = gsisInfoService;
            _citizenRegistryService = ctizenRegistryService;
        }
        public PersonService SetServiceContext(ServiceContext context)
        {
            this.ServiceContext = context;
            return this;
        }

        public PersonService SetWebUIContext(WebUIContext context)
        {
            this.WebUIContext = context;
            return this;
        }
    }
}

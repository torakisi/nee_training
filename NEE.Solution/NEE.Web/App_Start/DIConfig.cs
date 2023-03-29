using NEE.Core;
using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Core.Security;
using NEE.Database;
using NEE.Service;
using NEE.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;
using XServices.Idika;
using XServices.Iban;
using XServices.Edto;
using XServices.NotificationCenter;
using XServices.Ergani;
using XServices.Efka;
using XServices.CitizenRegistry;
using XServices.Gsis;
using System.Data.Entity.Infrastructure;

namespace NEE.Web
{
    public class DIConfig
    {
        public static void Configure()
        {
            var container = ConfigureContainer();
            var resolver = new UnityDependencyResolver(container);
            var spy = new DependencyResolverSpy(resolver);
            DependencyResolver.SetResolver(spy);
        }


        private static IUnityContainer ConfigureContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<INEECurrentUserContext, NEECurrentUserContext>();
            container.RegisterType<INEEAppIdCreator, NEEAppDBIdHelper>();
            container.RegisterInstance<IClock>(new SystemClock());
            container.RegisterType<IErrorLogger, ErrorLogRepository>();

            Register(container, CreateDbContextFactory);
            Register(container, CreateIDIKAService);
            Register(container, CreateEDTOService);
            Register(container, CreateErganiService);
            Register(container, CreateCitizenRegistryService);
            Register(container, CreateGsisPropertyService);
            Register(container, CreateGsisInfoService);
            Register(container, CreateEfkaService);
            Register(container, CreateNotificationCenterService);
            Register(container, CreateAmkaServiceGateway);
            Register(container, CreateAmkaMaritalStatusGateway);
            Register(container, CreateIbanService);
            Register(container, CreateSupportRepository);

            VerifyContainer(container);

            return container;
        }

        private static NEEDbContextFactory CreateDbContextFactory(IUnityContainer c)
        {
            return new NEEDbContextFactory("Name=NEEDataConnection", "(auto)");
        }

        private static IDIKAService CreateIDIKAService(IUnityContainer c)
        {
            return new IDIKAService(
                c.Resolve<AmkaServiceGateway>(),
                c.Resolve<AmkaMaritalStatusGateway>(),
                c.Resolve<OtherBenefitsGateway>());
        }
        private static KEDService CreateEDTOService(IUnityContainer c)
        {
            return new KEDService(
                c.Resolve<NEEDbContextFactory>(),
                c.Resolve<INEECurrentUserContext>(),
                //"T00010332T01383V2PFM6WNDCCKKULKRP2S|h2AWuS1r5zb4w6wueocC@|https://test.gsis.gr/esbpilot/policePersonalDetailsEDTOService");
                "L00010332L01383H3DM36BXBHM9BC8DXBBM|ProtecTci9#|https://ked.gsis.gr/esb/policePersonalDetailsEDTOService");
        }
        private static ErganiService CreateErganiService(IUnityContainer c)
        {
            return new ErganiService(
                c.Resolve<NEEDbContextFactory>(),
                c.Resolve<INEECurrentUserContext>(),
                //"T00010332T01189CYMHUDBR2PN48UDWUHTH|GdqCxxUtpl2tYS7nhNMU@|https://test.gsis.gr/esbpilot/erganiService");
                "L00010332L01189HMLE8NA7KC6D2NNZYBAP|Ergan#i281!|https://ked.gsis.gr/esb/erganiService");
        }
        private static CitizenRegistryService CreateCitizenRegistryService(IUnityContainer c)
        {
            return new CitizenRegistryService(
                c.Resolve<NEEDbContextFactory>(),
                c.Resolve<INEECurrentUserContext>(),
                //"T00010332T01207THCD7NZDKWD42FC4E74S|Mcert1f#|https://test.gsis.gr/esbpilot/citizenRegistryService
                "L00010332L01207TWKCZ86XHXWKLZFPBTHX|Mcert2f#Pol|https://www1.gsis.gr/esb/citizenRegistryService");
        }
        private static GsisPropertyService CreateGsisPropertyService(IUnityContainer c)
        {
            return new GsisPropertyService(
                c.Resolve<NEEDbContextFactory>(),
                c.Resolve<INEECurrentUserContext>(),
                //"T00010332T01387E6USPV8BUK32DDZMRX8D|AKINproperE9$|https://test.gsis.gr/esbpilot/propertyValueInfoService");
                "L00010332L01387M5A82DKDEBE7UWF3FMRL|AKINproperE8$|https://ked.gsis.gr/esb/propertyValueInfoService");
        }
        private static GsisInfoService CreateGsisInfoService(IUnityContainer c)
        {
            return new GsisInfoService(
                c.Resolve<NEEDbContextFactory>(),
                c.Resolve<INEECurrentUserContext>(),
                //"T00010332T01355N6ULL5V79FDYNR38TS9B|IncomeValue!9|https://test.gsis.gr/esbpilot/taxSeniorHouseAssistInfoService");
                "L00010332L01355CZV8PWHLRZ8BNMT5N72C|IncomeValue!8|https://ked.gsis.gr/esb/taxSeniorHouseAssistInfoService");
        }
        private static EfkaService CreateEfkaService(IUnityContainer c)
        {
            return new EfkaService(
                c.Resolve<NEEDbContextFactory>(),
                c.Resolve<INEECurrentUserContext>(),
                //"T00010332T01302VXULPN6NCHRPX5ULYULH|EfkaPEN9#|https://test.gsis.gr/esbpilot/salaryEmpInfoService");
                "L00010332L01302HZX2MAHH734ALLBKEK6P|EfkaPEN8#|https://ked.gsis.gr/esb/salaryEmpInfoService");
        }
        private static NotificationCenterService CreateNotificationCenterService(IUnityContainer c)
        {
            return new NotificationCenterService(
                c.Resolve<NEEDbContextFactory>(),
                c.Resolve<INEECurrentUserContext>(),
                //"T00010332T01220MZYB7PCPSFKC4C6TKUYB|GHzUSJFOMsJBSiK48BSM*|https://test.gsis.gr/esbpilot/notificationCenterElementsService");
                "L00010332L01220CCMHFWKMAXZFVLBYSLBK|MP17@epik!|https://ked.gsis.gr/esb/notificationCenterElementsService");
        }
        private static IbanService CreateIbanService(IUnityContainer c)
        {
            return new IbanService("Idika.IbanValidator|uxL6{m]a#wybEZD5|https://www.idika.gov.gr/iban/api/");
        }

        private static AmkaServiceGateway CreateAmkaServiceGateway(IUnityContainer c)
        {
            AmkaWebServiceConnectionString cnStr = new AmkaWebServiceConnectionString("wsafmhb|Gdg43$#fdsBr45|https://www.idika.gov.gr/webservices/amka/AFM2DATA/Service.asmx");
            return new AmkaServiceGateway(cnStr);
        }

        private static AmkaMaritalStatusGateway CreateAmkaMaritalStatusGateway(IUnityContainer c)
        {
            return new AmkaMaritalStatusGateway(
                c.Resolve<NEEDbContextFactory>());
        }

        private static SupportRepository CreateSupportRepository(IUnityContainer c)
        {
            return new SupportRepository(
                c.Resolve<NEEDbContextFactory>(),
                c.Resolve<INEECurrentUserContext>(),
                c.Resolve<IClock>());
        }


        private static void Register<T>(IUnityContainer container, Func<IUnityContainer, T> factory)
        {
            container.RegisterType<T>(new InjectionFactory(c => factory(c)));
        }

        private static void VerifyContainer(UnityContainer container)
        {
            foreach (var registration in container.Registrations)
            {
                container.Resolve(registration.RegisteredType, registration.Name);
            }
        }

        class DependencyResolverSpy : IDependencyResolver
        {
            private readonly IDependencyResolver resolver;

            public DependencyResolverSpy(IDependencyResolver resolver)
            {
                this.resolver = resolver;
            }

            public object GetService(Type serviceType)
            {
                var res = resolver.GetService(serviceType);
                return res;
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                var res = resolver.GetServices(serviceType);
                return res;
            }
        }
    }
}
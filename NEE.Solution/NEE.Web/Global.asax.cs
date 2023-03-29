using Microsoft.Ajax.Utilities;
using NEE.Core.Contracts.Enumerations;
using NEE.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.UI.WebControls;

namespace NEE.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            DIConfig.Configure();
            InitZipCodes();
        }

        private async void InitZipCodes()
        {
            var appService = DependencyResolver.Current.GetService<AppService>();
            var ZipCodes = await appService.ZipCodesRetrieverAsync();
            var items = ZipCodes.Select(x => new Core.BO.ZipCode(x.Code, x.City, x.District)).ToList();
            Core.BO.ZipCodes.SetData(items);
        }
    }
}

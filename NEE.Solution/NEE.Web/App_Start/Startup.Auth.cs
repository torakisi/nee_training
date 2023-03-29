using NEE.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
//using Owin.Gsis;
using System;
using System.Configuration;
using System.Web.Helpers;
using Unity;

namespace NEE.Web
{
	public partial class Startup
	{
		// For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
            // Configure the db context, user manager and signin manager to use a single instance per request
            //app.CreatePerOwinContext(() =>new ApplicationDbContext.Create(ConfigurationManager.ConnectionStrings["ESTestDataConnection"].ConnectionString, "(auto)"));

            //app.CreatePerOwinContext(ApplicationDbContext.Create);	
            app.CreatePerOwinContext(() => UnityConfig.GetConfiguredContainer().Resolve<ApplicationDbContext>());
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
			app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
			app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

			AntiForgeryConfig.UniqueClaimTypeIdentifier = "unique_user_key";

			// Enable the application to use a cookie to store information for the signed in user
			// and to use a cookie to temporarily store information about a user logging in with a third party login provider
			// Configure the sign in cookie
			var enableAdminTag = ConfigurationManager.AppSettings["ApplicationForPublic"] == "false" ? true : false;
			if (enableAdminTag)
			{
				app.UseCookieAuthentication(new CookieAuthenticationOptions
				{
					AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
					LoginPath = new PathString("/AdminAccount/Login"),
					CookieName = ".AspNet.ApplicationCookie.NEE.gov",
					LogoutPath = new PathString("/AdminAccount/LogOff"),
					ExpireTimeSpan = TimeSpan.FromMinutes(180),
					SlidingExpiration = true
				});
			}
			else
			{
				app.UseCookieAuthentication(new CookieAuthenticationOptions
				{
					AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
					LoginPath = new PathString("/Account/Login"),
					CookieName = ".AspNet.ApplicationCookie.NEE.pub",
					LogoutPath = new PathString("/Account/LogOff"),
					ExpireTimeSpan = TimeSpan.FromMinutes(30),
					SlidingExpiration = true
				});
			}

			app.SetDefaultSignInAsAuthenticationType(DefaultAuthenticationTypes.ExternalCookie);
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ExternalCookie,
				AuthenticationMode = AuthenticationMode.Passive,
				CookieName = ".AspNet.ExternalCookie.NEE.pub",
				ExpireTimeSpan = TimeSpan.FromMinutes(5),
			});


			if (ConfigurationManager.AppSettings["ApplicationForPublic"] == "true")
			{
				app.Use((context, next) =>
			{
				context.Request.Scheme = ConfigurationManager.AppSettings["GsisAuthHttpSchema"];
				return next();
			});

				//app.UseGsisAuthentication(new GsisAuthenticationOptions()
				//{
				//	ClientId = ConfigurationManager.AppSettings["GsisAuthClientId"],
				//	ClientSecret = ConfigurationManager.AppSettings["GsisAuthClientSecret"],
				//	//EnableProxy = true,
				//	//ProxyUrl = "http://localhost:3128",
				//	CallbackPath = new PathString("/Account/LoginGsis"),
				//	UseProduction = ConfigurationManager.AppSettings["GsisAuthUseProduction"] == "false" ? false : true,
				//});
			}
			//IdentitySeed.EnsureUsersAndRoles();
		}
	}
}
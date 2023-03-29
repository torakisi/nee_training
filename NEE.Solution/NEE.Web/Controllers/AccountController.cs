using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NEE.Core.BO;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Core.Security;
using NEE.Service;
using NEE.Web.Code;
using NEE.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static NEE.Service.PersonService;
using NEE.Database.Entities;
using System.Net.Http;
using System.Security.Policy;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using System.Data.SqlTypes;
using System.Xml;
using NEE.Web.Models.Core;
using System.Net;

namespace NEE.Web.Controllers
{
    [Authorize]
    public class AccountController : NEEBaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IErrorLogger _errorLogger;

        private PersonService _personService;

        public AccountController(UserService gsUserService, PersonService personService, IErrorLogger errorLogger)
            : base(gsUserService)
        {
            if (NEE_Environment.IsAdminEnabled())
                throw new UnauthorizedAccessException();

            _personService = personService;
            _errorLogger = errorLogger;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/Authorize
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Authorize()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var authUrl = ConfigurationManager.AppSettings["GsisAuthorizationUrl"];
                    var clientId = ConfigurationManager.AppSettings["GsisAuthClientId"];
                    var redirectUri = ConfigurationManager.AppSettings["GsisAuthRedirectUri"];
                    var builder = new UriBuilder(authUrl);
                    builder.Port = -1;
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query["response_type"] = "code";
                    query["client_id"] = clientId;
                    query["redirect_uri"] = redirectUri;
                    builder.Query = query.ToString();
                    string url = builder.ToString();
                    return Redirect(url);
                }
            }
            catch (Exception ex)
            {
                var log1 = new ErrorLog()
                {
                    CreatedAt = DateTime.Now,
                    ErrorLogSource = ErrorLogSource.Service,
                    Exception = ex.ToString(),
                    User = "TEST",
                    StackTrace = "loginLog",
                    InnerException = ex.InnerException?.ToString()
                };
                _errorLogger.LogErrorAsync(log1);
                ViewBag.errorMessage = "Η Πιστοποίηση TaxisNet απέτυχε, παρακαλώ ξαναδοκιμάστε...";
                return View("Error");
            }            
        }

        // GET: /Account/GetToken
        [AllowAnonymous]
        public async Task<ActionResult> LoginGsis(string code)
        {
            using (var httpClient = new HttpClient())
            {
                var tokenUrl = ConfigurationManager.AppSettings["GsisAuthTokenUrl"];
                var clientId = ConfigurationManager.AppSettings["GsisAuthClientId"];
                var clientSecret = ConfigurationManager.AppSettings["GsisAuthClientSecret"];
                var redirectUri = ConfigurationManager.AppSettings["GsisAuthRedirectUri"];
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId), 
                    new KeyValuePair<string, string>("client_secret", clientSecret), 
                    new KeyValuePair<string, string>("code", code), 
                    new KeyValuePair<string, string>("scope", "read"), 
                    new KeyValuePair<string, string>("grant_type", "authorization_code"), 
                    new KeyValuePair<string, string>("redirect_uri", redirectUri), 
                });
                
                HttpResponseMessage response = await httpClient.PostAsync(tokenUrl, content);
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject resJson = JObject.Parse(responseBody);
                string token = resJson["access_token"].ToString();

                return RedirectToAction(nameof(GetUserInfo), new { token });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetUserInfo(string token)
        {
            var userInfoUrl = ConfigurationManager.AppSettings["GsisAuthUserInfoUrl"];
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, userInfoUrl);
            request.Headers.Add("Authorization", "Bearer" + token);
            var response = await client.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody == null)
            {
                ViewBag.errorMessage = "Η Πιστοποίηση TaxisNet απέτυχε, παρακαλώ ξαναδοκιμάστε...";
                return View("Error");

            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseBody);

            XmlNode userinfoNode = xmlDoc.SelectSingleNode("/root").SelectSingleNode("userinfo");
            string afm = userinfoNode.Attributes["taxid"].Value.Trim();

            if (!Afm.IsValid(afm))
            {
                ViewBag.errorMessage = "Η Πιστοποίηση TaxisNet απέτυχε, παρακαλώ ξαναδοκιμάστε...";
                return View("Error");
            }

            var identity = new ClaimsIdentity(
                    new[]
                    {
                    new Claim(ClaimTypes.AuthenticationMethod, "GSIS"),
                    new Claim("AFM", afm),
                    //new Claim("oauth_token", oauth_token),
                    new Claim(ClaimTypes.Name, afm),
                    new Claim(ClaimTypes.NameIdentifier, afm),
                    new Claim(System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier, afm)
                    },
                    DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(identity);
            return RedirectToAction("LoginAMKA");

        }
        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            
            string afm = null;
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null || loginInfo.ExternalIdentity == null)
            {
                ViewBag.errorMessage = "Η Πιστοποίηση TaxisNet απέτυχε, παρακαλώ ξαναδοκιμάστε...";
                return View("Error");
            }
            afm = loginInfo.ExternalIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;

            if (!Afm.IsValid(afm))
            {
                ViewBag.errorMessage = "Η Πιστοποίηση TaxisNet απέτυχε, παρακαλώ ξαναδοκιμάστε...";
                return View("Error");
            }

            var identity = new ClaimsIdentity(
                    new[]
                    {
                    new Claim(ClaimTypes.AuthenticationMethod, "GSIS"),
                    new Claim("AFM", afm),
                    //new Claim("oauth_token", oauth_token),
                    new Claim(ClaimTypes.Name, afm),
                    new Claim(ClaimTypes.NameIdentifier, afm),
                    new Claim(System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier, afm)
                    },
                    DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(identity);

            return RedirectToAction("LoginAMKA");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> LoginAMKA()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            var afm = await GetAfmFromExternalCookie();
            if (!Afm.IsValid(afm))
            {
                return new HttpUnauthorizedResult();
            }
            var m = new LoginAMKAViewModel
            {
                AFM = afm
            };

            return View(m);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAMKA(LoginAMKAViewModel m)
        {
            var afm = await GetAfmFromExternalCookie();
            if (!Afm.IsValid(afm))
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    m.AFM = afm;
                    if (await VerifyAfmAndSignIn(m, afm))
                    {
                        return RedirectToAction("RedirectToHome", "Home");
                    }
                }
                catch (Exception ex)
                {
                    // TODO defstat: Fix error
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(m);
        }

        private async Task<bool> VerifyAfmAndSignIn(LoginAMKAViewModel m, string afm)
        {
            afm = new Afm(afm).Value;
            var amka = new Amka(m.AMKA).Value;

            // Get Person Identity(find person in AMKA Registry)
            var reqIdentity = new GetAmkaRegistryRequest
            {
                AMKA = amka,
                AFM = afm
            };

            var resIdentity = await _personService.GetAmkaRegistryAsync(reqIdentity);
            if (!resIdentity._IsSuccessful)
            {
                foreach (var err in resIdentity._Errors)
                {
                    ModelState.AddModelError("", err.Message);
                }

                return false;
            }
            else
            {
                AllowLoginRequest reqAllowLogin = new AllowLoginRequest()
                {
                    Person = resIdentity.Person,
                    ProvidedAfm = afm
                };

                var responseAllowLogin = _gsUserService.AllowLogin(reqAllowLogin);

                if (!responseAllowLogin.LoginAllowed)
                {
                    ModelState.AddModelError("", responseAllowLogin.UnauthorizedMessage);
                    return false;
                }

            }



            var uniqueUserKey = $"{afm}_{amka}";
            var externalIdentity = await GetExternalIdentity();
            var oauth_token = externalIdentity?.FindFirst("oauth_token")?.Value;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.AuthenticationMethod, "GSIS"),
                new Claim(ClaimTypes.Name, uniqueUserKey),
                new Claim(ClaimTypes.NameIdentifier, uniqueUserKey),
                new Claim(System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier, uniqueUserKey)
            };

            // create new identity
            var claimsIdentity = new ClaimsIdentity(
                claims,
                DefaultAuthenticationTypes.ApplicationCookie);

            var afmUserInfo = new NEEAfmUserHelper.AfmUserInfo(resIdentity.Person.AFM, resIdentity.Person.AMKA, resIdentity.Person.LastName, resIdentity.Person.FirstName, false);
            claimsIdentity.AddAfmUserInfo(afmUserInfo);

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, claimsIdentity);

            return true;
        }

        private async Task<ClaimsIdentity> GetExternalIdentity()
        {
            var authenticateResult = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            return authenticateResult?.Identity;
        }
        private async Task<string> GetAfmFromExternalCookie()
        {
            var externalIdentity = await GetExternalIdentity();
            return externalIdentity?.FindFirst("AFM")?.Value;
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            string originalUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme);
            var logoutCallback = NEEHttpsRewriteHelper.RewriteUrl(originalUrl);
            // var logoutUrl = $"{originalUrl}{logoutCallback}";
            var clientId = ConfigurationManager.AppSettings["GsisAuthClientId"];
            if (ConfigurationManager.AppSettings["GsisAuthUseProduction"] == "true")
                return Redirect($"https://www1.gsis.gr/oauth2server/logout/{clientId}/?url={logoutCallback}");
            else if (ConfigurationManager.AppSettings["GsisAuthUseProduction"] == "false")
                return Redirect($"https://test.gsis.gr/oauth2server/logout/{clientId}/?url={logoutCallback}");

            return RedirectToAction("Login", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
                
    }
}
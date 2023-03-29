using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NEE.Web.Code;
using NEE.Web.Models;
using NEE.Web.Models.AdminAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NEE.Web.Controllers
{
    [Authorize]
    public class AdminAccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        // GET: AdminAccount

        public AdminAccountController()
        {
            if (!NEE_Environment.IsAdminEnabled())
                throw new UnauthorizedAccessException();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public static bool IsSpecialUser()
        {
            var specialUsers = new string[]
            {
                "torakisi@cbs.gr"
            };
            var user = System.Web.HttpContext.Current.User;
            return user.Identity.IsAuthenticated && specialUsers.Contains(user.Identity.Name);
            //return true;
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

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AdminLoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {

                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                if (!user.IsActive)
                {
                    ViewBag.errorMessage = "Ο λογαριασμός αυτός είναι απενεργοποιημένος!<br/>"
                                         + "Παρακαλώ επικοινωνήστε με το διαχειριστή του συστήματος για την ενεργοποίηση του.";

                    return View("Error");
                }

                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Επιβεβαιώστε το Λογαριασμό σας (επανάληψη) (Επίδομα Βορειοηπειρωτών)");

                    // Uncomment to debug locally
                    // ViewBag.Link = callbackUrl;

                    ViewBag.errorMessage = "Για να μπορέσετε να χρησιμοποιήσετε την εφαρμογή, πρέπει πρώτα να γίνει επιβεβαίωση του λογαριασμού σας.<br/>"
                                         + "Ένα επιπλέον email επιβεβαίωσης έχει αποσταλεί μόλις τώρα, παρακαλώ ελέγξτε τα εισερχόμενα email σας και ακολουθήστε τις οδηγίες.";

                    return View("Error");
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(user != null ? this.UserManager.IsInRole(user.Id, "NEEUsers") : false, returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.Clear();
                    ModelState.AddModelError("", "Μη επιτυχής προσπάθεια σύνδεσης");
                    return View(model);
            }
        }

        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new AdminVerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(AdminVerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(this.UserManager.IsInRole(this.User.Identity.GetUserId(), "NEEUsers"), model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Λανθασμένο token");
                    return View(model);
            }
        }

        // GET: /Account/Register
        //[AllowAnonymous]
        public ActionResult Register()
        {
            if (!IsSpecialUser()) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(AdminRegisterViewModel model)
        {
            if (!IsSpecialUser()) return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Επιβεβαιώστε το Λογαριασμό σας (Επίδομα Βορειοηπειρωτών)");

                    // Uncomment to debug locally
                    // ViewBag.Link = callbackUrl;

                    ViewBag.Message = "Παρακαλούμε ελέγξτε τα εισερχόμενα email σας, για email που σας ζητάει την επιβεβαίωση του λογαριασμού σας ώστε να μπορέσετε να χρησιμοποιήσετε την εφαρμογή.";

                    return View("Info");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            //var codeHtmlDecoded = HttpUtility.UrlDecode(code);
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                string resetPasswordCode = await UserManager.GeneratePasswordResetTokenAsync(userId);
                return RedirectToAction(nameof(ResetPassword), new { code = resetPasswordCode });
            }
            else
            {
                return View("Error");
            }
            //return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(AdminForgotPasswordViewModel model)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindByNameAsync(model.Email);
                    if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                    {
                        // Don't reveal that the user does not exist or is not confirmed
                        return View("ForgotPasswordConfirmation");
                    }

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "AdminAccount", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    callbackUrl = NEEHttpsRewriteHelper.RewriteUrl(callbackUrl);
                    await UserManager.SendEmailAsync(user.Id, "Επανάκτηση Κωδικού Πρόσβασης (Επίδομα Βορειοηπειρωτών)", "Παρακαλώ επαναφέρετε τον κωδικό πρόσβασης σας κάνοντας click <a href=\"" + callbackUrl + "\">εδώ</a>.");
                    return RedirectToAction("ForgotPasswordConfirmation", "AdminAccount");
                }

                // If we got this far, something failed, redisplay form
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(AdminResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "AdminAccount");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "AdminAccount");
            }
            AddErrors(result);
            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new AdminSendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /AdminAccount/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(AdminSendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        // POST: /AdminAccount/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "AdminAccount");
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

        private ActionResult RedirectToLocal(bool isKKUser, string returnUrl)
        {
            //if (Url.IsLocalUrl(returnUrl))
            //{
            //    return Redirect(returnUrl);
            //}+
            if (isKKUser)
                return RedirectToAction("Index", "AdminApplication");
            return RedirectToAction("About", "AdminAccount");
        }

        #endregion

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "AdminAccount", new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            callbackUrl = NEEHttpsRewriteHelper.RewriteUrl(callbackUrl);
            await UserManager.SendEmailAsync(userID, subject, "Παρακαλώ επιβεβαιώστε το λογαριασμό σας κάνοντας click <a href=\"" + callbackUrl + "\">εδώ</a>.");
            return callbackUrl;
        }
    }
}
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Validation;
using NEE.Service;
using NEE.Web.AuthorizeAttributes;
using NEE.Web.Code;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace NEE.Web.Controllers
{
    public class HomeController : NEEBaseController
    {
        private SupportService _gsSupportService;

        public HomeController(UserService gsUserService, SupportService gsSupportService)
            : base(gsUserService)
        {
            _gsSupportService = gsSupportService;
        }
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        [AllowAnonymous]
        public ActionResult FAQ()
        {
            ViewBag.Message = "Συχνές ερωτήσεις";


            return View();
        }

        [AllowAnonymous]
        public ActionResult Faqs()
        {
            ViewBag.Message = "Συχνές ερωτήσεις";

            var resp = _gsSupportService.GetFaqs(null);

            if (!resp._IsSuccessful)
            {
                ViewBag.errorMessage = ServiceErrorMessages.UIActionFailedMessage;
                ViewBag.errorDescription = resp.UIDisplayedErrorsFormatted;

                return View("Error");
            }

            return View(resp.Faqs);
        }

        [Authorize]
        [NEERole(Roles = "NEEUsers,ReadOnly")]
        public ActionResult AdminFAQ()
        {
            var resp = _gsSupportService.GetFaqs(null);

            if (!resp._IsSuccessful)
            {
                ViewBag.errorMessage = ServiceErrorMessages.UIActionFailedMessage;
                ViewBag.errorDescription = resp.UIDisplayedErrorsFormatted;

                return View("Error");
            }

            return View(resp.Faqs);
        }

        [AllowAnonymous]
        public ActionResult RedirectToHome()
        {

            AuthorizeUserResponse authResponse = _gsUserService.AuthorizeUser();

            if (authResponse._IsSuccessful)
            {
                if (authResponse.IsUserAuthenticated)
                {
                    if (authResponse.IsNormalUser)
                    {
                        return RedirectToAction("Index", "AdminApplication");
                    }
                    return RedirectToAction("ManageApplications", "Application");
                }
                else
                {
                    if (!NEE_Environment.IsAdminEnabled())
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "AdminApplication");
                    }
                }
            }

            return Error();
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Info()
        {

            return View();
        }
    }
}
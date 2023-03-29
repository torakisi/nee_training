using NEE.Core.Validation;
using NEE.Service;
using NEE.Web.AuthorizeAttributes;
using NEE.Web.Models.AdminApplicationViewModels;
using NEE.Web.Models.ApplicationViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static NEE.Service.PersonService;

namespace NEE.Web.Controllers
{
    [Authorize]
    [NEERole(Roles = "NEEUsers, OpekaNEEUsers")]
    public class AdminApplicationController : NEEBaseController
    {
        private AppService _gsAppService;
        private PersonService _personService;
        private SupportService _gsSupportService;

        public AdminApplicationController(AppService gsAppService, PersonService personService, UserService gsUserService, SupportService gsSupportService)
            : base(gsUserService)
        {
            _gsAppService = gsAppService;
            _personService = personService;
            _gsSupportService = gsSupportService;
        }

        // GET: AdminApplication

        [Authorize]
        [NEERoleAttribute(Roles = "NEEUsers, OpekaNEEUsers, ReadOnly")]
        public async Task<ActionResult> Index()
        {

            //var resp = _gsSupportService.GetAnnouncements(null);
            //if (!resp._IsSuccessful)
            //{
            //    ViewBag.errorMessage = ServiceErrorMessages.UIActionFailedMessage;
            //    ViewBag.errorDescription = resp.UIDisplayedErrorsFormatted;

            //    return View("Error");
            //}
            //return View(resp.Announcements);

            if (IsOpekaUser == true)
            {
                var opekaDistrict = await _gsAppService.GetOpekaDistrict();
                TempData["UserDistrict"] = opekaDistrict;
                return RedirectToAction("OpekaSearch", "Admin");
            }
            return View();
        }

        public bool IsOpekaUser
        {
            get
            {
                return IsInRole("OpekaNEEUsers");
            }
        }
        public bool IsInRole(string role) => HttpContext.GetOwinContext().Request.User.IsInRole(role);

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> Find(string amka, string afm, string fromReturn = "0")
        {
            FindViewModel myModel = new FindViewModel();
            if (fromReturn == "1")
            {
                try
                {
                    GetApplicationOwnerRequest req = new GetApplicationOwnerRequest()
                    {
                        AFM = afm,
                        AMKA = amka,
                        ShouldCheckForMinedu = true
                    };
                    myModel = await PerformFind(req, myModel);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(myModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Find(FindViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Amka) || string.IsNullOrEmpty(model.Afm))
                {
                    ModelState.AddModelError("", "Είναι απαραίτητη η εισαγωγή του ΑΜΚΑ και του ΑΦΜ για την αναζήτηση");
                }

                // clear results
                model.Results = new FindViewModel.FindResults();

                try
                {
                    GetApplicationOwnerRequest req = new GetApplicationOwnerRequest()
                    {
                        AFM = model.Afm,
                        AMKA = model.Amka,
                        ShouldCheckForMinedu = true
                    };
                    model = await PerformFind(req, model);
                }
                catch (Exception ex)
                {
                    // TODO: do something better here...
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        private async Task<FindViewModel> PerformFind(GetApplicationOwnerRequest req, FindViewModel m)
        {
            m.Results = new FindViewModel.FindResults();

            GetAmkaRegistryRequest pReq = new GetAmkaRegistryRequest()
            {
                AFM = req.AFM,
                AMKA = req.AMKA
            };

            GetAmkaRegistryResponse pResp = await _personService.GetAmkaRegistryAsync(pReq);
            if (!pResp._IsSuccessful)
            {
                throw new Exception(pResp._ErrorsFormatted);
            }

            //check if amka afm from user are the same from AMKA
            if (pResp.AmkaServiceResponse.AMKA != req.AMKA || pResp.AmkaServiceResponse.AFM != req.AFM)
            {
                throw new Exception("Ο ΑΜΚΑ/ΑΦΜ που εισάγατε δεν αντιστοιχούν με το Μητρώο του ΑΜΚΑ");
            }


            m.Results.AMKA = pResp.AmkaServiceResponse.AMKA;
            m.Results.AFM = pResp.AmkaServiceResponse.AFM;
            m.Results.LastName = pResp.AmkaServiceResponse.LastName;
            m.Results.FirstName = pResp.AmkaServiceResponse.FirstName;
            m.Results.LastNameEN = pResp.AmkaServiceResponse.LastNameEN;
            m.Results.FirstNameEN = pResp.AmkaServiceResponse.FirstNameEN;
            m.Results.Gender = pResp.AmkaServiceResponse.Gender;
            m.Results.DOB = pResp.AmkaServiceResponse.DOB;

            GetApplicationOwnerResponse resp = await _gsAppService.GetApplicationOwner(req);

            if (!resp._IsSuccessful)
            {
                throw new Exception(resp._ErrorsFormatted);
            }
            if (resp.ApplicationOwner.AFM != req.AFM)
            {
                throw new Exception("Δεν βρέθηκε");
            }

            ApplicationManagementViewModel model = new ApplicationManagementViewModel()
            {
                ApplicationOwner = resp.ApplicationOwner
            };

            m.Results.Applications = model;


            m.Afm = req.AFM;
            m.Amka = req.AMKA;
            return m;
        }
    }
}
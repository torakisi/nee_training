using NEE.Core.Contracts.Enumerations;
using NEE.Service;
using NEE.Service.Core;
using NEE.Web.AuthorizeAttributes;
using NEE.Web.Code;
using NEE.Web.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NEE.Web.Controllers
{
    [AdminEnabled]
    [NEERole(Roles = "NEEUsers, OpekaNEEUsers, ReadOnly")]
    public class AdminController: NEEBaseController
    {
        private AppService _gsAppService;

        public AdminController(AppService gsAppService, UserService gsUserService)
            : base(gsUserService)
        {
            _gsAppService = gsAppService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<SearchViewModel> PerformSearch(SearchApplicationsRequest req)
        {
            SearchViewModel res = new SearchViewModel
            {
                AFM = req.AFM,
                AMKA = req.AMKA,
                City = req.City,
                Email = req.Email,
                FirstName = req.FirstName,
                HomePhone = req.HomePhone,
                LastName = req.LastName,
                MobilePhone = req.MobilePhone,
                IBAN = req.IBAN,
                Zip = req.Zip,
                Id = req.Id,
                SearchInAppPerson = req.SearchInAppPerson,
                StateId = req.StateId,
                Results = new List<SearchViewModel.SearchResultTableViewModel>()
            };

            var resSearch = await _gsAppService.SearchApplicationAsync(req);
            //error sto view issuccessfull
            res.Results = resSearch.ApplicationSearchResults.MapList();

            res.Total = resSearch.Total;
            res.Skip = resSearch.Skip;
            res.Take = resSearch.Take;
            res.HasResults = true;
            return res;
        }

        [HttpGet]
        [ActionName("Search")]
        public async Task<ActionResult> SearchGet(string AppId,
                                                  string AMKA,
                                                  string AFM,
                                                  string IBAN,
                                                  string LastName,
                                                  string FirstName,
                                                  string City,
                                                  string Zip,
                                                  string Email,
                                                  string MobilePhone,
                                                  string HomePhone,
                                                  string SearchInPerson,
                                                  string SelState,
                                                  string isSearched = "0")
        {
            SearchViewModel m = new SearchViewModel();
            if (isSearched == "1")
            {
                try
                {

                    var reqSearch = new SearchApplicationsRequest
                    {
                        Id = AppId,
                        AMKA = AMKA,
                        AFM = AFM,
                        City = City,
                        Zip = Zip,
                        LastName = LastName,
                        FirstName = FirstName,
                        IBAN = IBAN,
                        Email = Email,
                        HomePhone = HomePhone,
                        MobilePhone = MobilePhone,
                        SearchInAppPerson = !string.IsNullOrEmpty(SearchInPerson) ? Convert.ToBoolean(SearchInPerson) : false,
                        StateId = Convert.ToInt32(SelState),
                        Skip = m.Skip,
                        Take = m.Take
                    };

                    m = await PerformSearch(reqSearch);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(m);
        }


        [HttpPost]
        [ActionName("Search")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchPost(SearchViewModel m, string command)
        {
            switch (command)
            {
                case "ClearFields":
                {
                    SearchViewModel model = ClearCriteria();
                    ModelState.Clear();
                    return View(model);
                }

                case "SearchApplications":
                {
                    if (ModelState.IsValid)
                    {

                        try
                        {
                            var reqSearch = new SearchApplicationsRequest
                            {
                                Id = m.Id,
                                AMKA = m.AMKA,
                                AFM = m.AFM,
                                City = m.City,
                                Zip = m.Zip,
                                LastName = m.LastName,
                                FirstName = m.FirstName,
                                IBAN = m.IBAN,
                                Email = m.Email,
                                HomePhone = m.HomePhone,
                                MobilePhone = m.MobilePhone,
                                SearchInAppPerson = m.SearchInAppPerson,
                                StateId = m.StateId,
                                Skip = m.Skip,
                                Take = m.Take
                            };

                            m = await PerformSearch(reqSearch);

                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.Message);
                        }
                    }

                    return View(m);
                }

                default:
                    return View(m);
            }
        }

        [HttpGet]
        [NEERole(Roles = "OpekaNEEUsers")]
        [ActionName("OpekaSearch")]
        public async Task<ActionResult> OpekaSearchGet(
                                                  string SubmittedByKK,
                                                  string SelState,
                                                  string isSearched = "0")
        {
            SearchViewModel m = new SearchViewModel();
            ViewBag.UserDistrict = TempData["UserDistrict"];
            if (isSearched == "0")
            {
                try
                {
                    m.UserDistrictId = ViewBag.UserDistrict;                    
                    var reqSearch = new SearchApplicationsRequest
                    {
                        SubmittedByKK = false,
                        StateId = 6,
                        Skip = m.Skip,
                        Take = m.Take,
                        DistrictId = m.UserDistrictId ?? 0
                    };
                    m = await PerformSearch(reqSearch);
                    m.UserDistrictId = ViewBag.UserDistrict;
                    if (m.HasResults && m.Results.Count > 0)
                        m.CanEditResults = (m.UserDistrictId == (int)m.Results.FirstOrDefault().DistrictId);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(m);
        }

        [HttpPost]
        [NEERole(Roles = "OpekaNEEUsers")]
        [ActionName("OpekaSearch")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OpekaSearchPost(SearchViewModel m, string command)
        {
            if (command.Contains("SelectDistrict"))
            {
                //var commandParams = command.Split(':');
                command = "SelectDistrict";
                WebUIContext context = new WebUIContext()
                {
                    WebUIAction = WebUIAction.ChangeDistrict
                };
                await _gsAppService.SetWebUIContext(context).ChangeDistrictAsync(new ChangeDistrictRequest
                {
                    AppId = m.Id,
                    DistrictId = m.DistrictId
                });
            }

            switch (command)
            {
                case "ClearFields":
                    {
                        SearchViewModel model = ClearCriteria();
                        ModelState.Clear();
                        return View(model);
                    }

                case "OpekaSearchApplications":
                case "SelectDistrict":
                    {
                        if (ModelState.IsValid)
                        {
                            try
                            {
                                var reqSearch = new SearchApplicationsRequest
                                {
                                    SubmittedByKK = m.SubmittedByKK,
                                    StateId = m.StateId,
                                    DistrictId = m.DistrictIdFilter,
                                    Skip = m.Skip,
                                    Take = m.Take
                                };

                                m = await PerformSearch(reqSearch);
                                var opekaDistrict = await _gsAppService.GetOpekaDistrict();
                                m.UserDistrictId = opekaDistrict;
                                if (m.Results.Count > 0)
                                    m.CanEditResults = (m.UserDistrictId == (int)m.Results.FirstOrDefault().DistrictId);
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("", ex.Message);
                            }
                        }

                        return View(m);
                    }                
                default:
                    return View(m);
            }
        }

        public SearchViewModel ClearCriteria()
        {
            SearchViewModel res = new SearchViewModel
            {
                AFM = "",
                AMKA = "",
                City = "",
                Email = "",
                FirstName = "",
                HomePhone = "",
                LastName = "",
                MobilePhone = "",
                IBAN = "",
                Zip = "",
                Id = "",
                SearchInAppPerson = false,
                Results = new List<SearchViewModel.SearchResultTableViewModel>(),

                HasResults = false
            };
            return res;
        }
    }
}
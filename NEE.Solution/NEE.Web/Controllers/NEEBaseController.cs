using NEE.Core.Security;
using NEE.Service;
using NEE.Web.Code;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NEE.Web.Controllers
{
    public class NEEBaseController : Controller
    {
        protected UserService _gsUserService;

        public bool IsDBDev => NEE_Environment.IsDBDev();

        public NEEAfmUserHelper.AfmUserInfo UserInfo => User.Identity.GetAfmUserInfo();

        public NEEBaseController(UserService gsUserService)
        {
            _gsUserService = gsUserService;
        }

        protected ActionResult ModelStateIncludeAction(string actionName, object routeValues = null)
        {
            TempData["ModelState"] = ModelState;
            return RedirectToAction(actionName, routeValues);
        }

        protected Task<ActionResult> ModelStateIncludeActionAsync(string actionName, object routeValues = null) =>
            Task.FromResult(ModelStateIncludeAction(actionName, routeValues));

        protected void PRGApplyModelState()
        {
            var state = TempData["ModelState"] as ModelStateDictionary;
            ModelState.Merge(state);
        }
        protected ActionResult PRGAction(string actionName, object routeValues = null)
        {
            TempData["ModelState"] = ModelState;
            return RedirectToAction(actionName, routeValues);
        }

        protected Task<ActionResult> PRGActionAsync(string actionName, object routeValues = null) =>
            Task.FromResult(PRGAction(actionName, routeValues));

        public ActionResult Error(string message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
                ViewBag.errorMessage = message;
            return View("Error");
        }

        public Task<ActionResult> ErrorAsync() =>
            Task.FromResult(Error());

        public string GetModelStateErrorMessages()
        {
            var errorMessages = string.Join("<br />", ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage));
            return errorMessages;
        }

        protected bool IsNormalUser()
        {
            AuthorizeUserResponse authResponse = _gsUserService.AuthorizeUser();

            if (authResponse._IsSuccessful)
            {
                return authResponse.IsNormalUser;
            }
            return false;
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
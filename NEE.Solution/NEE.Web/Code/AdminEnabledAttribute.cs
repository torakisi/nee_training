using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NEE.Web.Code
{
    public class AdminEnabledAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!NEE_Environment.IsAdminEnabled())
                filterContext.Result =
                    new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"action", "Index"},
                        {"controller", "Error"},
                    });

            base.OnActionExecuting(filterContext);
        }
    }
}
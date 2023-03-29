using System.Web;
using System.Web.Mvc;

namespace NEE.Web.AuthorizeAttributes
{
    public class NEERoleAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            var roleParts = Roles.Split(',');
            var valid = false;
            foreach (var rolePart in roleParts)
            {
                if ((HttpContext.Current.User).IsInRole(rolePart.Trim()))
                {
                    valid = true;
                }
            }

            return valid;
        }
    }
}
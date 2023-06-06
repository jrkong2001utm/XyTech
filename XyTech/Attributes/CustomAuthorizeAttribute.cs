using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace XyTech.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Check if the session variable contains the user ID
            return httpContext.Session["u_username"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Redirect unauthenticated users to the login page
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                { "controller", "Login" },
                { "action", "Index" }
                }
            );
        }
    }

}
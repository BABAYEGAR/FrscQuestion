using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace FrscQuestion.Models.Encryption
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session.GetString("FrscQuestionLoggedInUser") == null &&
                filterContext.HttpContext.Session.GetString("FrscQuestionLoggedInUserId") == null)
            {
                filterContext.Result =
                    new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            {"controller", "Account"},
                            {"action", "Login"},
                            {"returnUrl", "sessionExpired"}
                        });
            }


            base.OnActionExecuting(filterContext);
        }
    }
}
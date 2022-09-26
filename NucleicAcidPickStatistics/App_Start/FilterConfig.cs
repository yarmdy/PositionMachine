using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZEHOU.PM.Command;

namespace NucleicAcidPickStatistics
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthFilter());
        }
    }

    public class AuthFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var needLogin = true;
            var controller = filterContext.Controller;
            var action = filterContext.ActionDescriptor;
            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Help")
            {
                needLogin = false;
                goto finish;
            }
            if (action.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Length > 0)
            {
                needLogin = false;
                goto finish;
            }
            if (action.GetCustomAttributes(typeof(AuthorizeAttribute), false).Length > 0)
            {
                needLogin = true;
                goto finish;
            }
            if (controller.GetType().GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Length > 0)
            {
                needLogin = false;
                goto finish;
            }
            if (controller.GetType().GetCustomAttributes(typeof(AuthorizeAttribute), false).Length > 0)
            {
                needLogin = true;
                goto finish;
            }
        //var isAjax = filterContext.HttpContext.Request.IsAjaxRequest();
        finish:

            var cookie = filterContext.HttpContext.User.Identity.Name;
            var userStr = cookie;
            if (needLogin && (userStr + "" == "" || userStr == null))
            {
                goto login;
            }
            if (userStr == null)
            {
                return;
            }
            
            return;
        login:

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var json = new JsonResult();
                json.Data = new { code = -2, msg = "您没有权限",ok=-2};
                json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = json;
                return;
            }
            filterContext.Result = new RedirectResult("~/login");
        }
    }
}

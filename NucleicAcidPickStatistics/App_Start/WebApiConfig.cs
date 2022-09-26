using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ZEHOU.PM.Command;

namespace NucleicAcidPickStatistics
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new ApiAuthFilter());
        }
    }

    public class ApiAuthFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var needLogin = true;
            var controller = actionContext.ControllerContext.Controller;
            var action = actionContext.ActionDescriptor;
            
            if (action.GetCustomAttributes<AllowAnonymousAttribute>(false).Count > 0)
            {
                needLogin = false;
                goto finish;
            }
            if (action.GetCustomAttributes<AuthorizeAttribute>(false).Count > 0)
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

            var cookie = HttpContext.Current.User.Identity.Name;
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
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(new { code = -2, msg = "您没有权限", ok = -2 }), Encoding.UTF8, "application/json");
            httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            actionContext.Response = httpResponseMessage;
        }
    }
}

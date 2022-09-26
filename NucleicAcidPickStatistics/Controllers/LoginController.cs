using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZEHOU.PM.DB.dbLabelInfo;
using ZEHOU.PM.Command;
using System.Web.Security;

namespace NucleicAcidPickStatistics.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            FormsAuthentication.SignOut();
            return View();
        }
        [HttpPost]
        public ActionResult Index(User model)
        {
            ViewBag.ERROR = null;
            ViewBag.User = model;
            if (string.IsNullOrWhiteSpace(model.LoginName))
            {
                ViewBag.ERROR = "请输入用户名";
                goto res;

            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ViewBag.ERROR = "请输入密码";
                goto res;

            }

            var loginBll = new ZEHOU.PM.Bll.Login();
            var password = Crypt.Md5(model.Password);
            var user = loginBll.GetUserByLoginName(model.LoginName);

            if (user?.Password != password)
            {
                ViewBag.ERROR = "用户名或密码错误";
                goto res;
            }

            FormsAuthentication.SetAuthCookie($"{user.ID}|{user.LoginName}|{user.TrueName}|{user.RoleID}", false);

            return Redirect("~/home");

        res:
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ZEHOU.PM.Command;
using System.Web.Security;

namespace NucleicAcidPickStatistics.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var userFull = this.GetUserFull();
            var user = userFull.user;
            var roles = userFull.roles;

            var funcList = roles.SelectMany(a => a.FunctionID.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Distinct().ToList();
            var funcModels = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,FunctionModel>>(System.IO.File.ReadAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Scripts/Functions.json")));
            var funcModelList=new List<FunctionModel>();
            if (funcList.Any(a => a.StartsWith("M1")))
            {
                var funcM100 = new FunctionModel { };
                funcM100.CopyFrom(funcModels.G("M100"));
                funcM100.children = new List<FunctionModel>();
                funcList.Where(a => a.StartsWith("M1") && a!="M100").ToList().ForEach(a => {
                    var child = new FunctionModel { };
                    child.CopyFrom(funcModels.G(a));
                    funcM100.children.Add(child);
                });
                funcModelList.Add(funcM100);
            }
            if (funcList.Any(a => a.StartsWith("M2")))
            {
                var funcM200 = new FunctionModel { };
                funcM200.CopyFrom(funcModels.G("M200"));
                funcM200.children = new List<FunctionModel>();
                funcList.Where(a => a.StartsWith("M2") && a != "M200").ToList().ForEach(a => {
                    var child = new FunctionModel { };
                    child.CopyFrom(funcModels.G(a));
                    funcM200.children.Add(child);
                });
                funcModelList.Add(funcM200);
            }


            ViewBag.User = user;
            ViewBag.Roles = roles;
            ViewBag.Menus = funcModelList;
            return View();
        }

        public ActionResult Main(string id) {
            
            if (id == null) {
                var tmp = this.GetUser();
                id = tmp.ID;
            }
            var bll = new ZEHOU.PM.Bll.Login();
            var userfull = bll.GetUserAllInfoByUserId(id);
            if (userfull.user == null)
            {
                return Content("用户不存在");
            }

            ViewBag.User = userfull.user;
            ViewBag.Depart = userfull.depart;
            ViewBag.Roles = userfull.roles;
            ViewBag.Post = userfull.post;
            ViewBag.RoleNames = string.Join(",", userfull.roles.Select(a => a.Name));
            return View();
        }
        [HttpGet]
        public ActionResult ChangePassword() {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string password,string old)
        {
            var bll = new ZEHOU.PM.Bll.Login();
            var user = bll.GetUserById(this.GetUser().ID);
            var oldmd5 = Crypt.Md5(old);
            if (oldmd5 != user.Password)
            {
                return Json(new { code=-1,msg="旧密码错误"},JsonRequestBehavior.AllowGet);
            }
            var newmd5 = Crypt.Md5(password);
            var ret = bll.EditUserPwd(user.ID,newmd5);
            if (ret <= 0)
            {
                return Json(new { code = -1, msg = "密码修改失败" }, JsonRequestBehavior.AllowGet);
            }
            FormsAuthentication.SignOut();
            return Json(new { code = 1, msg = "密码修改成功" }, JsonRequestBehavior.AllowGet);
        }
    }
}

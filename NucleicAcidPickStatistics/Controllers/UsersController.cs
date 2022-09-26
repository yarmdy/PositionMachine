using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZEHOU.PM.Command;

namespace NucleicAcidPickStatistics.Controllers
{
    public class UsersController : Controller
    {
        // GET: Roles
        public ActionResult Index()
        {
            GetUserOtherInfo();
            return View();
        }

        public ActionResult ListJson(string tname=null,int pageIndex=1,int pageSize = 10)
        {
            var bll = new ZEHOU.PM.Bll.Login();
            var bll2 = new ZEHOU.PM.Bll.NAP();
            int count = 0;
            var res = bll.GetUsers(out count, pageIndex, pageSize,null,null, tname).Select(a => {
                var obj = new ZEHOU.PM.DB.dbLabelInfo.User { };
                obj.CopyFrom(a);
                obj.Department = null;
                obj.LRs = null;
                obj.Post = null;
                return obj;
            }).ToList();
            var pointsCount = bll2.GetUsersPointsCount(res.Select(a=>a.ID).ToArray());
            return Json(new { Rows=res,Total=count,PointsCount=pointsCount});
        }
        [HttpGet]
        public ActionResult Edit(string id=null) {
            GetUserOtherInfo();
            var bll = new ZEHOU.PM.Bll.Login();
            var user = bll.GetUserById(id);
            if (user == null)
            {
                user = new ZEHOU.PM.DB.dbLabelInfo.User { IsUse = true };
            }
            ViewBag.Model = user;
            return View();
        }
        [HttpPost]
        public ActionResult Edit(ZEHOU.PM.DB.dbLabelInfo.User model,bool isNew)
        {
            GetUserOtherInfo();
            if (model == null) {
                goto finish;
            }
            if (model.Tel == null)
            {
                model.Tel = "";
            }
            if (model.Note == null)
            {
                model.Note = "";
            }
            if (string.IsNullOrEmpty(model.ID))
            {
                ViewBag.ERROR = "请输入编号";
                goto finish;
            }
            if (string.IsNullOrEmpty(model.LoginName))
            {
                ViewBag.ERROR = "请输入登录名称";
                goto finish;
            }
            if (string.IsNullOrEmpty(model.TrueName))
            {
                ViewBag.ERROR = "请输入真实姓名";
                goto finish;
            }
            var bll = new ZEHOU.PM.Bll.Login();
            var existsUser = bll.GetUserById(model.ID);
            var existsUser2 = bll.GetUserByLoginName(model.LoginName);
            if (isNew) {
                if (existsUser != null) {
                    ViewBag.ERROR = "用户编号已存在";
                    goto finish;
                }
                if (existsUser2 != null)
                {
                    ViewBag.ERROR = "登录名已存在";
                    goto finish;
                }
            }
            var ret = 0;
            if (isNew)
            {
                model.Password = Crypt.Md5("123456");
                ret = bll.AddUser(model);
            }
            else
            {
                ret = bll.EditUser(model);
            }
            if (ret <= 0)
            {
                ViewBag.ERROR = $"{(isNew ? "新增":"编辑")}用户失败";
                goto finish;
            }
            ViewBag.Succ = 1;
        finish:
            if (model == null)
            {
                model = new ZEHOU.PM.DB.dbLabelInfo.User { IsUse = true };
            }
            ViewBag.Model = model;
            ViewBag.IsNew = isNew;
            return View();
        }

        private void GetUserOtherInfo() {
            var bll = new ZEHOU.PM.Bll.Login();
            ViewBag.Roles = bll.GetRoles().ToDictionary(a=>a.ID);
            var departs = bll.GetDepartments();
            departs.ForEach(a => {
                a.Users = null;
            });
            ViewBag.Departs = departs.ToDictionary(a => a.ID, b => {
                var item = new ZEHOU.PM.DB.dbLabelInfo.Department { };
                item.CopyFrom(b);
                return item;
            });
            var posts = bll.GetPosts();
            posts.ForEach(a => {
                a.Users = null;
            });
            ViewBag.Posts = posts.ToDictionary(a => a.ID, b => {
                var item = new ZEHOU.PM.DB.dbLabelInfo.Post { };
                item.CopyFrom(b);
                return item;
            });
        }
        public ActionResult Del(string id)
        {
            var bll = new ZEHOU.PM.Bll.Login();
            var ret = bll.DelUser(id);
            return Json(new { code = ret, msg = $"删除{(ret > 0 ? "成功" : "失败")}" });
        }

        [HttpGet]
        public ActionResult Funcs(string id = null)
        {
            var bll = new ZEHOU.PM.Bll.Login();
            var role = bll.GetRoleById(id);
            if (role == null)
            {
                return Content("角色不存在");
            }
            ViewBag.Model = role;
            ViewBag.Funcs = role.FunctionID.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return View();
        }
        [HttpPost]
        public ActionResult SetFuncs(string roleId,string[] funcs)
        {
            var bll = new ZEHOU.PM.Bll.Login();
            var role = bll.GetRoleById(roleId);
            if (role == null)
            {
                return Json(new { code = -1, ok = -1, msg = "设置失败，角色不存在" }, JsonRequestBehavior.AllowGet);
            }
            role.FunctionID = string.Join(",",funcs);
            var ret = bll.EditRoleFunc(role);
            return Json(new { code = 1, ok = 1,msg=$"设置{(ret > 0 ? "成功" : "失败")}" },JsonRequestBehavior.AllowGet);
        }
    }
}
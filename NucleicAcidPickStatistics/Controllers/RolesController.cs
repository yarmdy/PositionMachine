using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NucleicAcidPickStatistics.Controllers
{
    public class RolesController : Controller
    {
        // GET: Roles
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListJson(string tname=null,int pageIndex=1,int pageSize = 10)
        {
            var bll = new ZEHOU.PM.Bll.Login();
            int count = 0;
            var res = bll.GetRoles(out count,pageIndex,pageSize,tname);

            return Json(new { Rows=res,Total=count});
        }
        [HttpGet]
        public ActionResult Edit(string id=null) {
            var bll = new ZEHOU.PM.Bll.Login();
            var role = bll.GetRoleById(id);
            if (role == null)
            {
                role = new ZEHOU.PM.DB.dbLabelInfo.Role { IsUse = true };
            }
            ViewBag.Model = role;
            return View();
        }
        [HttpPost]
        public ActionResult Edit(ZEHOU.PM.DB.dbLabelInfo.Role model, bool isNew)
        {
            if (model == null) {
                goto finish;
            }
            model.FunctionID = "";
            if (string.IsNullOrEmpty(model.ID))
            {
                ViewBag.ERROR = "请输入编号";
                goto finish;
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                ViewBag.ERROR = "请输入角色名称";
                goto finish;
            }
            var bll = new ZEHOU.PM.Bll.Login();
            var existsRole = bll.GetRoleById(model.ID);
            if(isNew && existsRole != null)
            {
                ViewBag.ERROR = "角色编号已存在";
                goto finish;
            }
            var ret = 0;
            if (isNew)
            {
                ret = bll.AddRole(model);
            }
            else
            {
                ret = bll.EditRole(model);
            }
            if (ret <= 0)
            {
                ViewBag.ERROR = $"{(isNew ? "新增":"编辑")}角色失败";
                goto finish;
            }
            ViewBag.Succ = 1;
        finish:
            if (model == null)
            {
                model = new ZEHOU.PM.DB.dbLabelInfo.Role { IsUse = true };
            }
            ViewBag.Model = model;
            ViewBag.IsNew = isNew;
            return View();
        }
        public ActionResult Del(string id)
        {
            var bll = new ZEHOU.PM.Bll.Login();
            var ret = bll.DelRole(id);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NucleicAcidPickStatistics.Controllers
{
    public class PickPointsController : Controller
    {
        // GET: Roles
        public ActionResult Index(string userId=null,bool nohas=false)
        {
            ViewBag.UserId = userId;
            ViewBag.NoHas = nohas;
            return View();
        }

        public ActionResult ListJson(string tname=null,int pageIndex=1,int pageSize = 10,string userId = null, bool nohas = false)
        {
            var bll = new ZEHOU.PM.Bll.NAP();
            int count = 0;
            var res = bll.GetPickPoints(out count,pageIndex,pageSize,tname,userId,nohas);

            return Json(new { Rows=res,Total=count});
        }
        [HttpGet]
        public ActionResult Edit(int? id=null) {
            var bll = new ZEHOU.PM.Bll.NAP();
            var pickpoint = bll.GetPickPointById(id??0);
            if (pickpoint == null)
            {
                pickpoint = new ZEHOU.PM.DB.dbLabelInfo.NAP_PickPoints {  };
            }
            ViewBag.Model = pickpoint;
            return View();
        }
        [HttpPost]
        public ActionResult Edit(ZEHOU.PM.DB.dbLabelInfo.NAP_PickPoints model, bool isNew)
        {
            if (model == null) {
                goto finish;
            }
            //if (model.ID<=0)
            //{
            //    ViewBag.ERROR = "请输入编号，编号为数字不能小于0";
            //    goto finish;
            //}
            if (string.IsNullOrEmpty(model.Name))
            {
                ViewBag.ERROR = "请输入名称";
                goto finish;
            }
            var bll = new ZEHOU.PM.Bll.NAP();
            
            var ret = 0;
            if (isNew)
            {
                model.ID= 0;
                ret = bll.AddPickPoint(model);
            }
            else
            {
                
                ret = bll.EditPickPoint(model);
            }
            if (ret <= 0)
            {
                ViewBag.ERROR = $"{(isNew ? "新增":"编辑")}失败";
                goto finish;
            }
            ViewBag.Succ = 1;
        finish:
            if (model == null)
            {
                model = new ZEHOU.PM.DB.dbLabelInfo.NAP_PickPoints { };
            }
            ViewBag.Model = model;
            ViewBag.IsNew = isNew;
            return View();
        }
        public ActionResult Del(int id)
        {
            var bll = new ZEHOU.PM.Bll.NAP();
            var ret = bll.DelPickPoint(id);
            return Json(new { code = ret, msg = $"删除{(ret > 0 ? "成功" : "失败")}" });
        }

        public ActionResult Append(string userId,int id)
        {
            var bll = new ZEHOU.PM.Bll.NAP();
            var ret = bll.AddPickPointToUser(userId,id);
            return Json(new { code = ret, msg = $"加入{(ret > 0 ? "成功" : "失败")}" });
        }
        public ActionResult Remove(string userId, int id)
        {
            var bll = new ZEHOU.PM.Bll.NAP();
            var ret = bll.RemoveUserPickPoint(userId, id);
            return Json(new { code = ret, msg = $"移除{(ret > 0 ? "成功" : "失败")}" });
        }


    }
}
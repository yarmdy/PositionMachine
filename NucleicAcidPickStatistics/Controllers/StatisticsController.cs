using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZEHOU.PM.Command;

namespace NucleicAcidPickStatistics.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TubeInfo()
        {
            return View();
        }

        public ActionResult TubeList(int pageIndex, int pageSize,string tname = null, int? pickType = null, DateTime? labelDate = null ) {
            var bll = new ZEHOU.PM.Bll.NAP();
            var bll2 = new ZEHOU.PM.Bll.Login();
            var user = bll2.GetUserAllInfoByUserId(this.GetUser().ID);

            var isAdmin = user.roles.SelectMany(a=>a.FunctionID.Split(',')).Distinct().Contains("M202");

            var res = bll.GetTubePickInfo( pageIndex, pageSize, tname, pickType, labelDate==null?DateTime.Now:labelDate.Value,isAdmin?null:user.user.ID );


            return Json(new { Rows = res.list.Select(a => {
                var temp = new ZEHOU.PM.DB.dbLabelInfo.User();
                temp.CopyFrom(a.user);
                temp.Department = null;
                temp.LRs = null;
                temp.Post = null;
                return new { a.tube, a.record, a.pickpoint, user = temp, a.picknum };
                }).ToArray(), Total = res.total.count,data = new { res.total.count, res.total.pickcount } });
        }
        public ActionResult PeopleList(int pageIndex, int pageSize, string tname = null, int? pickType = null, DateTime? labelDate = null, int? tubeId = null)
        {
            var bll = new ZEHOU.PM.Bll.NAP();
            var bll2 = new ZEHOU.PM.Bll.Login();
            var user = bll2.GetUserAllInfoByUserId(this.GetUser().ID);

            var isAdmin = user.roles.SelectMany(a => a.FunctionID.Split(',')).Distinct().Contains("M202");
            var res = bll.GetPeoplePickInfo(pageIndex, pageSize, tname, pickType, labelDate == null ? DateTime.Now : labelDate.Value,tubeId, isAdmin ? null : user.user.ID);


            return Json(new
            {
                Rows = res.list.Select(a => {
                    
                    return new { a.person,a.record, a.tube , a.pickpoint };
                }).ToArray(),
                Total = res.total.count,
                data = new { res.total.count, res.total.pickcount }
            });
        }

        public ActionResult PeopleInfo(int? tubeId=null,DateTime? pickDate=null)
        {
            ViewBag.TubeId = tubeId;
            ViewBag.PickDate = pickDate;
            return View();
        }
    }
}
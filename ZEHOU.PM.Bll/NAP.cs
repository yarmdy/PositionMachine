using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZEHOU.PM.DB.dbLabelInfo;
using ZEHOU.PM.Command;

namespace ZEHOU.PM.Bll
{
    public class NAP
    {
        public (List<(NAP_PickRecords record, NAP_Tubes tube, NAP_PickPoints pickpoint, User user,int picknum)> list,(int count,int pickcount) total) GetTubePickInfo(int pageIndex, int pageSize, string keyword = null, int? pickType = null, DateTime? labelDate = null,string userId=null)
        {
            (List<(NAP_PickRecords record, NAP_Tubes tube, NAP_PickPoints pickpoint, User user, int picknum)> list, (int count, int pickcount) total) res = (new List<(NAP_PickRecords record, NAP_Tubes tube, NAP_PickPoints pickpoint, User user, int picknum)>(),(count:0, pickcount:0));
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var queryt = db.NAP_Tubes.Where(a=>true);
                    if (userId != null)
                    {
                        queryt = db.NAP_Tubes.Join(db.NAP_UserPickPoints.Where(a => a.UserId == userId), a => a.PPId, b => b.PPId, (a, b) => a);
                    }
                    var query = queryt.GroupJoin(db.NAP_PickRecords, a => a.ID, b => b.TubeId, (a, b) => new { tube = a, record = b })
                        .GroupJoin(db.NAP_PickPoints,a=>a.tube.PPId,b=>b.ID,(a,b)=>new { tube =a.tube, record =a.record,pickpoint = b})
                        .GroupJoin(db.Users, a => a.tube.UserId, b => b.ID, (a, b) => new { tube = a.tube, record = a.record, pickpoint=a.pickpoint,user=b }).Where(a=>a.tube.LabelStatus==1);

                    
                        
                    if (labelDate.HasValue) {
                        var startTime = labelDate.Value.Date;
                        var endTime = labelDate.Value.Date.AddDays(1);
                        query = query.Where(a=>a.tube.LabelTime>=startTime && a.tube.LabelTime<endTime);
                    }
                    if (pickType.HasValue) {
                        if (pickType == 0)
                        {
                            query = query.Where(a => a.record.Count() <= 0);
                        }
                        else {
                            query = query.Where(a => a.record.Count() > 0);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(keyword)) { 
                        query = query.Where(a => a.tube.BarCode.Contains(keyword));
                    }

                    var count = query.Count();
                    var pickcount = query.Count(a=>a.record.Count()>0);
                    var list = query.OrderByDescending(a=>a.tube.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(a=>new { tube = a.tube, record = a.record.OrderBy(b=>b.PickDate).FirstOrDefault(), pickpoint = a.pickpoint.FirstOrDefault(), user = a.user.FirstOrDefault(), picknum = a.record.Count()}).ToList().Select(a=>(record:a.record,tube:a.tube,pickpoint:a.pickpoint,user:a.user,picknum:a.picknum)).ToList();
                    res.list = list;
                    res.total = (count:count, pickcount:pickcount);
                    //res.list.ForEach(a => {
                    //    var temp = new User();
                    //    temp.CopyFrom(a.user);
                    //    temp.Department = null;
                    //    temp.LRs= null;
                    //    temp.Post = null;
                    //    a.user = temp;
                    //});
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【NAP.GetTubePickInfo】获取试管使用情况失败", ex);
            }
            return res;
        }

        public (List<(NAP_People person, NAP_PickRecords record, NAP_Tubes tube, NAP_PickPoints pickpoint)> list, (int count, int pickcount) total) GetPeoplePickInfo(int pageIndex, int pageSize, string keyword = null, int? pickType = null, DateTime? labelDate = null,int? tubeId=null, string userId = null)
        {
            (List<(NAP_People person, NAP_PickRecords record, NAP_Tubes tube, NAP_PickPoints pickpoint)> list, (int count, int pickcount) total) res = (new List<(NAP_People person, NAP_PickRecords record, NAP_Tubes tube, NAP_PickPoints pickpoint)>(), (count: 0, pickcount: 0));
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var queryt = db.NAP_People.GroupJoin(db.NAP_PickRecords, a => a.ID, b => b.PersonId, (a, b) => new { person = a, record = b.DefaultIfEmpty() });
                    if (labelDate.HasValue)
                    {
                        var pickDate = labelDate.Value.Date;
                        queryt = db.NAP_People.GroupJoin(db.NAP_PickRecords, a => new { PersonId = a.ID,PickDate=pickDate}, b => new { PersonId = b.PersonId, PickDate=b.PickDate }, (a, b) => new { person = a, record = b.DefaultIfEmpty() });
                    }
                    var queryt2 = queryt.SelectMany(a => a.record.Select(b => new { a.person, record = b }))
                        .GroupJoin(db.NAP_Tubes, a => a.record.TubeId, b => b.ID, (a, b) => new { a.person, a.record, tube = b.FirstOrDefault() });

                    if (userId != null)
                    {
                        queryt2 = queryt2.Join(db.NAP_UserPickPoints.Where(a => a.UserId == userId), a => a.tube.PPId, b => b.PPId, (a, b) => a);
                    }

                    var query = queryt2
                        .GroupJoin(db.NAP_PickPoints, a => a.tube.PPId, b => b.ID, (a, b) => new { a.person, a.tube, a.record, pickpoint = b.FirstOrDefault() }).Where(a => true);

                    //if (labelDate.HasValue)
                    //{
                    //    var pickDate = labelDate.Value.Date;
                    //    query = query.Where(a => a.record == null || a.record.PickDate == pickDate);
                    //}
                    if (pickType.HasValue)
                    {
                        if (pickType == 0)
                        {
                            query = query.Where(a => a.record==null);
                        }
                        else
                        {
                            query = query.Where(a => a.record!=null);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        query = query.Where(a => a.person.Name.Contains(keyword) || a.person.ID.Contains(keyword));
                    }
                    if (tubeId != null)
                    {
                        query = query.Where(a=>a.tube.ID==tubeId);
                    }
                    var count = query.Count();
                    var pickcount = query.Count(a => a.record!=null);
                    var list = query.OrderBy(a => a.person.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(a => new { a.person, a.record, a.tube, a.pickpoint }).ToList().Select(a=>(person:a.person, record:a.record, tube:a.tube, pickpoint:a.pickpoint)).ToList();
                    res.list = list;
                    res.total = (count: count, pickcount: pickcount);
                    //res.list.ForEach(a => {
                    //    var temp = new User();
                    //    temp.CopyFrom(a.user);
                    //    temp.Department = null;
                    //    temp.LRs= null;
                    //    temp.Post = null;
                    //    a.user = temp;
                    //});
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【NAP.GetPeoplePickInfo】获取试管使用情况失败", ex);
            }
            return res;
        }

        #region 采样点
        public NAP_PickPoints GetPickPointById(int id)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.NAP_PickPoints.Find(id);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【NAP.GetPickPointById】按ID获取采样点失败", ex);
                return null;
            }

        }
        public List<NAP_PickPoints> GetPickPoints(out int count, int page, int size, string keyword = null, string userId = null, bool nohas = false)
        {
            count = 0;
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var query = db.NAP_PickPoints.Where(a => true);
                    if (!string.IsNullOrEmpty(userId)&&!nohas)
                    {
                        query = query.Join(db.NAP_UserPickPoints.Where(a=>a.UserId==userId), a => a.ID, b => b.PPId, (a, b) => new { point=a,userid=b.UserId}).Where(a=>a.userid==userId).Select(a=>a.point);
                    }else if (!string.IsNullOrEmpty(userId) && nohas)
                    {
                        query = query.GroupJoin(db.NAP_UserPickPoints.Where(a=>a.UserId==userId), a => a.ID, b => b.PPId, (a, b) => new { point = a, user = b.FirstOrDefault() }).Where(a => a.user == null).Select(a => a.point);
                    }
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query = query.Where(a => a.Name.Contains(keyword));
                    }
                    count = query.Count();
                    return query.OrderBy(a => a.ID).Skip((page - 1) * size).Take(size).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【NAP.GetPickPoints】获取采样点列表失败", ex);
                return new List<NAP_PickPoints>();
            }

        }

        public int AddPickPoint(NAP_PickPoints pickpoint)
        {
            var tmppickpoint = GetPickPointById(pickpoint.ID);
            if (tmppickpoint != null)
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    pickpoint.CreateTime = DateTime.Now;
                    db.NAP_PickPoints.Add(pickpoint);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【NAP.AddPickPoint】添加采样点失败", ex);
                return -99;
            }

        }

        public int EditPickPoint(NAP_PickPoints pickpoint)
        {
            if (pickpoint.ID <= 0)
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    db.NAP_PickPoints.Attach(pickpoint);
                    pickpoint.UpdateTime = DateTime.Now;
                    db.Entry(pickpoint).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(pickpoint).Property(a => a.CreateTime).IsModified = false;

                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【NAP.EditPickPoint】修改采样点失败", ex);
                return -99;
            }

        }

        public int DelPickPoint(int id)
        {
            if (id <= 0)
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var pickpoint = new NAP_PickPoints { ID = id };
                    db.NAP_PickPoints.Attach(pickpoint);
                    db.Entry(pickpoint).State = System.Data.Entity.EntityState.Deleted;

                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【NAP.DelPickPoint】删除采样点失败", ex);
                return -99;
            }

        }

        public NAP_UserPickPoints GetUserPickPoint(string userId, int id) {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.NAP_UserPickPoints.Find(userId,id);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【NAP.GetUserPickPoint】获取用户的采样点失败", ex);
                return null;
            }
        }
        public int AddPickPointToUser(string userId,int id)
        {
            var tmppickpoint = GetUserPickPoint(userId,id);
            if (tmppickpoint != null)
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var userPoint = new NAP_UserPickPoints { UserId=userId,PPId=id,CreateTime=DateTime.Now};
                    db.NAP_UserPickPoints.Add(userPoint);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【NAP.AddPickPointToUser】添加采样点失败", ex);
                return -99;
            }

        }

        public int RemoveUserPickPoint(string userId, int id)
        {
            if (id <= 0)
            {
                return -1;
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    var userpickpoint = new NAP_UserPickPoints { UserId = userId,PPId=id };
                    db.NAP_UserPickPoints.Attach(userpickpoint);
                    db.Entry(userpickpoint).State = System.Data.Entity.EntityState.Deleted;

                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【NAP.RemoveUserPickPoint】删除用户采样点失败", ex);
                return -99;
            }

        }

        public Dictionary<string, int> GetUsersPointsCount(string[] userIds)
        {
            if (userIds == null || userIds.Length <= 0)
            {
                return new Dictionary<string, int>();
            }
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.NAP_UserPickPoints.GroupBy(a => a.UserId).Select(a => new { userid = a.Key, count = a.Count() }).ToDictionary(a=>a.userid,b=>b.count);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【NAP.GetUsersPointsCount】获取用户采样点数量失败", ex);
                return new Dictionary<string, int>();
            }

        }

        #endregion
    }
}

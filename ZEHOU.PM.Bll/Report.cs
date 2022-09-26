using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZEHOU.PM.DB.dbLabelInfo;

namespace ZEHOU.PM.Bll
{
    public class Report
    {
        public List<LR> GetLabelRecordList(string deviceId=null, DateTime? startTime = null,DateTime? endTime=null, string barCode=null, string patientID=null,string patientName = null) {
            try
            {
                using (var db = new dbLabelInfoEntities { })
                {
                    var query = db.LRs.Where(a => true);
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        query = query.Where(a => a.DeviceID == deviceId);
                    }
                    if (startTime != null)
                    {
                        query = query.Where(a => a.CreateTime >= startTime);
                    }
                    if (endTime != null)
                    {
                        var tmpdate = endTime.Value.Date.AddDays(1);
                        query = query.Where(a => a.CreateTime < tmpdate);
                    }
                    if (!string.IsNullOrEmpty(barCode))
                    {
                        query = query.Where(a => a.BarCode.Contains(barCode));
                    }
                    if (!string.IsNullOrEmpty(patientID))
                    {
                        query = query.Where(a => a.PatientID.Contains(barCode));
                    }
                    if (!string.IsNullOrEmpty(patientName))
                    {
                        query = query.Where(a => a.PatientName.Contains(patientName));
                    }
                    return query.OrderByDescending(a => a.BarCode).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Report.GetLabelRecordList】查询贴标记录失败", ex);
                return new List<LR>();
            }
        }

        public List<(User user, int tubeNum, int patientNum)> GetNurseStatistics(DateTime? startTime = null, DateTime? endTime = null, string id = null) {
            try
            {
                using (var db = new dbLabelInfoEntities { })
                {
                    var query = db.Users.Where(a => true);
                    if (!string.IsNullOrEmpty(id))
                    {
                        query = query.Where(a => a.ID == id);
                    }

                    var queryLr = db.LRs.Where(a => true);
                    if (startTime != null)
                    {
                        queryLr = queryLr.Where(a => a.CreateTime >= startTime);
                    }
                    if (endTime != null)
                    {
                        var tmpdate = endTime.Value.Date.AddDays(1);
                        queryLr = queryLr.Where(a => a.CreateTime < tmpdate);
                    }
                    if (!string.IsNullOrEmpty(id))
                    {
                        queryLr = queryLr.Where(a => a.UserID == id);
                    }
                    var resQuery = query.GroupJoin(queryLr, a => a.ID, b => b.UserID, (a, b) => new { user = a, tubeNum = b.Count(), patientNum = b.Select(c => c.PatientID).Distinct().Count() }).OrderBy(a => a.user.ID).ToList();



                    return resQuery.Select(a => (a.user, a.tubeNum, a.patientNum)).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Report.GetNurseStatistics】查询抽血统计失败", ex);
                return new List<(User user, int tubeNum, int patientNum)>();
            }
            
        }

        public List<(string tubeColor, int tubeNum)> GetTubeStatistics(DateTime? startTime = null, DateTime? endTime = null, string id = null)
        {
            try
            {
                using (var db = new dbLabelInfoEntities { })
                {
                    var queryLr = db.LRs.Where(a => true);
                    if (startTime != null)
                    {
                        queryLr = queryLr.Where(a => a.CreateTime >= startTime);
                    }
                    if (endTime != null)
                    {
                        var tmpdate = endTime.Value.Date.AddDays(1);
                        queryLr = queryLr.Where(a => a.CreateTime < tmpdate);
                    }
                    if (!string.IsNullOrEmpty(id))
                    {
                        queryLr = queryLr.Where(a => a.UserID == id);
                    }
                    var resQuery = queryLr.GroupBy(a => a.TubeColor).Select(a => new { tubeColor = a.Key, tubeNum = a.Count() }).ToList();

                    return resQuery.Select(a => (a.tubeColor, a.tubeNum)).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Report.GetTubeStatistics】查询试管类型统计失败", ex);
                return new List<(string tubeColor, int tubeNum)>();
            }
            
        }
        /// <summary>
        /// 添加贴标记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddOrEditLr(LR model) {
            try
            {
                using (var db = new dbLabelInfoEntities { })
                {
                    var model2 = db.LRs.Find(model.BarCode);
                    if (model2 == null)
                    {
                        model.UpdateTime = DateTime.Now;
                        model.PrintCount = 1;
                        db.LRs.Add(model);
                    }
                    else
                    {
                        db.Entry(model2).State = System.Data.Entity.EntityState.Detached;
                        model.UpdateTime = DateTime.Now;
                        model.PrintCount = model2.PrintCount + 1;
                        db.LRs.Attach(model);
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    }
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Report.AddOrEditLr】添加贴标记录失败", ex);
                return -99;
            }
        }
    }
}

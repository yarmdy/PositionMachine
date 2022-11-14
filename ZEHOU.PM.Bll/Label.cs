using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZEHOU.PM.DB.dbLabelMid;

namespace ZEHOU.PM.Bll
{
    public class Label
    {
        public Patient GetPatientById(string id) {
            try
            {
                using (var db = new dbLabelMidEntities())
                {
                    return db.Patients.Find(id);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Label.GetPatientById】获取患者信息失败", ex);
                return null;
            }
            
        }

        public int AddOrEditOperateRequest(OperateRequest model) {
            try
            {
                using (var db = new dbLabelMidEntities())
                {
                    var model2 = db.OperateRequests.Find(model.ID);
                    if (model2 == null)
                    {
                        db.OperateRequests.Add(model);
                    }
                    else
                    {
                        db.Entry(model2).State = System.Data.Entity.EntityState.Detached;
                        db.OperateRequests.Attach(model);
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    }
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Label.AddOrEditOperateRequest】请求贴标失败", ex);
                return -99;
            }
            
        }

        public List<DB.dbLabelMid.Label> GetPatientLabels(string deviceId, string patientId) {
            try
            {
                using (var db = new dbLabelMidEntities())
                {
                    return db.Labels.Where(a => a.DeviceID == deviceId && patientId == a.PatientID).OrderBy(a => a.BarCode).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Label.GetPatientLabels】获取患者试管失败", ex);
                return new List<DB.dbLabelMid.Label>();
            }
            
        }

        public async Task<OperateRequest> GetOperateRequestStillSuccAsync(string id) {
            var time=DateTime.Now;
            var ret = await Task.Run(() => {
                try
                {
                    OperateRequest res = null;
                    using (var db = new dbLabelMidEntities())
                    {
                        while ((DateTime.Now - time).TotalSeconds < 10 && res == null)
                        {
                            res = db.OperateRequests.FirstOrDefault(a => a.ID == id && a.RequestFlag == 0);
                            System.Threading.Thread.Sleep(250);
                        }
                    }
                    return res;
                }
                catch (Exception ex)
                {
                    ExceptionTrigger.ProssException("【Label.GetOperateRequestStillSuccAsync】异步获取贴标请求失败", ex);
                    return null;
                }
                
            });
            return ret;
        }

        public DB.dbLabelMid.Label GetLastLabel(string deviceId,DateTime time)
        {
            try {
                using (var db = new dbLabelMidEntities())
                {
                    return db.Labels.Where(a => a.LabelStatus == 0 && a.DeviceID == deviceId && a.CreateTime >= time).OrderByDescending(a => a.CreateTime).FirstOrDefault();
                }
            } catch (Exception ex) {
                ExceptionTrigger.ProssException("【Label.GetLastLabel】获取最新贴标信息失败", ex);
                return null;
            }
        }

        public int EditLabelStatus(string barCode,int status)
        {
            try
            {
                using (var db = new dbLabelMidEntities())
                {
                    var label = new DB.dbLabelMid.Label {BarCode=barCode,LabelStatus=status };
                    db.Labels.Attach(label);
                    db.Entry(label).State=System.Data.Entity.EntityState.Unchanged;
                    db.Entry(label).Property(a => a.LabelStatus).IsModified = true;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Label.EditLabelStatus】修改贴标状态失败", ex);
                return -1;
            }
        }
    }
}

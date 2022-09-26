using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZEHOU.PM.DB.dbLabelInfo;

namespace ZEHOU.PM.Bll
{
    public class Device
    {
        #region 系统信息
        public List<DR> GetDevicesList()
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.DRs.OrderBy(m => m.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Device.GetDevicesList】获取设备列表失败", ex);
                return new List<DR>();
            }
        }

        public SystemParameter GetSystemParameterByDevId(string id)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.SystemParameters.FirstOrDefault(a => a.ID == id);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Device.GetSystemParameterByDevId】获取系统参数失败", ex);
                return null;
            }
        }

        public int EditSystemParameter(SystemParameter param)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    db.SystemParameters.Attach(param);
                    db.Entry(param).State = System.Data.Entity.EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Device.EditSystemParameter】编辑系统参数失败", ex);
                return -99;
            }
        }
        #endregion

        #region 试管类型
        public List<TubeType> GetTubeTypes(string deviceId)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.TubeTypes.Where(a => a.DeviceID == deviceId).OrderBy(a => a.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Device.GetTubeTypes】获取试管类型失败", ex);
                return new List<TubeType>();
            }
        }

        public TubeType GetTubeTypeById(string deviceId,string id)
        {
            try
            {
                using (var db = new dbLabelInfoEntities())
                {
                    return db.TubeTypes.Find(deviceId, id);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Device.GetTubeTypeById】按ID和设备ID获取试管类型失败", ex);
                return null;
            }
            
        }

        public int AddTubeType(TubeType tubeType)
        {
            return -99;
        }

        public int EditTubeType(TubeType tubeType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tubeType.ID))
                {
                    return -1;
                }
                using (var db = new dbLabelInfoEntities())
                {
                    db.TubeTypes.Attach(tubeType);
                    db.Entry(tubeType).State = System.Data.Entity.EntityState.Modified;

                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrigger.ProssException("【Device.EditTubeType】编辑试管类型失败", ex);
                return -99;
            }
        }

        #endregion

        #region 模板
        /// <summary>
        /// 获取贴标模板
        /// </summary>
        /// <param name="hosId">医院id</param>
        /// <param name="inOutId">0标准 1非标</param>
        /// <param name="labelType">0贴标 1回执单</param>
        /// <returns></returns>
        public List<PrintTemplate> GetPrintTemplatesByHosId(string hosId,string inOutId,string labelType)
        {
            try
            {
                using (var db = new dbLabelInfoEntities()) {
                    return db.PrintTemplates.Where(a => a.HospitalID == hosId && a.InOutID==inOutId && a.LabelTypeID== labelType).OrderBy(a => a.CommandID).ToList();
                }
            }
            catch (Exception ex) {
                ExceptionTrigger.ProssException("【Device.GetPrintTemplatesByHosId】获取打印模板失败", ex);
                return new List<PrintTemplate>();
            }
        }
        #endregion
    }
}

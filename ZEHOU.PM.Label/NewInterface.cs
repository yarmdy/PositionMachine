using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using ZEHOU.PM.Config;
using System.IO;

namespace ZEHOU.PM.Label
{
    public class NewInterface
    {
        private string addr = null;
        public NewInterface()
        {

        }
        public async Task<string> GetPatientAsync(GetLabelInfo labelInfo)
        {
            try {
                var buffer = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(labelInfo,Newtonsoft.Json.Formatting.Indented));
                var req = (HttpWebRequest)WebRequest.Create(Configs.Settings["InterfaceUrl"]);
                req.Method = "POST";
                req.Timeout = 1000 * 10;
                req.ContentType = "application/json";
                req.ContentLength = buffer.Length;
                req.KeepAlive = false;
                var reqStream = await req.GetRequestStreamAsync();
                reqStream.Write(buffer, 0, buffer.Length);
                var res = (HttpWebResponse)req.GetResponse();
                var resStream = res.GetResponseStream();
                StreamReader sr = new StreamReader(resStream);
                var resBody = sr.ReadToEnd();
                resStream.Dispose();
                reqStream.Dispose();
                sr.Dispose();
                res.Dispose();

                return resBody;
            } catch (Exception ex) {
                UILog.Error("从中间件获取数据失败！",ex);
            }
            return null;
        }
    }
    public class GetLabelInfo
    {
        /// <summary>病人ID</summary>
        public string PatientId { get; set; }
        /// <summary>病人类型</summary>
        public string PatientType { get; set; }
        /// <summary>设备ID</summary>
        public string DeviceId { get; set; }
        /// <summary>操作员ID</summary>
        public string UserId { get; set; }
        /// <summary>数据时间段范围_开始</summary>
        public DateTime StartTime { get; set; }
        /// <summary>数据时间段范围_结束</summary>
        public DateTime EndTime { get; set; }
    }
}
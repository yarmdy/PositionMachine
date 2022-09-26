using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZEHOU.PM.Label
{
    public static class UILog
    {
        public static void Info(string msg) {
            Global.Log.Info(msg);
            if (Global.BindingInfo == null) return;
            if (Global.BindingInfo.SysLog.Count > 1000) {
                Global.BindingInfo.SysLog.RemoveAt(0);
            }
            Global.BindingInfo.SysLog.Add($"{DateTime.Now.ToString("HH:mm:ss.fff")}：{msg}");
        }
        public static void Error(string msg,Exception ex)
        {
            Global.Log.Error(msg,ex);
            if (Global.BindingInfo == null) return;
            if (Global.BindingInfo.SysLog.Count > 1000)
            {
                Global.BindingInfo.SysLog.RemoveAt(0);
            }
            Global.BindingInfo.SysLog.Add($"{DateTime.Now.ToString("HH:mm:ss.fff")}：【系统异常】{msg}{(string.IsNullOrEmpty(ex?.Message)?"":$"\r\n{ex?.Message}")}");
        }
        public static void Send(string msg)
        {
            Global.SerialSendLog.Debug(msg);
            if (Global.BindingInfo == null) return;
            if (Global.BindingInfo.DebugLog.Count > 1000)
            {
                Global.BindingInfo.DebugLog.RemoveAt(0);
            }
            Global.BindingInfo.DebugLog.Add($"{DateTime.Now.ToString("HH:mm:ss.fff")}：【发送】{msg}");
        }
        public static void Receive(string msg)
        {
            Global.SerialReceiveLog.Debug(msg);
            if (Global.BindingInfo == null) return;
            if (Global.BindingInfo.DebugLog.Count > 1000)
            {
                Global.BindingInfo.DebugLog.RemoveAt(0);
            }
            Global.BindingInfo.DebugLog.Add($"{DateTime.Now.ToString("HH:mm:ss.fff")}：【接收】{msg}");
        }
    }
}

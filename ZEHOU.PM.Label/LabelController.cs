using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZEHOU.PM.Config;
using ZEHOU.PM.Command;
using System.Collections.ObjectModel;

namespace ZEHOU.PM.Label
{
    public class LabelController
    {
        private object _QueuesLocker = new object();

        /// <summary>
        /// 异步获取一个lis贴标信息
        /// </summary>
        /// <param name="barCode"></param>
        public async Task GetABarCode(string barCode) {
            Global.AutoLabel?.layerMsg("正在获取贴标数据");
            var labelBll = new Bll.Label();
            var backInfo = "";

            if (Configs.Settings["InterfaceUrl"] == "")
            {
                var reqo = new DB.dbLabelMid.OperateRequest { ID = barCode, DeviceID = Configs.Settings["DeviceID"], PatientID = barCode, RequestFlag = 1, UserID = Global.LocalUser.ID, PatientType = Configs.Settings["PatientType"], RequestTime = Global.BindingInfo.SysInfo.StartTime, CompleteTime = Global.BindingInfo.SysInfo.EndTime };

                UILog.Info($"【{barCode}】向中间件发起贴标请求");
                var ret = labelBll.AddOrEditOperateRequest(reqo);
                if (ret <= 0)
                {
                    backInfo = $"【{barCode}】向中间件发起贴标请求失败";
                    goto error;
                }
                UILog.Info($"【{barCode}】向中间件发起贴标请求成功，正在获取数据");

                var reqInfo = await labelBll.GetOperateRequestStillSuccAsync(barCode);

                if (reqInfo == null)
                {
                    backInfo = $"【{barCode}】贴标请求失败，中间件未处理";
                    goto error;
                }

                UILog.Info($"【{barCode}】中间件返回数据，正在获取患者信息");
            }
            else {
                var reqo = new GetLabelInfo {  DeviceId = Configs.Settings["DeviceID"], PatientId = barCode, UserId = Global.LocalUser.ID, PatientType = Configs.Settings["PatientType"], StartTime = Global.BindingInfo.SysInfo.StartTime, EndTime = Global.BindingInfo.SysInfo.EndTime };
                var newInfoerFace = new NewInterface();
                UILog.Info($"【{barCode}】向中间件发起贴标请求");
                var resStr = await newInfoerFace.GetPatientAsync(reqo);

                if (resStr == null)
                {
                    backInfo = $"【{barCode}】向中间件发起贴标请求失败";
                    goto error;
                }
                var jobj = Newtonsoft.Json.Linq.JObject.Parse(resStr);
                if (jobj["ReaultCode"]?.ToString() != "0")
                {
                    backInfo = $"【{barCode}】中间件返回错误\r\n错误信息："+ jobj["ResultMsg"];
                    goto error;
                }
                if (!jobj["Body"].HasValues || (jobj["Body"]["PatientId"]+"" == ""))
                {
                    backInfo = $"【{barCode}】中间件返回数据失败";
                    goto error;
                }
                barCode = jobj["Body"]["PatientId"] + "";
            }

            var patient = labelBll.GetPatientById(barCode);
            if (patient == null) {
                backInfo = $"【{barCode}】获取患者信息失败";
                goto error;
            }
            var labelList = labelBll.GetPatientLabels(Configs.Settings["DeviceID"],barCode);
            if (labelList.Count <= 0) {
                backInfo = $"【{barCode}】获取患者贴标信息失败";
                goto error;
            }
            UILog.Info($"【{barCode}】获取患者信息成功，开始匹配试管颜色");

            var deviceBll = new Bll.Device();

            var tubeType = deviceBll.GetTubeTypes(Configs.Settings["DeviceId"]).Where(a=>a.IsUse).ToList();

            Global.BindingInfo.LocalPatient.CopyFrom(patient);
            Global.BindingInfo.LocalLabelList.Clear();
            labelList.ForEach(a=> {
                var tinfo = new TubeInfoNotify();
                tinfo.CopyFrom(a);
                var patientL = new PatientInfoNotify();
                patientL.CopyFrom(Global.BindingInfo.LocalPatient);
                var info = new LabelInfoNotify {Patient= patientL,TubeInfo=tinfo,IsChecked=true };
                var ttype = tubeType.FirstOrDefault(b => b.HID.Split(new[] { ',' }).Contains(a.TubeColor));

                info.BinId = ttype?.BinID??"";
                if (string.IsNullOrWhiteSpace(info.BinId))
                {
                    info.TubeColor = Global.TubeColorDic.G(tubeType.FirstOrDefault(b=>b.ID=="00")?.Name ?? "");
                }
                else {
                    info.TubeColor = Global.TubeColorDic.G(ttype?.Name ?? "");
                }
                

                Global.BindingInfo.LocalLabelList.Add(info);
            });

            if (!Global.BindingInfo.SysInfo.AutoReadCard) {
                UILog.Info($"【{barCode}】颜色匹配完成，等待操作员下一步操作");
                goto finish;
            }
            AddToQueue();


        finish:
            Global.AutoLabel?.layerClose();
            return;
        error:
            UILog.Error(backInfo, null);
            Global.AutoLabel?.layerClose();
            UI.Popup.Error(Global.MainWindow,backInfo);
        }
        /// <summary>
        /// 添加到贴标列队
        /// </summary>
        public void AddToQueue(ObservableCollection<LabelInfoNotify> list=null,bool? onlyPrint=null) {
            var hasBackOrder = false;
            if (list == null) {
                list = Global.BindingInfo.LocalLabelList;
                hasBackOrder = Global.BindingInfo.SysInfo.AutoReadCard;
            }
            if (list.Count <= 0)
            {
                UILog.Info($"当前没有贴标信息,添加到贴标列队失败");
                UI.Popup.Error(Global.MainWindow, $"当前没有贴标信息,添加到贴标列队失败");
                return;
            }
            //var newOrder = false;
            //if (Global.BindingInfo.LabelQueue.Count <= 0)
            //{
            //    Global.BindingInfo.QueueId++;
            //    UILog.Info($"更新贴标清单号【{Global.BindingInfo.QueueId}】，开始加入贴标列队");
            //    newOrder = true;
            //}
            //else {
            //    UILog.Info($"现有贴标清单号【{Global.BindingInfo.QueueId}】，开始加入贴标列队");
            //}
            var printPc = new List<LabelInfoNotify>();
            var addCount = 0;
            foreach (var item in list) {
                if (Global.BindingInfo.LabelQueue.Contains(item) && item.TubeLabelStatus>0 || !item.IsChecked) {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(item.BinId) || onlyPrint.HasValue && onlyPrint.Value  || !onlyPrint.HasValue && !Global.BindingInfo.SysInfo.PrintAndLabel)
                {
                    item.OnlyPrint = true;
                }
                else {
                    item.OnlyPrint = false;
                }
                item.TubeLabelStatus = 0;
                if (!Global.IsMachineNonStandard && item.OnlyPrint) {
                    item.TubeLabelStatus = 10;
                    printPc.Add(item);
                    continue;
                }
                if (!Global.BindingInfo.LabelQueue.Contains(item))
                {
                    Global.BindingInfo.LabelQueue.Add(item);
                }
                addCount++;
            }
            if (printPc.Count > 0)
            {
                PrintNonStandardOnPC(printPc);
            }
            if (addCount <= 0 && printPc.Count<=0)
            {
                UILog.Info($"没有添加任何贴标记录，请勾选要贴标的项目");
                UI.Popup.Error(Global.MainWindow, $"没有添加任何贴标记录，请勾选要贴标的项目");
                return;
            }
            if (!hasBackOrder)
            {
                goto finish;
            }
            var backItem = new LabelInfoNotify { OnlyPrint = true, PrintBackOrder = true, TubeInfo = new TubeInfoNotify(), Patient = new PatientInfoNotify() };
            backItem.TubeInfo.CopyFrom(list.FirstOrDefault()?.TubeInfo);
            backItem.Patient.CopyFrom(list.FirstOrDefault()?.Patient);
            backItem.TubeInfo.TestOrder = "回执";


            if (!Global.IsMachineBackOrder && Global.BackOrderPrinter != null)
            {
                PrintBackOrderOnPC(new List<LabelInfoNotify> { backItem });
                goto finish;
            }
            if (Global.IsMachineBackOrder) { 

                addCount++;
                Global.BindingInfo.LabelQueue.Add(backItem);
            }

        finish:

            UILog.Info($"成功添加{addCount}条贴标记录");
            //if (!newOrder)
            //{
            //    return;
            //}
            SendLabelList();
        }
        /// <summary>
        /// 发送贴标清单
        /// </summary>
        public void SendLabelList() {
            lock (_QueuesLocker)
            {
                Global.BindingInfo.Queues.Where(a => a.Status == 0 || a.Status==255).ToList().ForEach(a=> Global.BindingInfo.Queues.Remove(a));
                
                var queueCount = Global.BindingInfo.Queues.Sum(a=>a.Nums);
                var listCount = Global.BindingInfo.LabelQueue.Count(a => a.TubeLabelStatus >= 0 && a.TubeLabelStatus < 10);                
                if (queueCount >= listCount)
                {
                    return;
                }

                var count = listCount - queueCount;
                var addCount = count / 255;
                var remainCount = count % 255;

                new int[addCount].ToList().ForEach(a => {
                    createQueue(255);
                });
                if (remainCount > 0)
                {
                    createQueue((byte)remainCount);
                }
                //if (Global.BindingInfo.SysInfo.SysStatus < 0 || Global.BindingInfo.LabelQueue.Count <= 0)
                //{
                //    return;
                //}
                //UILog.Info($"向下位机发送准备贴标清单");

                //var binNum = byte.Parse(Configs.Settings["BinNum"]);
                //var binId = 0;
                //var binLbNum = new byte[binNum].Select(a => {
                //    binId++;
                //    var num = Global.BindingInfo.LabelQueue.Count(b => b.BinId.Split(new[] { ',' }).Contains(binId + ""));
                //    return num > 255 ? (byte)255 : (byte)num;
                //}).ToArray();
                //Global.LPM.StartLabelList(Global.BindingInfo.QueueId, Global.BindingInfo.LabelQueue.Count > 255 ? (byte)255 : (byte)Global.BindingInfo.LabelQueue.Count, binNum, binLbNum);
                //SendLabelInfo();
            }
        }
        public void ResetQueue() { 
            
        }
        /// <summary>
        /// 从列队删除
        /// </summary>
        public void removeAPos() {
            lock (_QueuesLocker) {
                var queue = Global.BindingInfo.Queues.FirstOrDefault(a=>a.Status==2);
                if (queue == null)
                {
                    return;
                }
                queue.Nums--;
                if (queue.Nums <= 0)
                {
                    Global.BindingInfo.Queues.Remove(queue);
                }
            }
        }
        /// <summary>
        /// 创建清单
        /// </summary>
        /// <param name="remainCount"></param>
        private static void createQueue(byte remainCount)
        {
            var qid = Global.BindingInfo.QueueId++;
            var qinfo = new QueueInfo { AskTime = DateTime.Now, Nums = remainCount, Id = qid, Status = 0 };
            var commId = Global.LPM.StartLabelList(qinfo.Id, qinfo.Nums, 0, null);
            qinfo.CommId = commId;
            Global.BindingInfo.Queues.Add(qinfo);
        }


        /// <summary>
        /// 取消贴标清单
        /// </summary>
        public void CancelLabelList(byte? listNo=null) {
            lock (_QueuesLocker) {
                if (listNo != null) {
                    UILog.Info($"取消贴标清单【{listNo.Value}】");
                    Global.LPM.CancelLabelList(listNo.Value);
                }
                foreach (var queue in Global.BindingInfo.Queues)
                {
                    if (queue.Id == listNo)
                    {
                        continue;
                    }
                    if (queue.Status == 1 || queue.Status == 2 || queue.Status == 254 )
                    {
                        UILog.Info($"取消贴标清单【{queue.Id}】");
                        Global.LPM.CancelLabelList(queue.Id);
                    }
                    queue.Status = 255;
                }
                Global.BindingInfo.Queues.Clear();
            }
        }
        /// <summary>
        /// 发送贴标信息
        /// </summary>
        public void SendLabelInfo(byte? listno=null)
        {
            var queue = Global.BindingInfo.Queues.Where(a => a.Status == 1 || a.Status == 2).OrderByDescending(a => a.Status).FirstOrDefault();
            if (queue != null && queue.Status==1) { 
                queue.Status = 2;
            }
            //var labeling = Global.BindingInfo.LabelQueue.Count(a=>a.TubeLabelStatus>1);
            //if (labeling != null)
            //{
            //    return;
            //}
            var label = Global.BindingInfo.LabelQueue.FirstOrDefault(a => a.TubeLabelStatus >= 0 && a.TubeLabelStatus < 10);
            if (label == null|| Global.BindingInfo.SysInfo.SysStatus<0)
            {
                CancelLabelList(listno);
                return;
            }
            byte tmp;
            var binIds = label.BinId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(a=>byte.TryParse(a,out tmp)).Select(byte
                .Parse).ToArray();

            var printData = (byte[])null;
            if (!label.PrintBackOrder)
            {
                printData = GetPrintData(label.TubeInfo, label.OnlyPrint ? "1" : "0", label.TubeInfo.LableType + "");
            }
            else {
                printData = GetPrintBackOrderData(label.TubeInfo);
            }
            if (printData == null) {
                UILog.Error($"获取打印模板失败",null);
                label.TubeLabelStatus = -1;
                SendLabelInfo();
                return;
            }
            UILog.Info($"向下位机发送贴标命令");
            if (label.PrintBackOrder) {
                Global.LPM.StartBackOrder(label.TubeInfo.PatientID, printData);
            }
            else if (!label.OnlyPrint)
            {
                Global.LPM.StartLabel(binIds, label.TubeInfo.BarCode, printData);
            }
            else
            {
                Global.LPM.StartSpecialLabel(label.TubeInfo.BarCode, printData);
            }
            
            label.TubeLabelStatus = 1;
            label.SendTime = DateTime.Now;

            Global.BindingInfo.AlmostDoneLabel = Global.BindingInfo.LocalLabel;
            Global.BindingInfo.LocalLabel = label;
        }
        public byte[] GetPrintBackOrderData(TubeInfoNotify info) {
            var tmpTubeInfo = new TubeInfoNotify();
            tmpTubeInfo.CopyFrom(info);
            tmpTubeInfo.LS01 = info.PatientID;
            tmpTubeInfo.LS02 = info.TestOrder;
            tmpTubeInfo.LS05 = "回执";
            return GetPrintData(info,"1","1");
        }
        /// <summary>
        /// 获取打印数据
        /// </summary>
        /// <param name="info">贴标信息</param>
        /// <param name="printType">0标准 1非标</param>
        /// <param name="labelType">0贴标 1回执</param>
        /// <returns></returns>
        public byte[] GetPrintData(TubeInfoNotify info, string printType,string labelType) {
            var tempList = new Bll.Device().GetPrintTemplatesByHosId(Config.Configs.Settings["HospitalID"],printType, labelType);
            if (tempList.Count <= 0)
            {
                return null;
            }
            var paramArr = new string[] {
                "",
                info.LS01,
                info.LS02,
                info.LS03,
                info.LS04,
                info.LS05,
                info.LS06,
                info.LS07,
                info.LS08,
                info.LS09,
                info.LS10,
            };
            var bytes1D6B = new byte[] {0x1D,0x6B};
            var data = tempList.SelectMany(a =>{
                var command = stringData2Byte(a.CommandInfo);
                if (command == null) return new byte[0];
                var param = paramArr.G(a.ParameterID);
                var item = new List<byte>();
                var index = JSerialPort.SerialPortLib.IndexOfBytes(command, bytes1D6B, 0);
                if (index >= 0) {
                    command[index + 2 + 1] = (byte)(param.Length + 2);
                }
                item.AddRange(command);
                item.AddRange(gb2312Data(param));
                return item.ToArray();
            }).ToArray();

            return data;
        }

        /// <summary>
        /// 打印非标在pc上
        /// </summary>
        /// <param name="list"></param>
        public async void PrintNonStandardOnPC(List<LabelInfoNotify> list) {
            await Task.Run(() => {
                var printer = Global.NonStandardPrinter;
                if (printer == null)
                {
                    list.ForEach(a => a.TubeLabelStatus = -1);
                    return;
                }

                list.ForEach(a => {
                    var data = GetPrintData(a.TubeInfo,"1",a.TubeInfo.LableType+"");
                    var printObj = new Printer.PrinterListItem { Data=data,BackObj=a};
                    printer.Print(printObj);
                });
                list.ForEach(a => a.TubeLabelStatus = 100);
            });
        }
        /// <summary>
        /// 打印回执单在pc上
        /// </summary>
        /// <param name="list"></param>
        public async void PrintBackOrderOnPC(List<LabelInfoNotify> list)
        {
            await Task.Run(() => {
                var printer = Global.BackOrderPrinter;
                if (printer == null)
                {
                    list.ForEach(a => a.TubeLabelStatus = -1);
                    return;
                }

                list.ForEach(a => {
                    var data = GetPrintBackOrderData(a.TubeInfo);
                    var printObj = new Printer.PrinterListItem { Data = data, BackObj = a };
                    printer.Print(printObj);
                });
                list.ForEach(a => a.TubeLabelStatus = 100);
            });
        }
        /// <summary>
        /// 开始剩余时间计时器
        /// </summary>
        public void StartRemainingTimer() {
            var timer = new System.Timers.Timer(1000);

            timer.Elapsed += new System.Timers.ElapsedEventHandler((sender, args) => { 
                timer.Enabled = false;
                //if (Global.BindingInfo.SysInfo.RemainingTime <= 0)
                //{
                //    goto end;
                //}
                //var old = Global.BindingInfo.SysInfo.RemainingTime;
                //Global.BindingInfo.SysInfo.RemainingTime--;
                //if(!(Global.BindingInfo.SysInfo.RemainingTime==0 && old > 0)) { 
                //    goto end;
                //}
                //Global.LabelController.SendLabelList();
                //end:
                Global.BindingInfo.LabelQueue.Where(a => a.TubeLabelStatus >= 1 && a.TubeLabelStatus < 100 && a.SendTime.AddSeconds(int.Parse(Configs.Settings["LabelingTimeout"])) < DateTime.Now).ToList().ForEach(a=>a.TubeLabelStatus=-3);
                Global.BindingInfo.Queues.Where(a => a.Status == 1).ToList().ForEach(a => {
                    if (a.RemainingTime>=DateTime.Now )
                    {
                        return;
                    }
                    a.Status = 254;
                    var commid = Global.LPM.StartLabelList(a.Id,a.Nums,0,null);
                    a.CommId = commid;
                } );
                var working = Global.BindingInfo.Queues.FirstOrDefault(a => a.Status == 2);
                if (working != null) {
                    Global.BindingInfo.SysInfo.MachineStatus= 1;
                    Global.BindingInfo.SysInfo.RemainingTime = 0;
                    goto end;
                }
                var waiting = Global.BindingInfo.Queues.FirstOrDefault(a => a.Status == 1);
                if (working != null)
                {
                    Global.BindingInfo.SysInfo.MachineStatus = -1;
                    Global.BindingInfo.SysInfo.RemainingTime = (uint)(working.RemainingTime.AddSeconds(working.Remaining)- DateTime.Now).TotalSeconds;
                    goto end;
                }
                Global.BindingInfo.SysInfo.MachineStatus = 0;

            end:
                timer.Enabled = true;
            });
            timer.Enabled = true;
        }
        /// <summary>
        /// 字符串的Hex格式转byte数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] stringData2Byte(string data) {
            if (data == null) { 
                return null;
            }
            var index = 0;
            var strs = data.Select(a => new { val = a, index = index++ }).GroupBy(a => a.index / 2).Select(a => string.Join("", a.Select(b => b.val))).ToList();
            byte tmp;
            if (strs.Any(a => !byte.TryParse(a, System.Globalization.NumberStyles.HexNumber,null, out tmp))) {
                return null;
            }
            return strs.Select(a => byte.Parse(a, System.Globalization.NumberStyles.HexNumber)).ToArray();
        }
        /// <summary>
        /// ascii转换
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private byte[] gb2312Data(string code)
        {
            return Encoding.GetEncoding("gb2312").GetBytes(code+"");
        }
    }
    /// <summary>
    /// 被动模式问题
    /// </summary>
    public class PassiveClass
    {
        public enum PassiveStatus :byte{ 
            /// <summary>
            /// 等待接入
            /// </summary>
            Waiting=0,
            /// <summary>
            /// 获取数据
            /// </summary>
            Getting=1,
            /// <summary>
            /// 打印
            /// </summary>
            Printing=2
        }

        public PassiveStatus Status { get; set; } = PassiveStatus.Waiting;

        private System.Timers.Timer _timer = null;

        public void Start() {
            if (_timer != null)
            {
                _timer.Enabled = false;
                _timer.Dispose();
            }
            _timer = new System.Timers.Timer(3000);

            _timer.Elapsed += _timer_Elapsed;

            _timer.Enabled = true;
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var timer = (System.Timers.Timer)sender;
            timer.Enabled = false;
            if (Status != PassiveStatus.Waiting)
            {
                goto finish;
            }
            if(Global.BindingInfo.LabelQueue.Any(a=>a.TubeLabelStatus==0 || a.TubeLabelStatus == 1|| a.TubeLabelStatus == 10))
            {
                goto finish;
            }
            var labelBll = new Bll.Label();
            var deviceId = "";
            if (Configs.Settings["Passive"]== "truename")
            {
                deviceId = Global.LocalUser.TrueName;
            }
            else if (Configs.Settings["Passive"] == "loginname")
            {
                deviceId = Global.LocalUser.LoginName;
            }
            else
            {
                deviceId = Configs.Settings["DeviceID"];
            }
            var label = labelBll.GetLastLabel(deviceId, DateTime.Now.AddMinutes(-5));
            if (label == null)
            {
                goto finish;
            }
            Global.LabelController.GetABarCode(label.PatientID).Wait();

            finish:
            timer.Enabled = true;
        }
    }
}

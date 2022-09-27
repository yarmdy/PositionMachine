﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZEHOU.PM.Printer;
using ZEHOU.PM.Command;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ZHWindow
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Global.SynchronizationContext = System.Threading.SynchronizationContext.Current;
            Global.TabControl = tbTab;
            setMenu();
            intBarcodeGun();
            initLPM();
            initBinding();
            Global.LabelController = new LabelController();
            InitTubeColorDic();
            Global.LabelController.StartRemainingTimer();
            initPrinter();
            initPassiveClass();
        }

        
        #region 初始化
        /// <summary>
        /// 设置菜单权限
        /// </summary>
        private void setMenu()
        {
            var funcs = Global.LocalFuncs;
            var controls = getFuncControls();
            controls.ForEach(a => {
                if (!funcs.Contains(a.DataContext + ""))
                {
                    a.Visibility = Visibility.Collapsed;
                }
            });
            if (!funcs.Any(a => a.StartsWith("0")))
            {
                btnLabelManager.Visibility = Visibility.Collapsed;
                btnSystemSetting_Click(null, null);
            }
            if (!funcs.Any(a => a.StartsWith("5")))
            {
                btnSystemSetting.Visibility = Visibility.Collapsed;
                btnLabelManager_Click(null, null);
            }
        }
        /// <summary>
        /// 获取权限菜单
        /// </summary>
        /// <returns></returns>
        private List<Control> getFuncControls()
        {
            var list = new List<Control>();
            void addtolist(UIElement control)
            {
                var ttp = control.GetType();
                var propDataContext = ttp.GetProperty("DataContext");
                if (propDataContext == null) return;
                var valueDataContext = propDataContext.GetValue(control);
                if (valueDataContext == null || valueDataContext.GetType() != typeof(string)) return;
                list.Add((Control)control);
            }
            foreach (UIElement control in menuAct.Items)
            {
                addtolist(control);
            }
            foreach (UIElement control in spLabelManager.Children)
            {
                addtolist(control);
            }
            foreach (UIElement control in spSystemSetting.Children)
            {
                addtolist(control);
            }

            return list;
        }
        /// <summary>
        /// 初始化扫码枪
        /// </summary>
        private void intBarcodeGun()
        {
            BarCodeScan.HookHelper.OnScan = new Action<string>(a => {
                UILog.Info("扫码：" + a);
                Global.LabelController.GetABarCode(a);
            });
            BarCodeScan.HookHelper.Hook_Start();
        }
        /// <summary>
        /// 初始化下位机事件
        /// </summary>
        private void initLPM()
        {
            Global.LPM = new SerialPort.LabelMachineHelper(Config.Configs.Settings["PortName"]);
            Global.LPM.MachineId = byte.Parse(Config.Configs.Settings["MachineId"]);
            Global.LPM.OnError += LPM_OnError;
            Global.LPM.OnSent += LPM_OnSent;
            Global.LPM.OnReceive += LPM_OnReceive;
            Global.LPM.OnBaseReceive += LPM_OnBaseReceive;

            Global.LPM.OnBackBackOrder += LPM_OnBackBackOrder;
            Global.LPM.OnBackCancelList += LPM_OnBackCancelList;
            Global.LPM.OnBackLabel += LPM_OnBackLabel;
            Global.LPM.OnBackLabelList += LPM_OnBackLabelList;
            Global.LPM.OnBackLightTest += LPM_OnBackLightTest;
            Global.LPM.OnBackSpecialLabel += LPM_OnBackSpecialLabel;
            Global.LPM.OnConnect += LPM_OnConnect;
            Global.LPM.OnDisconnect += LPM_OnDisconnect;
            Global.LPM.OnLabelStatus += LPM_OnLabelStatus;
            Global.LPM.OnLightStatus += LPM_OnLightStatus;
            Global.LPM.OnMyTurn += LPM_OnMyTurn;

            //Global.LPM.TestAddSend(new byte[] {0x40,0x5A,0x48,0x01,0x50,0xD2,0x01,0x10,0x00,0x12,0x00,0x3C,0x21,0x34,0x05,0x78,0x22,0xF6,0x22,0x2E,0x05,0xDC,0x1D,0x1A,0x1C,0x52,0x00,0x14,0xFC,0x3B,0x0D,0x0A
            //});
            //Global.LPM.TestAddSend(new byte[] {0x40,0x5A,0x48,0x01,0x50,0xD2,0x01,0x02,0x00,0x12,0x00,0x3C,0x21,0x34
            //});
            //System.Threading.Thread.Sleep(100);
            //Global.LPM.TestAddSend(new byte[] {0x05,0x78,0x22,0xF6,0x22,0x2E,0x05,0xDC,0x1D,0x1A,0x1C,0x52,0x00,0x14
            //});
            //System.Threading.Thread.Sleep(100);
            //Global.LPM.TestAddSend(new byte[] {0xCD,0x48,0x0D,0x0A
            //});
        }
        /// <summary>
        /// 初始化界面绑定
        /// </summary>
        private void initBinding()
        {
            Global.BindingInfo = new LabelBindingNotify { };
            Global.BindingInfo.LocalLabelList = new ObservableCollection<LabelInfoNotify>();
            Global.BindingInfo.LocalPatient = new PatientInfoNotify();
            Global.BindingInfo.QueueId = 0;
            Global.BindingInfo.LabelQueue = new ObservableCollection<LabelInfoNotify>();
            Global.BindingInfo.SysInfo = new SysInfoNotify();
            Global.BindingInfo.SysInfo.AutoReadCard = true;
            Global.BindingInfo.SysInfo.MachineStatus = 0;
            Global.BindingInfo.SysInfo.PrintAndLabel = true;
            Global.BindingInfo.SysInfo.EndTime = DateTime.Now.Date;
            Global.BindingInfo.SysInfo.StartTime = DateTime.Now.Date.AddMonths(-3);
            Global.BindingInfo.SysInfo.IsOpened = Global.LPM.IsOpen;
            Global.BindingInfo.SysInfo.RemainingTime = 0;
            Global.BindingInfo.SysInfo.SysStatus = 0;
            Global.BindingInfo.Queues = new ObservableCollection<QueueInfo>();
            this.DataContext = Global.BindingInfo;

            Global.BindingInfo.LabelQueue.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((a, e) =>
            {
                var queue = (ObservableCollection<LabelInfoNotify>)a;
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        {
                            //UILog.Info($"贴标列队添加【{((LabelInfoNotify)e.NewItems[0])?.TubeInfo?.BarCode}】");
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        {
                            //UILog.Info($"贴标列队移除【{((LabelInfoNotify)e.OldItems[0])?.TubeInfo?.BarCode}】");
                            Global.LabelController.SendLabelList();
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        {
                            //UILog.Info($"贴标列队重置");
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                        {
                            //UILog.Info($"贴标列队替换【{((LabelInfoNotify)e.OldItems[0])?.TubeInfo?.BarCode}】->【{((LabelInfoNotify)e.NewItems[0])?.TubeInfo?.BarCode}】");
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                        {
                            //UILog.Info($"贴标列队移动【{((LabelInfoNotify)e.OldItems[0])?.TubeInfo?.BarCode}】->【{((LabelInfoNotify)e.NewItems[0])?.TubeInfo?.BarCode}】");
                        }
                        break;
                }
            });
        }
        /// <summary>
        /// 初始化试管颜色
        /// </summary>
        public void InitTubeColorDic()
        {
            var json = "{}";
            if (System.IO.File.Exists(Global.TubeColorConfigFile))
            {
                json = System.IO.File.ReadAllText(Global.TubeColorConfigFile);
            }
            Global.TubeColorDic = new Dictionary<string, int>();
            try
            {
                Global.TubeColorDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            }
            catch (Exception ex)
            {
                UILog.Error("加载试管颜色出错", ex);
            }
            if (Global.TubeColorDic.Count <= 0)
            {
                json = @"{
  ""仅打印"": 0,
  ""蓝色"": -16744193,
  ""黑色"": -16777216,
  ""浅紫色"": -8355585,
  ""深紫色"": -12582784,
  ""绿色"": -16744448,
  ""灰色"": -8355712,
  ""淡黄色"": -128,
  ""橙黄色"": -32768,
  ""红色"": -65536
}";
                Global.TubeColorDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            }
        }
        /// <summary>
        /// 初始化打印机
        /// </summary>
        private void initPrinter() {
            Printer.LabelPrinterApi.InitPrinterDll();
            if (!Global.IsMachineNonStandard) {
                Global.NonStandardPrinter = new Printer.LabelPrinter("非标打印机", "LPQ80", Config.Configs.Settings["NonStandard"]);
                Global.NonStandardPrinter.OnError += NonStandardPrinter_OnError;
            }
            if (Global.IsSamePrinter)
            {
                Global.BackOrderPrinter = Global.NonStandardPrinter;
            }
            else if (Config.Configs.Settings["BackOrder"] != "" && Global.IsMachineBackOrder) {
                Global.BackOrderPrinter = new Printer.LabelPrinter("回执单打印机", "LPQ80", Config.Configs.Settings["BackOrder"]);
                Global.BackOrderPrinter.OnError += BackOrderPrinter_OnError;
            }
        }

        private void BackOrderPrinter_OnError(Printer.LabelPrinter arg1, Printer.PrinterEventArgs arg2)
        {
            UILog.Error(arg2.Message, null);
            prossPrintError(arg2);
        }

        private void NonStandardPrinter_OnError(Printer.LabelPrinter arg1, Printer.PrinterEventArgs arg2)
        {
            UILog.Error(arg2.Message, null);
            prossPrintError(arg2);
        }

        private void prossPrintError(PrinterEventArgs arg2)
        {
            if (arg2.BackObj == null)
            {
                return;
            }
            if (arg2.BackObj is List<object>)
            {
                ((List<object>)arg2.BackObj).ForEach(a =>
                {
                    if (a is LabelInfoNotify)
                    {
                        return;
                    }
                    ((LabelInfoNotify)a).TubeLabelStatus = -1;
                });
                return;
            }
            if (arg2.BackObj is LabelInfoNotify)
            {
                ((LabelInfoNotify)arg2.BackObj).TubeLabelStatus = -1;
                return;
            }
        }
        private void initPassiveClass()
        {
            var cfgPassive = Config.Configs.Settings["Passive"];
            if (cfgPassive=="none" || cfgPassive == "")
            {
                return;
            }
            Global.PassiveClass = new PassiveClass();
            Global.PassiveClass.Start();
        }
        #endregion

        #region 界面逻辑
        /// <summary>
        /// 菜单点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var control = (Control)sender;
            var tag = control.Tag + "";

            var header = "";

            var tt = control.GetType();
            var headerProp = tt.GetProperty("Header");
            var textProp = tt.GetProperty("Text");

            if (headerProp != null)
            {
                header = (string)headerProp.GetValue(control);
            }
            else if (textProp != null)
            {
                header = (string)textProp.GetValue(control);
            }

            OpenWindowWithClassName(tag, header);
        }
        /// <summary>
        /// 打开窗体和标签页逻辑
        /// </summary>
        /// <param name="className"></param>
        /// <param name="title"></param>
        private void OpenWindowWithClassName(string className, string title)
        {
            var tagarr = className.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var objName = tagarr[0];
            var isWin = false;
            if (tagarr.Length > 1)
            {
                isWin = true;
            }
            if (isWin)
            {
                var window = (Window)Activator.CreateInstance(Type.GetType("ZEHOU.PM.Label." + objName, false, true));
                window?.ShowDialog();
                return;
            }

            Global.AddTab(objName, title);
        }
        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_Quit(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = UI.Popup.Confirm(this, "确认要退出系统吗？");
            if (result == MessageBoxResult.OK)
            {
                return;
            }
            e.Cancel = true;
        }
        /// <summary>
        /// 侧边栏贴标管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLabelManager_Click(object sender, RoutedEventArgs e)
        {
            svLabelManager.Visibility = Visibility.Visible;
            DockPanel.SetDock(btnSystemSetting, Dock.Bottom);
            svSystemSetting.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 侧边栏系统设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSystemSetting_Click(object sender, RoutedEventArgs e)
        {
            svLabelManager.Visibility = Visibility.Collapsed;
            DockPanel.SetDock(btnSystemSetting, Dock.Top);
            svSystemSetting.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gdDebug.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 系统消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            gdSysLog.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 打印列队
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            gdQueue.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 状态栏事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusBarItemDown(object sender, MouseButtonEventArgs e)
        {
            var act = ((Control)sender).Tag + "";
            gdSysLog.Visibility = Visibility.Collapsed;
            gdDebug.Visibility = Visibility.Collapsed;
            gdQueue.Visibility = Visibility.Collapsed;
            switch (act)
            {
                case "syslog":
                    {
                        gdSysLog.Visibility = Visibility.Visible;
                    }
                    break;
                case "debuglog":
                    {
                        gdDebug.Visibility = Visibility.Visible;
                    }
                    break;
                case "queue":
                    {
                        gdQueue.Visibility = Visibility.Visible;
                    }
                    break;
            }
        }
        /// <summary>
        /// 自动滚动到最新消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            Global.SynchronizationContext.Post(new System.Threading.SendOrPostCallback(a => ((TextBox)sender).ScrollToEnd()), System.Threading.Thread.CurrentThread.ManagedThreadId);
        }
        /// <summary>
        /// 暂停继续
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Global.BindingInfo.SysInfo.SysStatus = Global.BindingInfo.SysInfo.SysStatus < 0 ? 0 : -1;
        }
        /// <summary>
        /// 删除列队条目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var objs = getQueueSelectObjs();
            if (objs.Count <= 0)
            {
                UI.Popup.Error(Global.MainWindow, "请选择要删除的记录");
                return;
            }

            var errorOne = objs.FirstOrDefault(a => a.TubeLabelStatus >= 10 && a.TubeLabelStatus < 100);
            if (errorOne != null)
            {
                UI.Popup.Error(Global.MainWindow, $"【{errorOne.TubeInfo.BarCode}】正在贴标中无法删除");
                return;
            }
            var res = UI.Popup.Confirm(Global.MainWindow, $"您确定要将【{(string.Join("、", objs.Select(a => a.TubeInfo.BarCode)))}】从列队中移除吗？");
            if (res != MessageBoxResult.OK)
            {
                return;
            }
            objs.ForEach(a => Global.BindingInfo.LabelQueue.Remove(a));
        }
        /// <summary>
        /// 获取选中的列队信息
        /// </summary>
        /// <returns></returns>
        private LabelInfoNotify getQueueSelectObj()
        {
            if (lvQueue.SelectedItem == null || lvQueue.SelectedItem.GetType() != typeof(LabelInfoNotify))
            {
                return null;
            }
            return lvQueue.SelectedItem as LabelInfoNotify;
        }
        /// <summary>
        /// 获取选中的列队信息
        /// </summary>
        /// <returns></returns>
        private List<LabelInfoNotify> getQueueSelectObjs()
        {
            var res = new List<LabelInfoNotify>();
            foreach (var item in lvQueue.SelectedItems)
            {
                if (item == null || item.GetType() != typeof(LabelInfoNotify))
                {
                    continue;
                }
                res.Add(item as LabelInfoNotify);
            }

            return res;
        }
        /// <summary>
        /// 清空列队
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var res = UI.Popup.Confirm(Global.MainWindow, $"您确定要清空贴标列队吗？");
            if (res != MessageBoxResult.OK)
            {
                return;
            }
            for (int i = 0; i < Global.BindingInfo.LabelQueue.Count; i++)
            {
                if (Global.BindingInfo.LabelQueue[i].TubeLabelStatus >= 10 && Global.BindingInfo.LabelQueue[i].TubeLabelStatus < 100)
                {
                    continue;
                }
                Global.BindingInfo.LabelQueue.RemoveAt(i--);
            }
            for (int i = 0; i < Global.BindingInfo.LocalLabelList.Count; i++)
            {
                if (Global.BindingInfo.LocalLabelList[i].TubeLabelStatus >= 10 && Global.BindingInfo.LabelQueue[i].TubeLabelStatus < 100)
                {
                    continue;
                }
                Global.BindingInfo.LocalLabelList.RemoveAt(i--);
            }
            Global.BindingInfo.LocalPatient = new PatientInfoNotify();
        }
        /// <summary>
        /// 重试失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            int retryCount = 0;
            foreach (var item in Global.BindingInfo.LabelQueue)
            {
                if (item.TubeLabelStatus >= 10)
                {
                    continue;
                }
                item.TubeLabelStatus = 0;
                retryCount++;
            }
            if (retryCount > 0)
            {
                UI.Popup.Succ(Global.MainWindow, $"已重试{retryCount}条贴标记录");
                Global.LabelController.SendLabelList();
                return;
            }
            UI.Popup.Error(Global.MainWindow, $"没有需要重试的贴标记录");
        }
        #endregion

        #region 下位机事件
        /// <summary>
        /// 轮到我了
        /// </summary>
        /// <param name="obj"></param>
        private void LPM_OnMyTurn(SerialPort.DataPackage obj)
        {
            Global.BindingInfo.SysInfo.MachineStatus = 0;
            Global.BindingInfo.SysInfo.RemainingTime = 0;
            UILog.Info("下位机空闲，可以接受贴标了");
            Global.LabelController.SendLabelInfo();
        }
        /// <summary>
        /// 开关状态设置
        /// </summary>
        /// <param name="obj"></param>
        private void LPM_OnLightStatus(SerialPort.DataPackage obj)
        {
            //throw new NotImplementedException();
            //需要处理 开关状态
        }
        /// <summary>
        /// 反馈贴标状态
        /// </summary>
        /// <param name="obj"></param>
        private void LPM_OnLabelStatus(SerialPort.DataPackage obj)
        {
            //需要处理 贴标状态
            if (Global.BindingInfo.LocalLabel == null)
            {
                if (Global.BindingInfo.SysInfo.SysStatus < 0 || Global.BindingInfo.SysInfo.SysStatus == 0)
                {
                    return;
                }
                Global.BindingInfo.SysInfo.SysStatus = 0;
                return;
            }
            if (obj.Data[0] == 1)
            {
                var finishiOne = Global.BindingInfo.AlmostDoneLabel ?? Global.BindingInfo.LocalLabel;
                UILog.Info($"【{finishiOne.TubeInfo.BarCode}】贴标完成，添加贴标记录");
                var lr = new DB.dbLabelInfo.LR { };
                if (Global.BindingInfo.AlmostDoneLabel == null)
                {
                    lr.CopyFrom(Global.BindingInfo.LocalLabel.TubeInfo);
                    Global.BindingInfo.LocalLabel.TubeLabelStatus = 10;
                    Global.BindingInfo.LocalLabel = null;
                }
                else {
                    lr.CopyFrom(Global.BindingInfo.AlmostDoneLabel.TubeInfo);
                    Global.BindingInfo.AlmostDoneLabel.TubeLabelStatus = 10;
                    Global.BindingInfo.AlmostDoneLabel = null;
                    Global.LabelController.removeAPos();
                }
                lr.PrintTime=DateTime.Now;
                var reportBll = new Bll.Report();
                var ret = reportBll.AddOrEditLr(lr);
                if (ret > 0)
                {
                    UILog.Info($"【{finishiOne.TubeInfo.BarCode}】添加贴标记录成功");
                }
                else {
                    UILog.Error($"【{finishiOne.TubeInfo.BarCode}】添加贴标记录失败",null);
                }
                if (Global.BindingInfo.SysInfo.SysStatus < 0 || Global.BindingInfo.SysInfo.SysStatus == 0)
                {
                    return;
                }
                Global.BindingInfo.SysInfo.SysStatus = 0;
            }
            if (obj.Data[0] == 255)
            {
                UILog.Info($"【{Global.BindingInfo.LocalLabel.TubeInfo.BarCode}】下位机贴标错误");
                Global.BindingInfo.LocalLabel.TubeLabelStatus = -1;
                Global.BindingInfo.LocalLabel = null;
                Global.LabelController.removeAPos();
                if (Global.BindingInfo.SysInfo.SysStatus < 0 || Global.BindingInfo.SysInfo.SysStatus == 0)
                {
                    return;
                }
                Global.BindingInfo.SysInfo.SysStatus = 0;
                return;
            }
            if (obj.Data[0] == 254)
            {
                UILog.Error($"【{Global.BindingInfo.LocalLabel.TubeInfo.BarCode}】下位机缺管", null);
                var tubeTypes = Global.BindingInfo.LocalLabel.BinId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                Global.BindingInfo.LocalLabel.TubeLabelStatus = -2;
                Global.BindingInfo.LocalLabel = null;
                Global.LabelController.removeAPos();

                var lackCount = 0;
                foreach (var tube in Global.BindingInfo.LabelQueue)
                {
                    if (tube.TubeLabelStatus != 0)
                    {
                        continue;
                    }
                    var itemTubeTypes = tube.BinId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (!itemTubeTypes.All(a => tubeTypes.Contains(a)))
                    {
                        continue;
                    }
                    tube.TubeLabelStatus = -2;
                    Global.LabelController.removeAPos();
                    lackCount++;
                }
                UILog.Error($"检测到贴标列队其它缺管${lackCount}条", null);

                if (Global.BindingInfo.SysInfo.SysStatus < 0 || Global.BindingInfo.SysInfo.SysStatus == 0)
                {
                    return;
                }
                Global.BindingInfo.SysInfo.SysStatus = 0;
                return;
            }
            if (Global.BindingInfo.SysInfo.SysStatus < 0 || Global.BindingInfo.SysInfo.SysStatus == 1)
            {
                goto other;
            }
            Global.BindingInfo.SysInfo.SysStatus = 1;
        other:;
        }
        /// <summary>
        /// 断开串口
        /// </summary>
        private void LPM_OnDisconnect()
        {
            UILog.Info("断开串口");
            if (Global.BindingInfo == null)
            {
                return;
            }
            Global.BindingInfo.SysInfo.IsOpened = Global.LPM.IsOpen;

        }
        /// <summary>
        /// 打开串口
        /// </summary>
        private void LPM_OnConnect()
        {
            UILog.Info("连接串口");
            if (Global.BindingInfo == null)
            {
                return;
            }
            Global.BindingInfo.SysInfo.IsOpened = Global.LPM.IsOpen;
        }
        
        /// <summary>
        /// 反馈非标贴标
        /// </summary>
        /// <param name="obj"></param>
        private void LPM_OnBackSpecialLabel(SerialPort.DataPackage obj)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 反馈开关状态
        /// </summary>
        /// <param name="obj"></param>
        private void LPM_OnBackLightTest(SerialPort.DataPackage obj)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 反馈请求清单
        /// </summary>
        /// <param name="obj"></param>
        private void LPM_OnBackLabelList(SerialPort.DataPackage obj)
        {
            //throw new NotImplementedException();
            //需要处理 返回清单请求状态
            
            var queue = Global.BindingInfo.Queues.FirstOrDefault(a => a.CommId == obj.CommId);
            if (queue != null) { 
                queue.Remaining = BitConverter.ToUInt32(obj.Data.Skip(1).Reverse().ToArray(),0);
            }
            if (obj.Data[0] == 1)
            {
                UILog.Info($"下位机返回空闲");
                
                Global.BindingInfo.SysInfo.MachineStatus = 0;
                Global.BindingInfo.SysInfo.RemainingTime = 0;
                Global.LabelController.SendLabelInfo();
                return;
            }
            if (obj.Data[0] == 2)
            {
                UILog.Info($"下位机返回忙碌");

                //Global.BindingInfo.SysInfo.MachineStatus = -1;
                //Global.BindingInfo.SysInfo.RemainingTime = Convert.ToUInt32(obj.Data.Skip(1).Reverse().ToArray());
                return;
            }
        }
        /// <summary>
        /// 反馈贴标
        /// </summary>
        /// <param name="obj"></param>
        private void LPM_OnBackLabel(SerialPort.DataPackage obj)
        {
            //throw new NotImplementedException();
            if (Global.BindingInfo.LocalLabel == null)
            {
                return;
            }
            if (obj.Data[0] == 1)
            {
                UILog.Info($"【{Global.BindingInfo.LocalLabel.TubeInfo.BarCode}】下位机已接收，下位机开始工作");
                Global.BindingInfo.LocalLabel.TubeLabelStatus = 10;
                Global.BindingInfo.SysInfo.MachineStatus = 1;
                Global.BindingInfo.SysInfo.RemainingTime = 0;
                return;
            }
            if (obj.Data[0] == 2)
            {
                UILog.Info($"【{Global.BindingInfo.LocalLabel.TubeInfo.BarCode}】下位机拒绝接收");
                Global.BindingInfo.LocalLabel.TubeLabelStatus = -1;
                Global.BindingInfo.LocalLabel = null;
                return;
            }
        }
        /// <summary>
        /// 反馈取消清单
        /// </summary>
        /// <param name="obj"></param>
        private void LPM_OnBackCancelList(SerialPort.DataPackage obj)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 反馈回执单
        /// </summary>
        /// <param name="obj"></param>
        private void LPM_OnBackBackOrder(SerialPort.DataPackage obj)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 基础串口接收
        /// </summary>
        /// <param name="obj"></param>
        private void LPM_OnBaseReceive(byte[] obj)
        {
            Global.SerialBaseReceiveLog.Debug(string.Join(" ", obj.Select(a => a.ToString("X2"))));
        }
        /// <summary>
        /// 优化后串口接收
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool LPM_OnReceive(byte[] arg)
        {
            UILog.Receive(string.Join(" ", arg.Select(a => a.ToString("X2"))));
            return true;
        }
        /// <summary>
        /// 串口发送
        /// </summary>
        /// <param name="obj"></param>
        private void LPM_OnSent(byte[] obj)
        {
            UILog.Send(string.Join(" ", obj.Select(a => a.ToString("X2"))));
        }
        /// <summary>
        /// 串口错误
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void LPM_OnError(int arg1, string arg2, Exception arg3)
        {
            UILog.Error($"({arg1}){arg2}", arg3);
        }
        #endregion

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Global.BindingInfo.IsHideLeft = !Global.BindingInfo.IsHideLeft;
        }
    }
    /// <summary>
    /// tab源
    /// </summary>
    public class TabModel { 
        public string Header { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
    }
}
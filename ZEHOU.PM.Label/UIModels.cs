using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// 绑定类
    /// </summary>
    public class LabelBindingNotify : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 清单号
        /// </summary>
        private byte _QueueId;
        /// <summary>
        /// 清单号
        /// </summary>
        public byte QueueId
        {
            get { return _QueueId; }
            set { _QueueId = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("QueueId")); }
        }
        /// <summary>
        /// 清单号
        /// </summary>
        private ObservableCollection<QueueInfo> _QueueInfo;
        /// <summary>
        /// 清单号
        /// </summary>
        public ObservableCollection<QueueInfo> Queues
        {
            get { return _QueueInfo; }
            set { _QueueInfo = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("QueueInfo")); }
        }

        /// <summary>
        /// 当前患者
        /// </summary>
        private PatientInfoNotify _LocalPatient;
        /// <summary>
        /// 当前患者
        /// </summary>
        public PatientInfoNotify LocalPatient
        {
            get { return _LocalPatient; }
            set { _LocalPatient = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LocalPatient")); }
        }
        /// <summary>
        /// 当前贴标信息
        /// </summary>
        private ObservableCollection<LabelInfoNotify> _LocalLabelList;
        /// <summary>
        /// 当前贴标信息
        /// </summary>
        public ObservableCollection<LabelInfoNotify> LocalLabelList
        {
            get { return _LocalLabelList; }
            set { _LocalLabelList = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LocalLabelList")); }
        }
        /// <summary>
        /// 贴标列队
        /// </summary>
        private ObservableCollection<LabelInfoNotify> _LabelQueue;
        /// <summary>
        /// 贴标列队
        /// </summary>
        public ObservableCollection<LabelInfoNotify> LabelQueue
        {
            get { return _LabelQueue; }
            set { _LabelQueue = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LabelQueue")); }
        }
        /// <summary>
        /// 当前贴标
        /// </summary>
        private LabelInfoNotify _LocalLabel;
        /// <summary>
        /// 当前贴标
        /// </summary>
        public LabelInfoNotify LocalLabel
        {
            get { return _LocalLabel; }
            set { _LocalLabel = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LocalLabel")); }
        }
        /// <summary>
        /// 快完成的贴标
        /// </summary>
        private LabelInfoNotify _AlmostDoneLabel;
        /// <summary>
        /// 当前贴标
        /// </summary>
        public LabelInfoNotify AlmostDoneLabel
        {
            get { return _AlmostDoneLabel; }
            set { _AlmostDoneLabel = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("AlmostDoneLabel")); }
        }
        /// <summary>
        /// 系统信息
        /// </summary>
        private SysInfoNotify _SysInfo;
        /// <summary>
        /// 系统信息
        /// </summary>
        public SysInfoNotify SysInfo
        {
            get { return _SysInfo; }
            set { _SysInfo = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("SysInfo")); }
        }
        /// <summary>
        /// 隐藏边栏
        /// </summary>
        private bool _IsHideLeft;
        /// <summary>
        /// 隐藏边栏
        /// </summary>
        public bool IsHideLeft
        {
            get { return _IsHideLeft; }
            set { _IsHideLeft = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("IsHideLeft")); }
        }

        /// <summary>
        /// 系统日志
        /// </summary>
        public ObservableCollection<string> SysLog { get; } = new ObservableCollection<string>();
        /// <summary>
        /// 系统日志绑定界面
        /// </summary>
        public string SysLogStr
        {
            get {
                return string.Join("\r\n",SysLog);
            }
        }
        /// <summary>
        /// 调试日志
        /// </summary>
        public ObservableCollection<string> DebugLog { get; } = new ObservableCollection<string>();
        /// <summary>
        /// 调试日志绑定界面
        /// </summary>
        public string DebugLogStr
        {
            get
            {
                return string.Join("\r\n", DebugLog);
            }
        }
        /// <summary>
        /// 最新消息
        /// </summary>
        public string LastMessage
        {
            get
            {
                var msg = SysLog.LastOrDefault() ?? "";

                return msg.Length >= 13 ? msg.Substring(13) : msg;
            }
        }


        /// <summary>
        /// 仓位状态
        /// </summary>
        private ObservableCollection<BinStatusInfo> _BinsList;
        /// <summary>
        /// 仓位状态
        /// </summary>
        public ObservableCollection<BinStatusInfo> BinsList
        {
            get { return _BinsList; }
            set { _BinsList = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BinsList")); }
        }

        /// <summary>
        /// 实例化初始化日志事件
        /// </summary>
        public LabelBindingNotify()
        {
            SysLog.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((a, b) =>
            {
                if (PropertyChanged == null) return;
                //PropertyChanged(this, new PropertyChangedEventArgs("SysLog"));
                PropertyChanged(this, new PropertyChangedEventArgs("SysLogStr"));
                PropertyChanged(this, new PropertyChangedEventArgs("LastMessage"));
            });
            DebugLog.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((a, b) =>
            {
                if (PropertyChanged == null) return;
                //PropertyChanged(this, new PropertyChangedEventArgs("DebugLog"));
                PropertyChanged(this, new PropertyChangedEventArgs("DebugLogStr"));
            });
        }
    }
    /// <summary>
    /// 患者和贴标信息
    /// </summary>
    public class LabelInfoNotify : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 患者
        /// </summary>
        private PatientInfoNotify _Patient;
        /// <summary>
        /// 患者
        /// </summary>
        public PatientInfoNotify Patient
        {
            get { return _Patient; }
            set { _Patient = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Patient")); }
        }
        /// <summary>
        /// 试管
        /// </summary>
        private TubeInfoNotify _TubeInfo;
        /// <summary>
        /// 试管
        /// </summary>
        public TubeInfoNotify TubeInfo
        {
            get { return _TubeInfo; }
            set { _TubeInfo = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("TubeInfo")); }
        }
        /// <summary>
        /// 是否选中
        /// </summary>
        private bool _IsChecked;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("IsChecked")); }
        }
        /// <summary>
        /// 是否测试
        /// </summary>
        private bool _IsTest;
        /// <summary>
        /// 是否测试
        /// </summary>
        public bool IsTest
        {
            get { return _IsTest; }
            set { _IsTest = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("IsTest")); }
        }
        /// <summary>
        /// 试管颜色值
        /// </summary>
        private int _TubeColor;
        /// <summary>
        /// 试管颜色值
        /// </summary>
        public int TubeColor
        {
            get { return _TubeColor; }
            set { _TubeColor = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("TubeColor")); }
        }
        /// <summary>
        /// 贴标状态 0等待贴标 1已发送 10已接受 100已完成 -1错误 -2缺管 -3超时 -4掉管
        /// </summary>
        private int _TubeLabelStatus;
        /// <summary>
        /// 贴标状态 0等待贴标 1已发送 10已接受 100已完成 -0xa1到-0xa4（掉管 主辊压空 压辊不到位 压辊不归位） -0xd1到-0xd4（缺管 复核不匹配 无条码 巡边故障）
        /// </summary>
        public int TubeLabelStatus
        {
            get { return _TubeLabelStatus; }
            set { _TubeLabelStatus = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("TubeLabelStatus")); }
        }

        /// <summary>
        /// 发送给下位机时间用来判断超时
        /// </summary>
        private DateTime _SendTime;
        /// <summary>
        /// 发送给下位机时间用来判断超时
        /// </summary>
        public DateTime SendTime
        {
            get { return _SendTime; }
            set { _SendTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("SendTime")); }
        }

        /// <summary>
        /// 仓位
        /// </summary>
        private string _BinId;
        /// <summary>
        /// 仓位
        /// </summary>
        public string BinId
        {
            get { return _BinId; }
            set { _BinId = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BinId")); }
        }
        /// <summary>
        /// 配备到的颜色
        /// </summary>
        private string _RegColor;
        /// <summary>
        /// 匹配到的颜色
        /// </summary>
        public string RegColor
        {
            get { return _RegColor; }
            set { _RegColor = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("RegColor")); }
        }
        /// <summary>
        /// 仅打印
        /// </summary>
        private bool _OnlyPrint;
        /// <summary>
        /// 仅打印
        /// </summary>
        public bool OnlyPrint
        {
            get { return _OnlyPrint; }
            set { _OnlyPrint = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("OnlyPrint")); }
        }

        ///// <summary>
        ///// 打印回执单
        ///// </summary>
        //private bool _PrintBackOrder;
        ///// <summary>
        ///// 打印回执单
        ///// </summary>
        //public bool PrintBackOrder
        //{
        //    get { return _PrintBackOrder; }
        //    set { _PrintBackOrder = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PrintBackOrder")); }
        //}
    }
    /// <summary>
    /// 患者基本信息
    /// </summary>
    public class PatientInfoNotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _ID;
        public string ID { get { return _ID; } set { _ID = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("ID")); } }
        private string _PatientType;
        public string PatientType { get { return _PatientType; } set { _PatientType = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PatientType")); } }
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Name")); } }
        private string _Gender;
        public string Gender { get { return _Gender; } set { _Gender = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Gender")); } }
        private int? _Age;
        public int? Age { get { return _Age; } set { _Age = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Age")); } }
        private string _AgeUnit;
        public string AgeUnit { get { return _AgeUnit; } set { _AgeUnit = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("AgeUnit")); } }
        private string _PS01;
        public string PS01 { get { return _PS01; } set { _PS01 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PS01")); } }
        private string _PS02;
        public string PS02 { get { return _PS02; } set { _PS02 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PS02")); } }
        private string _PS03;
        public string PS03 { get { return _PS03; } set { _PS03 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PS03")); } }
        private string _PS04;
        public string PS04 { get { return _PS04; } set { _PS04 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PS04")); } }
        private string _PS05;
        public string PS05 { get { return _PS05; } set { _PS05 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PS05")); } }
        private string _PS06;
        public string PS06 { get { return _PS06; } set { _PS06 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PS06")); } }
        private string _PS07;
        public string PS07 { get { return _PS07; } set { _PS07 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PS07")); } }
        private string _PS08;
        public string PS08 { get { return _PS08; } set { _PS08 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PS08")); } }
        private string _PS09;
        public string PS09 { get { return _PS09; } set { _PS09 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PS09")); } }
        private string _PS10;
        public string PS10 { get { return _PS10; } set { _PS10 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PS10")); } }
        private DateTime _CreateTime;
        public DateTime CreateTime { get { return _CreateTime; } set { _CreateTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("CreateTime")); } }
        private DateTime _UpdateTime;
        public DateTime UpdateTime { get { return _UpdateTime; } set { _UpdateTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("UpdateTime")); } }

    }
    /// <summary>
    /// 试管基本信息
    /// </summary>
    public class TubeInfoNotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _BarCode; 
        public string BarCode { get { return _BarCode; } set { _BarCode = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BarCode")); } }
        private string _PatientID; 
        public string PatientID { get { return _PatientID; } set { _PatientID = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PatientID")); } }
        private string _DeviceID; 
        public string DeviceID { get { return _DeviceID; } set { _DeviceID = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("DeviceID")); } }
        private string _Department; 
        public string Department { get { return _Department; } set { _Department = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Department")); } }
        private string _Bed; 
        public string Bed { get { return _Bed; } set { _Bed = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Bed")); } }
        private string _SampleName; 
        public string SampleName { get { return _SampleName; } set { _SampleName = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("SampleName")); } }
        private int _LableType; 
        public int LableType { get { return _LableType; } set { _LableType = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LableType")); } }
        private int _PrintCount; 
        public int PrintCount { get { return _PrintCount; } set { _PrintCount = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PrintCount")); } }
        private string _TubeColor; 
        public string TubeColor { get { return _TubeColor; } set { _TubeColor = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("TubeColor")); } }
        private string _TestGroup; 
        public string TestGroup { get { return _TestGroup; } set { _TestGroup = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("TestGroup")); } }
        private string _TestOrder; 
        public string TestOrder { get { return _TestOrder; } set { _TestOrder = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("TestOrder")); } }
        private string _EmergenteInfo; 
        public string EmergenteInfo { get { return _EmergenteInfo; } set { _EmergenteInfo = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("EmergenteInfo")); } }
        private string _BS01; 
        public string BS01 { get { return _BS01; } set { _BS01 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BS01")); } }
        private string _BS02; 
        public string BS02 { get { return _BS02; } set { _BS02 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BS02")); } }
        private string _BS03; 
        public string BS03 { get { return _BS03; } set { _BS03 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BS03")); } }
        private string _BS04; 
        public string BS04 { get { return _BS04; } set { _BS04 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BS04")); } }
        private string _BS05; 
        public string BS05 { get { return _BS05; } set { _BS05 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BS05")); } }
        private string _BS06; 
        public string BS06 { get { return _BS06; } set { _BS06 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BS06")); } }
        private string _BS07; 
        public string BS07 { get { return _BS07; } set { _BS07 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BS07")); } }
        private string _BS08; 
        public string BS08 { get { return _BS08; } set { _BS08 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BS08")); } }
        private string _BS09; 
        public string BS09 { get { return _BS09; } set { _BS09 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BS09")); } }
        private string _BS10; 
        public string BS10 { get { return _BS10; } set { _BS10 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BS10")); } }
        private string _LS01; 
        public string LS01 { get { return _LS01; } set { _LS01 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LS01")); } }
        private string _LS02; 
        public string LS02 { get { return _LS02; } set { _LS02 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LS02")); } }
        private string _LS03; 
        public string LS03 { get { return _LS03; } set { _LS03 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LS03")); } }
        private string _LS04; 
        public string LS04 { get { return _LS04; } set { _LS04 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LS04")); } }
        private string _LS05; 
        public string LS05 { get { return _LS05; } set { _LS05 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LS05")); } }
        private string _LS06; 
        public string LS06 { get { return _LS06; } set { _LS06 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LS06")); } }
        private string _LS07; 
        public string LS07 { get { return _LS07; } set { _LS07 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LS07")); } }
        private string _LS08; 
        public string LS08 { get { return _LS08; } set { _LS08 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LS08")); } }
        private string _LS09; 
        public string LS09 { get { return _LS09; } set { _LS09 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LS09")); } }
        private string _LS10; 
        public string LS10 { get { return _LS10; } set { _LS10 = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LS10")); } }
        private DateTime _CreateTime; 
        public DateTime CreateTime { get { return _CreateTime; } set { _CreateTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("CreateTime")); } }
        private DateTime _UpdateTime; 
        public DateTime UpdateTime { get { return _UpdateTime; } set { _UpdateTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("UpdateTime")); } }
        private int _LabelStatus; 
        public int LabelStatus { get { return _LabelStatus; } set { _LabelStatus = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LabelStatus")); } }
        private string _LabelUserID; 
        public string LabelUserID { get { return _LabelUserID; } set { _LabelUserID = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LabelUserID")); } }
        private DateTime _LabelTime; 
        public DateTime LabelTime { get { return _LabelTime; } set { _LabelTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LabelTime")); } }
        private int _ResponseStatus; 
        public int ResponseStatus { get { return _ResponseStatus; } set { _ResponseStatus = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("ResponseStatus")); } }
        private DateTime _ResponseTime; 
        public DateTime ResponseTime { get { return _ResponseTime; } set { _ResponseTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("ResponseTime")); } }

        public int Index { get; set; }
    }
    /// <summary>
    /// 系统信息
    /// </summary>
    public class SysInfoNotify : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 是否打开串口
        /// </summary>
        private bool _IsOpened; 
        /// <summary>
        /// 是否打开串口
        /// </summary>
        public bool IsOpened { get { return _IsOpened; } set { _IsOpened = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("IsOpened")); } }
        /// <summary>
        /// 系统状态
        /// </summary>
        private int _SysStatus; 
        /// <summary>
        /// 系统状态
        /// </summary>
        public int SysStatus
        {
            get { return _SysStatus; }
            set
            {
                var old = _SysStatus;
                _SysStatus = value;
                if (old>=0 && value < 0) {
                    Global.LabelController.CancelLabelList();
                } else if(old < 0 && value >= 0)
                {
                    Global.LabelController.SendLabelList();
                }
                if (PropertyChanged == null) return; 
                PropertyChanged(this, new PropertyChangedEventArgs("SysStatus"));
            }
        }
        /// <summary>
        /// 下位机状态
        /// </summary>
        private int _MachineStatus; 
        /// <summary>
        /// 下位机状态 0空闲 1工作中 -1忙碌
        /// </summary>
        public int MachineStatus { get { return _MachineStatus; } set { _MachineStatus = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("MachineStatus")); } }
        /// <summary>
        /// 自动读卡
        /// </summary>
        private bool _AutoReadCard; 
        /// <summary>
        /// 自动读卡
        /// </summary>
        public bool AutoReadCard { get { return _AutoReadCard; } set { _AutoReadCard = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("AutoReadCard")); } }
        /// <summary>
        /// 打印贴标
        /// </summary>
        private bool _PrintAndLabel; 
        /// <summary>
        /// 打印贴标
        /// </summary>
        public bool PrintAndLabel
        {
            get { return _PrintAndLabel; }
            set
            {
                _PrintAndLabel = value; if (PropertyChanged == null) return;
                PropertyChanged(this, new PropertyChangedEventArgs("PrintAndLabel"));
            }
        }
        /// <summary>
        /// 剩余等待时间
        /// </summary>
        private uint _RemainingTime;
        /// <summary>
        /// 剩余等待时间
        /// </summary>
        public uint RemainingTime { get { return _RemainingTime; } set { _RemainingTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("RemainingTime")); } }
        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime _StartTime; 
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get { return _StartTime; } set { _StartTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("StartTime")); } }
        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime _EndTime; 
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get { return _EndTime; } set { _EndTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("EndTime")); } }
        //private int _LabelListCount; public int LabelListCount { get { return _LabelListCount; } set { _LabelListCount = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("LabelListCount")); } }

    }
    public class QueueInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 清单号
        /// </summary>
        private byte _Id;
        /// <summary>
        /// 清单号
        /// </summary>
        public byte Id { get { return _Id; } set { _Id = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Id")); } }
        /// <summary>
        /// 数量
        /// </summary>
        private byte _Nums;
        /// <summary>
        /// 数量
        /// </summary>
        public byte Nums { get { return _Nums; } set { _Nums = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Nums")); } }
        /// <summary>
        /// 命令id
        /// </summary>
        private byte _CommId;
        /// <summary>
        /// 命令id
        /// </summary>
        public byte CommId { get { return _CommId; } set { _CommId = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("CommId")); } }
        /// <summary>
        /// 状态
        /// </summary>
        private byte _Status;
        /// <summary>
        /// 状态 0创建 1已确认 2已开始 255已取消 254已超时 253重试
        /// </summary>
        public byte Status { get { return _Status; } set { _Status = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Status")); } }
        /// <summary>
        /// 询问时间
        /// </summary>
        private DateTime _AskTime;
        /// <summary>
        /// 询问时间
        /// </summary>
        public DateTime AskTime { get { return _AskTime; } set { _AskTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("AskTime")); } }
        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime _CreateTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get { return _CreateTime; } set { _CreateTime = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("CreateTime")); } }

        /// <summary>
        /// 等待时间
        /// </summary>
        private uint _Remaining;
        /// <summary>
        /// 等待时长
        /// </summary>
        public uint Remaining
        {
            get { return _Remaining; }
            set { _Remaining = value; 
                if (PropertyChanged == null) return; 
                PropertyChanged(this, new PropertyChangedEventArgs("Remaining")); 
                PropertyChanged(this, new PropertyChangedEventArgs("RemainingTime")); 
            }
        }
        /// <summary>
        /// 等待时间
        /// </summary>
        public DateTime RemainingTime
        {
            get
            {
                return _AskTime.AddSeconds(_Remaining);
            }
        }
    }
    public partial class BinStatusInfo:INotifyPropertyChanged
    {
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 仓号
        /// </summary>
        private byte _BinId;
        /// <summary>
        /// 仓号
        /// </summary>
        public byte BinId { get { return _BinId; } set { _BinId = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("BinId")); } }
        /// <summary>
        /// 仓号
        /// </summary>
        private string _ID;
        /// <summary>
        /// 仓号
        /// </summary>
        public string ID { get { return _ID; } set { _ID = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("ID")); } }
        /// <summary>
        /// 颜色
        /// </summary>
        private string _Name;
        /// <summary>
        /// 颜色
        /// </summary>
        public string Name { get { return _Name; } set { _Name = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Name")); } }
        /// <summary>
        /// 厂家颜色
        /// </summary>
        private string _HID;
        /// <summary>
        /// 厂家颜色
        /// </summary>
        public string HID { get { return _HID; } set { _HID = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("HID")); } }
        /// <summary>
        /// 数量
        /// </summary>
        private byte _Nums;
        /// <summary>
        /// 数量
        /// </summary>
        public byte Nums { get { return _Nums; } set { _Nums = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Nums"));
                PropertyChanged(this, new PropertyChangedEventArgs("IsEmpty"));
            } }

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty { get { return _Nums<=0; } }

        /// <summary>
        /// 仓号
        /// </summary>
        private int _TubeColor;
        /// <summary>
        /// 仓号
        /// </summary>
        public int TubeColor { get { return _TubeColor; } set { _TubeColor = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("TubeColor")); } }

        /// <summary>
        /// 命令ID
        /// </summary>
        public byte CommId { get; set; }
    }
    /// <summary>
    /// 串口状态颜色转换
    /// </summary>
    public class OpenColorConvert : IValueConverter
    {
        public static Color Red = Color.FromArgb(255,255,0,0);
        public static Color Lime = Color.FromArgb(255,0,255,0);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value==null||!((bool)value)?Red:Lime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    /// <summary>
    /// 布尔型反转
    /// </summary>
    public class BoolFalseConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || !((bool)value);
        }
    }
    /// <summary>
    /// 系统状态转换
    /// </summary>
    public class SysStatusConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return "空闲";
            }
            if ((int)value == -1)
            {
                return "暂停";
            }
            return "运行中";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
    /// <summary>
    /// 下位机状态转换
    /// </summary>
    public class MachineStatusConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return "空闲";
            }
            if ((int)value == -1)
            {
                return "忙碌";
            }
            return "工作中";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    /// <summary>
    /// 贴标状态转换
    /// </summary>
    public class TubeStatusConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return "等待贴标";
            }
            if ((int)value == -0xd1)
            {
                return "缺管";
            }
            if ((int)value == -0xd2)
            {
                return "复核不匹配";
            }
            if ((int)value == -0xd3)
            {
                return "无条码";
            }
            if ((int)value == -0xd4)
            {
                return "巡边故障";
            }
            if ((int)value == -0xa1)
            {
                return "掉管";
            }
            if ((int)value == -0xa2)
            {
                return "主辊压空";
            }
            if ((int)value == -0xa3)
            {
                return "压辊不到位";
            }
            if ((int)value == -0xa4)
            {
                return "压辊不归位";
            }
            if ((int)value == 1)
            {
                return "已发送";
            }
            if ((int)value == 10)
            {
                return "已接收";
            }
            if ((int)value == 100)
            {
                return "贴标完成";
            }
            return "正在贴标";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    /// <summary>
    /// 剩余时间转换
    /// </summary>
    public class SecondConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value+"";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    /// <summary>
    /// int转颜色
    /// </summary>
    public class ColorInt2Color : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = System.Drawing.Color.FromArgb((int)value);

            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }

    /// <summary>
    /// 系统状态图片
    /// </summary>
    public class SysStatus2Image : IValueConverter
    {
        public static ImageSource ImgPause = new BitmapImage(new Uri("pack://application:,,,/Images/工具栏/暂停48.ico"));
        public static ImageSource ImgPlay = new BitmapImage(new Uri("pack://application:,,,/Images/工具栏/继续48.ico"));
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value < 0) {
                return ImgPlay;
            }
            return ImgPause;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
    /// <summary>
    /// 系统状态按钮文字
    /// </summary>

    public class SysStatus2BtnTxt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value < 0)
            {
                return "继续";
            }
            if ((int)value >= 0)
            {
                return "暂停";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
    /// <summary>
    /// 仅打印文字
    /// </summary>
    public class OnlyPrintConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value==null || !(bool)value)
            {
                return "打印贴标";
            }
            return "仅打印";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }

    /// <summary>
    /// 缺管正常
    /// </summary>
    public class EmptyBoolConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(bool)value)
            {
                return "正常";
            }
            return "缺管";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }

    /// <summary>
    /// 是否测试
    /// </summary>
    public class IsTestConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value==null || !(bool)value)
            {
                return "自动贴标";
            }
            return "批量打印";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
    /// <summary>
    /// 隐藏边栏文本
    /// </summary>
    public class IsHideLeftTextConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value==null || !(bool)value)
            {
                return "◀️";
            }
            return "▶️";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
    /// <summary>
    /// 隐藏边栏dock
    /// </summary>
    public class IsHideLeftVisConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value==null || !(bool)value)
            {
                return System.Windows.Visibility.Visible;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }

    /// <summary>
    /// 隐藏文本
    /// </summary>
    public class TxtHideConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value+""!="")
            {
                return System.Windows.Visibility.Visible;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }


    /// <summary>
    /// 列表转多行文本
    /// </summary>
    public class StringListConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) {
                return "";
            }
            return string.Join("\r\n", (ObservableCollection<string>)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    /// <summary>
    /// 列表转最后一行文本
    /// </summary>
    public class StringListLastConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ObservableCollection<string>)value)?.LastOrDefault()??"";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

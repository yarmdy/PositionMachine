using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// MachineSettings.xaml 的交互逻辑
    /// </summary>
    public partial class MachineSettings : Page, IPageClose
    {
        private MachineSetting _binding = null;
        private string _fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MachineSettings.config");
        private string _paramFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MachineParams.config");
        string tabName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        public MachineSettings()
        {
            InitializeComponent();
            initBinding();
            initEvent();
            var binNums = int.Parse(Config.Configs.Settings["BinNum"]);
            var index = 0;
            cbBins.ItemsSource = new byte[binNums].Select(a => $"{++index}号仓").ToList();
            cbBins.SelectedIndex = 0;

            Global.LPM.StartLightTest(SerialPort.LabelMachineHelper.EnumOpenClose.OPEN);

            
            //Global.LPM.
        }

        
        private Dictionary<int, bool> convertInt2Light(uint val) {
            var index = 0;
            return new int[32].Select(a => {
                var i = index++;
                var cval = ((uint)val) << (32 - i - 1) >> 31;
                var res = cval > 0;
                return new { id = i, val = res };
            }).ToDictionary(a => a.id, b => b.val);
        }
        private void LPM_OnLightStatus(SerialPort.DataPackage obj)
        {
            var data = BitConverter.ToUInt32(obj.Data, 0);
            var dic = convertInt2Light(data);
            foreach (var item in dic) {
                var light = _binding.Lights.SelectMany(a => a.List).FirstOrDefault(a => a.Index == item.Key);
                if (light == null)
                {
                    continue;
                }
                light.IsOn = item.Value;
            }
        }
        private void LPM_OnUpParam(SerialPort.DataPackage obj)
        {
            var len = obj.Data.Length/2;
            var index = 0;
            var dic = new int[len].Select(a => {
                var i = index++;
                var val = BitConverter.ToUInt16(obj.Data, i * 2);
                return new { id = i, val = val };
            }).ToDictionary(a => a.id, b => b.val);
            foreach (var item in dic) {
                var param = _binding.Params.SelectMany(a => a.List).FirstOrDefault(a => a.Index == item.Key);
                if (param == null)
                {
                    continue;
                }
                param.Value = item.Value;
            }
            Global.MainWindow.Dispatcher.Invoke(()=> UI.Popup.Succ(Global.MainWindow, "读取下位机参数成功"));
        }

        private void initEvent() {

            Global.LPM.OnLightStatus += LPM_OnLightStatus;
            Global.LPM.OnUpParam += LPM_OnUpParam;

            Global.LPM.OnBackSaveParams += LPM_OnBackSaveParams;
            Global.LPM.OnBackApplyParams += LPM_OnBackApplyParams;

            Global.LPM.OnUpMotorSteps += LPM_OnUpMotorSteps;
        }

        private void LPM_OnUpMotorSteps(SerialPort.DataPackage obj)
        {
            _binding.MotorSteps = BitConverter.ToInt16(obj.Data,0);
        }

        private void LPM_OnBackApplyParams(SerialPort.DataPackage obj)
        {
            Global.MainWindow.Dispatcher.Invoke(() => {
                if (obj.Data[0] == 1)
                {
                    UI.Popup.Succ(Global.MainWindow, "与下位机同步参数成功");
                    return;
                }
                UI.Popup.Error(Global.MainWindow, "与下位机同步参数失败");
            });
        }

        private void LPM_OnBackSaveParams(SerialPort.DataPackage obj)
        {
            Global.MainWindow.Dispatcher.Invoke(() => {
                if (obj.Data[0] == 1)
                {
                    UI.Popup.Succ(Global.MainWindow, "保存参数到下位机成功");
                    return;
                }
                UI.Popup.Error(Global.MainWindow, "保存参数到下位机失败");
            });
            
        }

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }
        private void initBinding() {
            var configStr = "{}";
            if (!System.IO.File.Exists(_fileName)) {
                UILog.Error("下位机配置文件不存在", null);
                goto error;
            }
            try {
                configStr = System.IO.File.ReadAllText(_fileName);
                _binding = Newtonsoft.Json.JsonConvert.DeserializeObject<MachineSetting>(configStr);
                if (_binding.Lights == null)
                {
                    _binding.Lights = new List<MachineSettingGroup<MachineLightStatus>>();
                }
                if (_binding.Params == null)
                {
                    _binding.Params = new List<MachineSettingGroup<MachineParam>>();
                }
                if (_binding.Actions == null)
                {
                    _binding.Actions = new List<MachineSettingGroup<MachineSettingGroup<MachineAction>>>();
                }
                if (_binding.Bins == null)
                {
                    _binding.Bins = new List<MachineSettingGroup<MachineAction>>();
                }
            } catch (Exception ex) {
                UILog.Error("读取下位机配置文件失败", ex);
                goto error;
            }
            goto finish;
        error:
            _binding = new MachineSetting();
            _binding.Lights = new List<MachineSettingGroup<MachineLightStatus>>();
            _binding.Params = new List<MachineSettingGroup<MachineParam>>();
            _binding.Actions = new List<MachineSettingGroup<MachineSettingGroup<MachineAction>>>();
            _binding.Bins = new List<MachineSettingGroup<MachineAction>>();
        finish:
            DataContext = _binding;
        }

        private void tbReadMachine_Click(object sender, RoutedEventArgs e)
        {
            Global.LPM.ReadParam();
        }

        private (byte len, ushort[] array) getLocalParams() {
            var paramArr = _binding.Params.SelectMany(a=>a.List).ToList();
            var len = (byte)(paramArr.Max(a=>a.Index)+1);
            var index = 0;
            var array = new ushort[len].Select(a => {
                var i = index++;
                return paramArr.FirstOrDefault(b => b.Index == i)?.Value ?? 0;
            }).ToArray();

            return (len, array);
        }
        private void tbRefresh_Click(object sender, RoutedEventArgs e)
        {
            var data=getLocalParams();
            Global.LPM.ApplyParam(data.array);

        }

        private void tbSaveMachine_Click(object sender, RoutedEventArgs e)
        {
            var data = getLocalParams();
            Global.LPM.SaveParam(data.array);
        }

        private void tbReadFile_Click(object sender, RoutedEventArgs e)
        {
            if (!System.IO.File.Exists(_paramFileName))
            {
                goto error;
            }
            try {
                var txt = System.IO.File.ReadAllText(_paramFileName);
                var fileParams = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MachineParam>>(txt);
                _binding.Params.SelectMany(a => a.List).ToList().ForEach(a => {
                    var fileParam = fileParams.FirstOrDefault(b => b.Index == a.Index);
                    if (fileParam == null) {
                        return;
                    }
                    a.Value = fileParam.Value;
                });
            } catch (Exception ex) {
                UILog.Error("读取参数文件失败",ex);
            }
            UI.Popup.Succ(Global.MainWindow, "读取文件成功");
            return;

            error:
            UI.Popup.Succ(Global.MainWindow, "读取文件失败");
        }

        private void tbSaveFile_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.WriteAllText(_paramFileName, Newtonsoft.Json.JsonConvert.SerializeObject(_binding.Params.SelectMany(a => a.List).ToList(), Newtonsoft.Json.Formatting.Indented));
            UI.Popup.Succ(Global.MainWindow,"保存到文件成功");
        }

        public void PageClose()
        {
            Global.LPM.StartLightTest(SerialPort.LabelMachineHelper.EnumOpenClose.CLOSE);
            Global.LPM.OnLightStatus -= LPM_OnLightStatus;
            Global.LPM.OnUpParam -= LPM_OnUpParam;

            Global.LPM.OnBackSaveParams -= LPM_OnBackSaveParams;
            Global.LPM.OnBackApplyParams -= LPM_OnBackApplyParams;

            Global.LPM.OnUpMotorSteps -= LPM_OnUpMotorSteps;
        }

        private void MotorTestButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var act = (MachineAction)(btn.DataContext);

            if (act.Name2 == null)
            {
                Global.LPM.TestMachine((byte)act.Index, act.Action);
                goto after;
            }

            if (btn.Tag + "" == "")
            {
                Global.LPM.TestMachine((byte)act.Index, act.Action);
                btn.Tag = "1";
                btn.Content = act.Name2;
                goto after;
            }
            else {
                Global.LPM.TestMachine((byte)act.Index, act.Action2);
                btn.Tag = "";
                btn.Content = act.Name;
                goto after;
            }

        after:
            if (string.IsNullOrWhiteSpace(act.After))
            {
                return;
            }
            var mthd = this.GetType().GetMethod(act.After,System.Reflection.BindingFlags.NonPublic| System.Reflection.BindingFlags.Public| System.Reflection.BindingFlags.Instance);
            mthd.Invoke(this, new object[] { act });
        }
        private void BinTestButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var act = (MachineAction)(btn.DataContext);
            if (act.Name2 == null)
            {
                Global.LPM.TestBin((byte)(cbBins.SelectedIndex+1), act.Action);
                goto after;
            }

            if (btn.Tag + "" == "")
            {
                Global.LPM.TestBin((byte)(cbBins.SelectedIndex+1), act.Action);
                btn.Tag = "1";
                btn.Content = act.Name2;
                goto after;
            }
            else
            {
                Global.LPM.TestBin((byte)(cbBins.SelectedIndex+1), act.Action2);
                btn.Tag = "";
                btn.Content = act.Name;
                goto after;
            }
        after:
            if (string.IsNullOrWhiteSpace(act.After))
            {
                return;
            }
            var mthd = this.GetType().GetMethod(act.After, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            mthd.Invoke(this, new object[] { act });
        }

        private void ParamTestButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var sp = ElementHelper.GetParent<StackPanel>(btn);
            if (sp == null)
            {
                return;
            }
            var txts = ElementHelper.GetChildren<TextBox>(sp);
            if (txts.Count <= 0)
            {
                return;
            }
            var txt = txts[0];
            var act = (MachineAction)(btn.DataContext);
            short tmp = 0;
            short.TryParse(txt.Text,out tmp);

            if (act.Name2 == null)
            {
                Global.LPM.TestMotor(act.Action, tmp);
                goto after;
            }

            if (btn.Tag + "" == "")
            {
                Global.LPM.TestMotor(act.Action, tmp);
                btn.Tag = "1";
                btn.Content = act.Name2;
                goto after;
            }
            else
            {
                Global.LPM.TestMotor(act.Action2, tmp);
                btn.Tag = "";
                btn.Content = act.Name;
                goto after;
            }

        after:
            if (string.IsNullOrWhiteSpace(act.After))
            {
                return;
            }
            var mthd = this.GetType().GetMethod(act.After, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            mthd.Invoke(this, new object[] { act });
        }

        private void binReset(MachineAction act) 
        {
            var btns = ElementHelper.GetChildren<Button>(lvBins);
            btns.ForEach(a => {
                if (!(a.DataContext is MachineAction)) return;
                var act2 = a.DataContext as MachineAction;
                a.Content = act2.Name;
                a.Tag = "";
            });
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ElementHelper.GetChildren<ListView>(sc_sc).ForEach(a => ElementHelper.UseTheScrollViewerScrolling(a));
        }

        private void ListView_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }

    public class MachineSetting: INotifyPropertyChanged
    {
        public List<MachineSettingGroup<MachineLightStatus>> Lights { get; set; }
        public List<MachineSettingGroup<MachineParam>> Params { get; set; }
        public List<MachineSettingGroup<MachineSettingGroup<MachineAction>>> Actions { get; set; }
        public List<MachineSettingGroup<MachineAction>> Bins { get; set; }

        private short _MotorSteps;

        public event PropertyChangedEventHandler PropertyChanged;

        public short MotorSteps
        {
            get { return _MotorSteps; }
            set { _MotorSteps = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("MotorSteps")); }
        }
    }
    public class MachineSettingGroup<T>
    {
        public string Name { get; set; }
        public List<T> List { get; set; }
    }
    public class MachineSettingItem
    {
        public string Name { get; set; }
        public int Index { get; set; }

        public List<MachineAction> Buttons { get; set; }
    }
    public class MachineLightStatus : MachineSettingItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _IsOn;
        public bool IsOn
        {
            get { return _IsOn; }
            set { _IsOn = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("IsOn")); }
        }
    }
    public class MachineParam : MachineSettingItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ushort _Value;
        public ushort Value
        {
            get { return _Value; }
            set { _Value = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Value")); }
        }
    }
    public class MachineAction : MachineSettingItem
    {
        public byte Action { get; set; }
        public string Name2 { get; set; }
        public byte Action2 { get; set; }

        public string After { get; set; }
    }

    public class ElementHelper {
        public static List<T> GetChildren<T>(DependencyObject obj) where T : class
        {
            var res = new List<T>();
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var local = VisualTreeHelper.GetChild(obj, i);
                if (local is T)
                {
                    res.Add(local as T);
                    continue;
                }
                res.AddRange(GetChildren<T>(local));
            }
            return res;
        }

        public static T GetParent<T>(DependencyObject obj) where T : class
        {
            var parent = VisualTreeHelper.GetParent(obj);
            if (parent == null)
            {
                return null;
            }
            if (parent.GetType() == typeof(T))
            {
                return parent as T;
            }
            return GetParent<T>(parent);
        }
        public static void UseTheScrollViewerScrolling(FrameworkElement fElement)
        {
            fElement.PreviewMouseWheel += (sender, e) =>
            {
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                fElement.RaiseEvent(eventArg);
            };
        }
    }
}

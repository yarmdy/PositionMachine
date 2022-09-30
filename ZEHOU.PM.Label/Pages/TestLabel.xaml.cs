using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ZEHOU.PM.Command;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// TestLabel.xaml 的交互逻辑 自动贴标
    /// </summary>
    public partial class TestLabel : Page,IPageClose
    {
        string tabName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        private TestLabelModel _model=null;
        public TestLabel()
        {
            InitializeComponent();
            
            tbPause.DataContext = Global.BindingInfo;
            _model = new TestLabelModel();
            var index = 0;
            _model.BinList = new byte[int.Parse(Config.Configs.Settings["BinNum"])].Select(a => {
                var i = (byte)++index;
                return new ListNameValueCheck { Name=$"{i}号仓",Value=i,Checked=i==1};
            }).ToList();
            index = 0;
            _model.NumsList = new byte[100].Select(a => {
                var i = (byte)++index;
                return new ComboxNameValue { Name = $"{i}", Value = i };
            }).ToList();
            _model.LS01 = "82345678902";
            _model.LS02 = "血生化";
            _model.LS03 = "张三丰 男 29";
            _model.LS04 = "86677";
            _model.LS05 = "糖类抗原50+糖类抗原";
            _model.Nums = 1;
            DataContext = _model;
            
        }

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }


        private void tbPause_Click(object sender, RoutedEventArgs e)
        {
            Global.BindingInfo.SysInfo.SysStatus = Global.BindingInfo.SysInfo.SysStatus < 0 ? 0 : -1;
            
        }


        public void PageClose()
        {
            tbPause.DataContext = null;
        }

        private ObservableCollection<LabelInfoNotify> createLabels(byte count, bool onlyPrint) {
            var deviceBll = new Bll.Device();
            //var tubeType = deviceBll.GetTubeTypes(Config.Configs.Settings["DeviceId"]).Where(a => a.IsUse).ToList();
            ObservableCollection<LabelInfoNotify> list = new ObservableCollection<LabelInfoNotify>();
            new byte[count].ToList().ForEach(a => {
                var patientL = new PatientInfoNotify { ID = $"{0}", Name = "贴标测试", Age = 0, Gender = "" };
                var tinfo = new TubeInfoNotify { BarCode = _model.LS01, PatientID = $"0", TubeColor = "", TestGroup = "", TestOrder = "", LS01 = _model.LS01, LS02 = _model.LS02, LS03 = _model.LS03, LS04 = _model.LS04, LS05 = _model.LS05, LS06 = _model.LS06, LS07 = _model.LS07, LS08 = _model.LS08, LS09 = _model.LS09, LS10 = _model.LS10 };
                var item = new LabelInfoNotify { Patient = patientL, TubeInfo = tinfo, IsChecked = true, IsTest = true };

                //var ttype = tubeType.FirstOrDefault(b => b.BinID.Split(new[] { ',' }).Contains(_model.BinId+""));

                item.BinId = _model.BinId;
                item.TubeColor = Global.TubeColorDic.G("未知");
                list.Add(item);
            });
            return list;
        }
        private void btnAutoLabel_Click(object sender, RoutedEventArgs e)
        {
            var list = createLabels(1,false);
            Global.LabelController.AddToQueue(list,false);
        }

        private void OnlyPrint_Click(object sender, RoutedEventArgs e)
        {
            var list = createLabels(1, true);
            Global.LabelController.AddToQueue(list,true);
        }

        private void btnContinuousPrinting_Click(object sender, RoutedEventArgs e)
        {
            var list = createLabels(_model.Nums, true);
            Global.LabelController.AddToQueue(list, false);
        }
    }
    public class TestLabelModel
    {
        public List<ListNameValueCheck> BinList { get; set; }
        public string BinId { get {
                return string.Join(",", BinList.Where(a=>a.Checked).Select(a=>a.Value));
            } }
        public string LS01 { get; set; }
        public string LS02 { get; set; }
        public string LS03 { get; set; }
        public string LS04 { get; set; }
        public string LS05 { get; set; }
        public string LS06 { get; set; }
        public string LS07 { get; set; }
        public string LS08 { get; set; }
        public string LS09 { get; set; }
        public string LS10 { get; set; }
        public List<ComboxNameValue> NumsList { get; set; }
        public byte Nums { get; set; }
    }

    public class ComboxNameValue
    {
        public string Name { get; set; }
        public byte Value { get; set; }
    }

    public class ListNameValueCheck
    {
        public string Name { get; set; }
        public byte Value { get; set; }
        public bool Checked { get; set; }
    }
}

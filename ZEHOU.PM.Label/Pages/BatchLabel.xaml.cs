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
    /// BatchLabel.xaml 的交互逻辑 自动贴标
    /// </summary>
    public partial class BatchLabel : Page,IPageClose
    {
        string tabName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        private BatchBinding binding=null;
        public BatchLabel()
        {
            InitializeComponent();
            
            tbPause.DataContext = Global.BindingInfo;
            lbStatus.DataContext = Global.BindingInfo;
            lbMCStatus.DataContext = Global.BindingInfo;

            binding = new BatchBinding();
            binding.PrintAndLabel = true;
            binding.LocalLabelList = new ObservableCollection<LabelInfoNotify>();
            DataContext = binding;
        }

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }


        private void tbPause_Click(object sender, RoutedEventArgs e)
        {
            Global.BindingInfo.SysInfo.SysStatus = Global.BindingInfo.SysInfo.SysStatus < 0 ? 0 : -1;
            
        }

        private void tbInputCode_Click(object sender, RoutedEventArgs e)
        {
            var dia=new BatchLabelDialog();
            dia.ShowDialog();
            if (dia.Result == null)
            {
                return;
            }
            binding.LocalLabelList.Clear();

            var deviceBll = new Bll.Device();

            var tubeType = deviceBll.GetTubeTypes(Config.Configs.Settings["DeviceId"]).Where(a => a.IsUse).ToList();
            dia.Result.ForEach(a => {
                var patientL = new PatientInfoNotify { ID=$"{a.Id}",Name=a.PName,Age=0,Gender=""};
                var tinfo  = new TubeInfoNotify { BarCode=a.BarCode,PatientID=$"{a.Id}",TubeColor=a.TubeColor,TestGroup="",TestOrder= "", LS01=a.BarCode,LS02=$"{a.Remark}",LS03= $"{a.PName}",LS04=$"{a.Id}",LS05=$"{a.Remark}",LS06="", LS07 = "", LS08 = "", LS09 = "", LS10 = "" };
                var item = new LabelInfoNotify { Patient = patientL, TubeInfo = tinfo, IsChecked = true,IsTest=true };
                var ttype = tubeType.FirstOrDefault(b => b.HID.Split(new[] { ',' }).Contains(a.TubeColor));

                item.BinId = ttype?.BinID ?? "";
                if (string.IsNullOrWhiteSpace(item.BinId))
                {
                    item.TubeColor = Global.TubeColorDic.G(tubeType.FirstOrDefault(b => b.ID == "00")?.Name ?? "");
                }
                else
                {
                    item.TubeColor = Global.TubeColorDic.G(ttype?.Name ?? "");
                }
                binding.LocalLabelList.Add(item);
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Global.LabelController.AddToQueue(binding.LocalLabelList,!binding.PrintAndLabel);
        }

        private void tbPrintBack2_Click(object sender, RoutedEventArgs e)
        {
            binding.LocalLabelList.Clear();
        }

        public void PageClose()
        {
            tbPause.DataContext = null;
            lbStatus.DataContext = null;
            lbMCStatus.DataContext = null;
        }
    }

    public class BatchBinding : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 打印贴标
        /// </summary>
        private bool _PrintAndLabel;
        /// <summary>
        /// 打印贴标
        /// </summary>
        public bool PrintAndLabel { get { return _PrintAndLabel; } set { _PrintAndLabel = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PrintAndLabel")); } }

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
    }
}

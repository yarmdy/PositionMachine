using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// TubeTypeManager.xaml 的交互逻辑
    /// </summary>
    public partial class TubeTypeManager : Page
    {
        string tabName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        List<DB.dbLabelInfo.TubeType> tubeTypeList = null;
        public TubeTypeManager()
        {
            InitializeComponent();
            loadData();
        }

        private void loadData() {
            var deviceBll = new Bll.Device();
            tubeTypeList = deviceBll.GetTubeTypes(Config.Configs.Settings["DeviceID"]);
            dgData.ItemsSource = null;
            dgData.ItemsSource = tubeTypeList;
        }

        

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }


        private void tbEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem == null || dgData.SelectedItem.GetType() != typeof(DB.dbLabelInfo.TubeType) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.TubeType))
            {
                UI.Popup.Error(Global.MainWindow, "请选择要修改的数据");
                return;
            }
            var tubeType = (DB.dbLabelInfo.TubeType)dgData.SelectedItem;
            var dialog = new EditTubeType(tubeType.ID);
            var res = dialog.ShowDialog();
            if (!(res ?? false)) {
                return;
            }
            loadData();
        }

        private void EditColor_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext == null || ((Button)sender).DataContext.GetType().BaseType != typeof(ZEHOU.PM.DB.dbLabelInfo.TubeType) && ((Button)sender).DataContext.GetType() != typeof(ZEHOU.PM.DB.dbLabelInfo.TubeType)) {
                return;
            }
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            var obj = ((ZEHOU.PM.DB.dbLabelInfo.TubeType)((Button)sender).DataContext);
            
            var colorName = obj.Name;
            dlg.Color = System.Drawing.Color.FromArgb(Global.TubeColorDic.G(colorName));
            var res = dlg.ShowDialog();
            if (res != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            
            
            Global.TubeColorDic[colorName]=dlg.Color.ToArgb();
            SaveTubeColorDic();
            foreach (var item in Global.BindingInfo.LocalLabelList) {
                if (item.BinId != obj.BinID || !obj.IsUse || obj.BinID == "" && obj.ID != "00") continue;
                item.TubeColor=dlg.Color.ToArgb();
            }
            foreach (var item in Global.BindingInfo.LabelQueue)
            {
                if (item.BinId != obj.BinID || !obj.IsUse || obj.BinID=="" && obj.ID!="00") continue;
                item.TubeColor = dlg.Color.ToArgb();
            }
            loadData();
        }

        private void SaveTubeColorDic()
        {
            System.IO.File.WriteAllText(Global.TubeColorConfigFile, Newtonsoft.Json.JsonConvert.SerializeObject(Global.TubeColorDic, Newtonsoft.Json.Formatting.Indented));
        }

    }

    public class ColorName2Color : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorv = Global.TubeColorDic.G(value+"");
            var color = System.Drawing.Color.FromArgb(colorv);

            return Color.FromArgb(color.A,color.R,color.G,color.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}

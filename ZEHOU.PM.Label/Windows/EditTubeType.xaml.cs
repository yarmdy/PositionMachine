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
using System.Windows.Shapes;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// EditTubeType.xaml 的交互逻辑
    /// </summary>
    public partial class EditTubeType : ZHWindow
    {
        DB.dbLabelInfo.TubeType _tubeType = null;
        bool isNew = false;
        List<BinSelectInfo> _selectInfo = null;
        public EditTubeType(string id)
        {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(id))
            {
                _tubeType = new DB.dbLabelInfo.TubeType { IsUse = true, ID = "", Name = "", OrderID = 0, BinID = "", DeviceID = Config.Configs.Settings["DeviceId"],HID="" };
                btnAdd.Content = "添 加";
                this.Title = "添加新试管类型";
                isNew = true;
            }
            else {
                var deviceBll = new Bll.Device();
                _tubeType = deviceBll.GetTubeTypeById(Config.Configs.Settings["DeviceId"],id);
                if (_tubeType == null)
                {
                    UI.Popup.Error(this,"试管类型不存在，无法修改");
                    Close();
                    return;
                }
                isNew = false;
                txtID.IsReadOnly = true;
                txtName.IsReadOnly = true;
                btnAdd.Content = "修 改";
                this.Title = "修改试管类型";
            }
            int binId = 0;
            int sBinId;
            var binIds = _tubeType.BinID.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(a => int.TryParse(a, out sBinId)).Select(int.Parse).ToArray();
            _selectInfo = new int[int.Parse(Config.Configs.Settings["BinNum"])].Select(a=>new BinSelectInfo { Id=++binId,Name=$"{binId}号",IsSelected=binIds.Contains(binId)}).ToList();
            lvBinID.ItemsSource = _selectInfo;
            this.DataContext = _tubeType;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_tubeType.ID)) {
                UI.Popup.Error(this,"编号不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(_tubeType.Name)) {
                UI.Popup.Error(this,"名称不能为空");
                return;
            }
            _tubeType.ID = _tubeType.ID.Trim();
            _tubeType.Name = _tubeType.Name.Trim();
            _tubeType.BinID = getBinIds();
            int ret;
            var deviceBll = new Bll.Device();
            if (isNew) {
                ret=deviceBll.AddTubeType(_tubeType);
                if (ret == -1)
                {
                    UI.Popup.Error(this, "编号已存在");
                    return;
                }
                if (ret <= 0)
                {
                    UI.Popup.Error(this, "添加失败");
                }
                this.DialogResult = true;
                UI.Popup.Succ(this, "添加成功");
                Close();
                return;
            }
            ret = deviceBll.EditTubeType(_tubeType);
            if (ret <= 0)
            {
                UI.Popup.Error(this, "修改失败");
            }
            this.DialogResult = true;
            UI.Popup.Succ(this, "修改成功");
            Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var re = new System.Text.RegularExpressions.Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }
        private string getBinIds()
        {
            return string.Join(",", _selectInfo.Where(a => a.IsSelected).Select(a => a.Id));
        }
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class EditTubeTypeStr2Int : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value + "";
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value + "")) return 0;
            int tmp;
            if (!int.TryParse(value + "", out tmp))
            {
                return 0;
            }
            return tmp;
        }
    }

    public class BinSelectInfo { 
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}

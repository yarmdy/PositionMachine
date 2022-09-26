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
    /// EditRole.xaml 的交互逻辑
    /// </summary>
    public partial class EditRole : ZHWindow
    {
        DB.dbLabelInfo.Role _role = null;
        bool isNew = false;
        public EditRole(string id)
        {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(id))
            {
                _role = new DB.dbLabelInfo.Role { IsUse = true,FunctionID="",ID="",Name="",OrderID=0 };
                btnAdd.Content = "添 加";
                this.Title = "添加新角色";
                isNew = true;
            }
            else {
                var loginBll = new Bll.Login();
                _role = loginBll.GetRoleById(id);
                if (_role == null)
                {
                    UI.Popup.Error(this,"角色不存在，无法修改");
                    Close();
                    return;
                }
                isNew = false;
                txtID.IsReadOnly = true;
                btnAdd.Content = "修 改";
                this.Title = "修改角色";
            }
            this.DataContext = _role;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_role.ID)) {
                UI.Popup.Error(this,"编号不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(_role.Name)) {
                UI.Popup.Error(this,"名称不能为空");
                return;
            }
            _role.ID = _role.ID.Trim();
            _role.Name = _role.Name.Trim();
            int ret;
            var loginBll = new Bll.Login();
            if (isNew) {
                ret=loginBll.AddRole(_role);
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
            ret = loginBll.EditRole(_role);
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
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class EditRoleStr2Int : IValueConverter
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
}

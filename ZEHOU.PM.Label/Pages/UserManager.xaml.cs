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

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// UserManager.xaml 的交互逻辑
    /// </summary>
    public partial class UserManager : Page
    {
        string tabName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        List<DB.dbLabelInfo.User> userList = null;
        List<DB.dbLabelInfo.Department> departList = null;
        List<DB.dbLabelInfo.Post> postList = null;
        public static List<DB.dbLabelInfo.Role> S_RoleList = null;
        public UserManager()
        {
            InitializeComponent();
            initCombo();
            loadData();
        }
        private void loadData()
        {
            var loginBll = new Bll.Login();
            userList = loginBll.GetUsers(((DB.dbLabelInfo.Department)cbDeparts.SelectedItem)?.ID, ((DB.dbLabelInfo.Post)cbPosts.SelectedItem)?.ID,txtName.Text.Trim());
            dgData.ItemsSource = null;
            dgData.ItemsSource = userList;
        }

        private void initCombo() {
            var loginBll = new Bll.Login();
            departList=loginBll.GetDepartments();
            postList=loginBll.GetPosts();

            departList.Insert(0,new DB.dbLabelInfo.Department { ID=null,Name="全部部门"});
            postList.Insert(0,new DB.dbLabelInfo.Post { ID=null,Name="全部职位"});

            cbDeparts.ItemsSource = departList;
            cbDeparts.SelectedIndex = 0;
            cbPosts.ItemsSource = postList;
            cbPosts.SelectedIndex = 0;
            S_RoleList = loginBll.GetRoles();
        }

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }

        private void tbEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem == null || dgData.SelectedItem.GetType() != typeof(DB.dbLabelInfo.User) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.User))
            {
                UI.Popup.Error(Global.MainWindow, "请选择要修改的数据");
                return;
            }
            var user = (DB.dbLabelInfo.User)dgData.SelectedItem;
            var dialog = new EditUser(user.ID);
            var res = dialog.ShowDialog();
            if (!(res ?? false))
            {
                return;
            }
            loadData();
        }

        private void tbAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddUser();
            var res = dialog.ShowDialog();
            if (!(res ?? false))
            {
                return;
            }
            loadData();
        }

        private void tbDel_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem == null || dgData.SelectedItem.GetType() != typeof(DB.dbLabelInfo.User) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.User))
            {
                UI.Popup.Error(Global.MainWindow, "请选择要删除的数据");
                return;
            }
            var user = (DB.dbLabelInfo.User)dgData.SelectedItem;
            var res = UI.Popup.Confirm(Global.MainWindow, "确定要删除“" + user.TrueName + "”吗？");
            if (res != MessageBoxResult.OK)
            {
                return;
            }
            var loginBll = new Bll.Login();
            var ret = loginBll.DelUser(user.ID);
            if (ret <= 0)
            {
                UI.Popup.Error(Global.MainWindow, "删除失败");
                return;
            }
            UI.Popup.Succ(Global.MainWindow, "删除成功");
            loadData();
        }
        private void tbFixPwd_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem == null || dgData.SelectedItem.GetType() != typeof(DB.dbLabelInfo.User) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.User))
            {
                UI.Popup.Error(Global.MainWindow, "请选择要重置密码的数据");
                return;
            }
            var user = (DB.dbLabelInfo.User)dgData.SelectedItem;
            var dialog = new ResetPassword(user.ID);
            var res = dialog.ShowDialog();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            loadData();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            S_RoleList.Clear();
        }
    }

    public class RoleIdConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Join(",", (value + "").Split(new[] { ","},StringSplitOptions.RemoveEmptyEntries).Where(a=>UserManager.S_RoleList.Any(b=>b.ID==a)).Select(a=>UserManager.S_RoleList.FirstOrDefault(b=>b.ID==a)?.Name??""));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

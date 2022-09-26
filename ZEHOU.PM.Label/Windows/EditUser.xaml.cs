using System;
using System.Collections.Generic;
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
    /// EditUser.xaml 的交互逻辑
    /// </summary>
    public partial class EditUser : ZHWindow
    {
        DB.dbLabelInfo.User _user = null;
        List<CheckBoxRole> _roleList = null;
        List<DB.dbLabelInfo.Department> _departList = null;
        List<DB.dbLabelInfo.Post> _postList = null;
        public EditUser(string id)
        {
            InitializeComponent();
            var loginBll = new Bll.Login();
            _user = loginBll.GetUserById(id);
            if (_user == null) {
                Close();
                return;
            }
            DataContext = _user;
            initCombo();
        }
        private void initCombo() {
            var loginBll = new Bll.Login();
            var  roles = _user.RoleID.Split(new[] { ','},StringSplitOptions.RemoveEmptyEntries);
            _roleList = loginBll.GetRoles().Select(a=>new CheckBoxRole { Select=roles.Contains(a.ID),Role= a }).ToList();
            _departList = loginBll.GetDepartments();
            _postList = loginBll.GetPosts();

            cbDepart.ItemsSource = _departList;
            cbPost.ItemsSource = _postList;
            trRole.ItemsSource = _roleList;

            cbDepart.SelectedItem = _departList.FirstOrDefault(a => a.ID == _user.DepartmentID);
            cbPost.SelectedItem = _postList.FirstOrDefault(a => a.ID == _user.PostID);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrEmpty(_user.TrueName))
            {
                UI.Popup.Error(this, "请输入用户姓名");
                return;
            }

            _user.RoleID = getSelRoles();
            _user.DepartmentID = ((DB.dbLabelInfo.Department)cbDepart.SelectedItem).ID;
            _user.PostID = ((DB.dbLabelInfo.Post)cbPost.SelectedItem).ID;

            var loginBll = new Bll.Login();
            var ret = loginBll.EditUser(_user);
            if (ret <= 0)
            {
                UI.Popup.Error(this, "修改失败");
                return;
            }

            UI.Popup.Succ(this, "修改成功");
            DialogResult = true;
            Close();
        }
        private string getSelRoles() {
            var roles = new List<string>();
            foreach(CheckBoxRole item in trRole.Items)
            {
                if (item.Select) {
                    roles.Add((item.Role).ID);
                }
            }
            return string.Join(",",roles);
        }

        private void btnAddDepart_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditDepartment(null);
            var res = dialog.ShowDialog();
            if (!(res ?? false)) {
                return;
            }
            initCombo();
        }

        private void btnAddPost_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditPost(null);
            var res = dialog.ShowDialog();
            if (!(res ?? false))
            {
                return;
            }
            initCombo();
        }
    }

}

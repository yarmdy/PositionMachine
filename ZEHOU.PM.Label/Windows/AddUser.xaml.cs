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
    /// AddUser.xaml 的交互逻辑
    /// </summary>
    public partial class AddUser : ZHWindow
    {
        DB.dbLabelInfo.User _user = null;
        List<CheckBoxRole> _roleList = null;
        List<DB.dbLabelInfo.Department> _departList = null;
        List<DB.dbLabelInfo.Post> _postList = null;
        public AddUser()
        {
            InitializeComponent();
            _user = new DB.dbLabelInfo.User { IsUse = true, ID = "", LoginName="",TrueName="", Department=null,Note="",Password="",Post=null,RoleID="",Tel=""};
            DataContext = _user;
            initCombo();
        }
        private void initCombo() {
            var loginBll = new Bll.Login();
            _roleList = loginBll.GetRoles().Select(a=>new CheckBoxRole { Select=false,Role= a }).ToList();
            _departList = loginBll.GetDepartments();
            _postList = loginBll.GetPosts();

            cbDepart.ItemsSource = _departList;
            cbPost.ItemsSource = _postList;
            trRole.ItemsSource = _roleList;

            if (cbDepart.SelectedItem == null && cbDepart.Items.Count>0)
            {
                cbDepart.SelectedIndex = 0;
            }
            if (cbPost.SelectedItem==null && cbPost.Items.Count>0)
            {
                cbPost.SelectedIndex = 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_user.ID))
            {
                UI.Popup.Error(this,"请输入编号");
                return;
            }
            if (string.IsNullOrEmpty(_user.LoginName))
            {
                UI.Popup.Error(this, "请输入用户名");
                return;
            }
            if (string.IsNullOrEmpty(_user.TrueName))
            {
                UI.Popup.Error(this, "请输入用户姓名");
                return;
            }
            if (pwdPassword.Password == ""||pwdPassword2.Password=="")
            {
                UI.Popup.Error(this, "请输入密码");
                return;
            }
            if (pwdPassword.Password!=  pwdPassword2.Password )
            {
                UI.Popup.Error(this, "两次密码输入不一致");
                return;
            }

            _user.Password = Command.Crypt.Md5(pwdPassword.Password);
            _user.RoleID = getSelRoles();
            _user.DepartmentID = ((DB.dbLabelInfo.Department)cbDepart.SelectedItem).ID;
            _user.PostID = ((DB.dbLabelInfo.Post)cbPost.SelectedItem).ID;

            var loginBll = new Bll.Login();
            var ret = loginBll.AddUser(_user);
            if (ret == -1)
            {
                UI.Popup.Error(this, "编号已存在");
                return;
            }
            if (ret == -2)
            {
                UI.Popup.Error(this, "用户名已存在");
                return;
            }

            UI.Popup.Succ(this, "添加成功");
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

    public class CheckBoxRole
    {
        public bool Select { get; set; }
        public DB.dbLabelInfo.Role Role { get; set; }
    }
}

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
    /// ChangePassword.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePassword : ZHWindow
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var old = pwdOld.Password;
            var @new = pwdNew.Password;
            var confirm = pwdConfirm.Password;
            if (old == ""||@new==""||confirm=="")
            {
                UI.Popup.Error(this,"密码不能为空");
                return;
            }
            if (@new != confirm)
            {
                UI.Popup.Error(this, "两次密码输入不一致，请重新输入");
                return;
            }
            var loginBll = new Bll.Login();
            var user = loginBll.GetUserById(Global.LocalUser.ID);
            var md5pwd = Command.Crypt.Md5(old);
            if (md5pwd != user.Password)
            {
                UI.Popup.Error(this, "旧密码错误");
                return;
            }
            var md5new = Command.Crypt.Md5(@new);
            var ret = loginBll.EditUserPwd(user.ID, md5new);
            if (ret <= 0)
            {
                UI.Popup.Error(this, "修改失败");
                return;
            }

            var userAll = loginBll.GetUserAllInfoByUserId(user.ID);

            Global.LocalUser = userAll.user;
            Global.LocalRoles = userAll.roles;
            Global.LocalDepartment = userAll.depart;
            Global.LocalPost = userAll.post;

            UI.Popup.Succ(this, "修改成功");
            Close();
        }
    }
}

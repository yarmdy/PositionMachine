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
    /// ResetPassword.xaml 的交互逻辑
    /// </summary>
    public partial class ResetPassword : ZHWindow
    {
        DB.dbLabelInfo.User _user = null;
        public ResetPassword(string id)
        {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(id))
            {
                DialogResult = false;
                Close();
            }
            else
            {
                var loginBll = new Bll.Login();
                _user = loginBll.GetUserById(id);
                if (_user == null)
                {
                    UI.Popup.Error(this, "用户不存在，无法修改");
                    Close();
                    return;
                }
            }
            this.DataContext = _user;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var pwd = pwdPwd.Password;
            var pwd2 = pwdCPwd.Password;

            if (pwd == "" || pwd2 == "")
            {
                UI.Popup.Error(this, "请输入密码");
                return;
            }
            if (pwd != pwd2)
            {
                UI.Popup.Error(this, "两次密码输入不一致");
                return;
            }
            var userBll = new Bll.Login();
            var ret = userBll.EditUserPwd(_user.ID,Command.Crypt.Md5(pwd));
            if (ret <= 0)
            {
                UI.Popup.Error(this, "修改失败");
                return;
            }
            UI.Popup.Succ(this, "修改成功");
            DialogResult = true;
            Close();
        }
    }
}

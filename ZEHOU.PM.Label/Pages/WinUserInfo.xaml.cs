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
    /// WinUserInfo.xaml 的交互逻辑
    /// </summary>
    public partial class WinUserInfo : Window
    {
        ZEHOU.PM.DB.dbLabelInfo.User _user = Global.LocalUser;
        public WinUserInfo()
        {
            InitializeComponent();
            var userBll = new Bll.Login();
            _user = userBll.GetUserById(Global.LocalUser.ID);
            txtRolesName.Text = Global.LocalRolesName;
        }
    }
}

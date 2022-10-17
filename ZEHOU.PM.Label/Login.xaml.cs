using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using ZEHOU.PM.Label.UI;
using JSerialPort;
using ZEHOU.PM.Command;
using System.Collections.ObjectModel;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : ZHWindow
    {
        public Login()
        {
            InitializeComponent();
            
            //Printer.LabelPrinterApi.InitPrinterDll();
            //var printer = new Printer.LabelPrinter();
            //printer.Name = "1";
            //printer.Model = "LPQ80";
            //printer.IoSettings = "USB";
            //printer.Print(new Printer.PrinterListItem { Name="asd",Desc="",Data= Command.StringExtend.Str2Bytes("1B401B4C181B54021B5782000A009001F0001B45001B2428001D2469001D771D1D684B001D6B490A7B4232303232303130311B45001B4D001B4D041B2450001D24870032303232303130311B45001B4D001B4D041B2404011D243700D1AAC4FD1B45001B4D001B4D041B2404011D246E00CDF5421B45001B4D011B4D031B243C001D249600313030311B45001B4D001B4D041B240F001D24B900D1AAC4FDCBC41B45001B4D001B4D041B24AA001D24B900CFEE1B45001B4D001B4D041B2414001D24DC001B45001B4D001B4D041B2414001D24EE001B45001B4D001B4D041B2454011D2455000C00AF19") });
            //var asd = SerialPort.LabelMachineHelper.getCRC16(LabelController.stringData2Byte("405A480150D201020012003C2134057822F6222E05DC1D1A1C520014"));
            //int a, b;
            //var asd = new SerialPort.LabelMachineHelper("com1");

            //asd.MachineId = 1;
            //asd.PN532Helper_OnMessageAnalysis(new byte[] {
            //    0x40,0x5A,0x48,0x01,0x50,0xD2,0x01,0x10,0x00,0x12,0x00,0x3C,0x21,0x34,0x05,0x78,0x22,0xF6,0x22,0x2E,0x05,0xDC,0x1D,0x1A,0x1C,0x52,0x00,0x14,0xFC,0x3B,0x0D,0x0A
            //}, out a, out b);

#if DEBUG
            return;
#endif

            if (RegisterClass.SoftRegister.IsRegister())
            {
                return;
            }

            var res = new Register().ShowDialog();
            if (!(res ?? false)) { 
                Close();
            }
        }

        private void btnLogin_Copy_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginName=txtUserName.Text.Trim();
            var password = pwdPassword.Password.Trim();
            if (string.IsNullOrEmpty(loginName))
            {
                Popup.Error(this,"请输入用户名");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                Popup.Error(this, "请输入密码");
                return;
            }
            var loginBll = new Bll.Login();
            var user = loginBll.GetUserByLoginName(loginName);
            if (user == null)
            {
                Popup.Error(this, "用户名或密码错误");
                return;
            }
            var pwdmd5 = Command.Crypt.Md5(password);
            if (pwdmd5 != user.Password) {
                Popup.Error(this, "用户名或密码错误");
                return;
            }
            var userAll = loginBll.GetUserAllInfoByUserId(user.ID);
            Global.LocalUser = userAll.user;
            Global.LocalRoles = userAll.roles;
            Global.LocalDepartment = userAll.depart;
            Global.LocalPost = userAll.post;
            Global.MainWindow = new MainWindow();
            Global.MainWindow.Show();
            this.Close();
        }
    }

    public class A { 
        public string Name { get; set; }
        public string Password { get; set; }
        public int? Id { get; set; }
        public int code { get; set; }
    }
    public class B
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }
        public int? code { get; set; }
    }
}

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
    /// Register.xaml 的交互逻辑
    /// </summary>
    public partial class Register : ZHWindow
    {
        public Register()
        {
            
            InitializeComponent();
            var devBll=new Bll.Device();

            var devList =devBll.GetDevicesList();
            devList.ForEach(a=>comDevId.Items.Add(a.ID));
            var thisId = Config.Configs.Settings["DeviceID"];
            comDevId.SelectedIndex = devList.FindIndex(a=>a.ID==thisId);
            var sysParam = devBll.GetSystemParameterByDevId("01");

            txtHosId.Text = sysParam?.CompanyName ?? "";

            txtUserId.Text= RegisterClass.SoftRegister.CreateUserID();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string strDeviceID = "";
            string strUserID = "";
            string strCompanyName = "";
            string strRegisterKey = "";

            // 获取数据
            strDeviceID = comDevId.SelectedItem+"";
            strUserID = txtUserId.Text;
            strCompanyName = txtHosId.Text;
            strRegisterKey = txtRegisterId.Text;

            // 检验
            if (strUserID.Length != 32)
            {
                UI.Popup.Error(this,"用户标识必须是32个字符!");
                return;
            }
            if (strCompanyName == "")
            {
                UI.Popup.Error(this, "医院名称不能为空!");
                return;
            }
            if (strRegisterKey.Length != 32)
            {
                UI.Popup.Error(this, "注册码必须是32个字符!");
                return;
            }

            // 注册
            if (RegisterClass.SoftRegister.Register(strUserID, strCompanyName, strDeviceID, strRegisterKey) == false)
            {
                UI.Popup.Error(this, "注册失败!");
                return;
            }

            // 保存设备编号到配置文件
            Config.Configs.Settings["DeviceID"] = strDeviceID;


            UI.Popup.Succ(this, "注册成功!");
            DialogResult = true;
            Close();
        }

        private void txtRegisterId_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbWordNum.Content = txtRegisterId.Text.Length.ToString() + " 个字符";
        }
    }
}

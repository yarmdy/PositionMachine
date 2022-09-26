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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// SystemParam.xaml 的交互逻辑
    /// </summary>
    public partial class SystemParam : Page
    {
        string tabName { get
            {
                return this.GetType().Name;
            } }
        public SystemParam()
        {
            InitializeComponent();
            var devBll=new Bll.Device();
            var sysParam = devBll.GetSystemParameterByDevId("01");

            txtName.Text = sysParam?.CompanyName??"";
            txtAddress.Text = sysParam?.CompanyAddress ?? "";
            txtPhone.Text = sysParam?.CompanyTel ?? "";
            txtFax.Text = sysParam?.CompanyFax ?? "";
            txtZipCode.Text = sysParam?.PostCode ?? "";
            txtRemarks.Text = sysParam?.Note ?? "";

            
        }

        private void tbSave_Click(object sender, RoutedEventArgs e)
        {
            var devBll = new Bll.Device();
            var sysParam = new DB.dbLabelInfo.SystemParameter { ID="01"
                ,CompanyName = txtName.Text.Trim()
                ,CompanyAddress = txtAddress.Text.Trim()
                ,CompanyTel=txtPhone.Text.Trim()
                ,CompanyFax=txtFax.Text.Trim()
                ,PostCode=txtZipCode.Text.Trim()
                ,Note=txtRemarks.Text
            };
            var res = devBll.EditSystemParameter(sysParam);
            if (res <= 0)
            {
                UI.Popup.Error(Global.MainWindow,"修改失败");
                return;
            }
            UI.Popup.Succ(Global.MainWindow, "修改成功");
        }

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }
    }
}

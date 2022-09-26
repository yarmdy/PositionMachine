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
    /// DepartmentManager.xaml 的交互逻辑
    /// </summary>
    public partial class DepartmentManager : Page
    {
        string tabName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        List<DB.dbLabelInfo.Department> departmentList = null;
        public DepartmentManager()
        {
            InitializeComponent();
            loadData();
        }
        private void loadData()
        {
            var loginBll = new Bll.Login();
            departmentList = loginBll.GetDepartments();
            dgData.ItemsSource = null;
            dgData.ItemsSource = departmentList;
        }

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }

        private void tbEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem == null || dgData.SelectedItem.GetType() != typeof(DB.dbLabelInfo.Department) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.Department))
            {
                UI.Popup.Error(Global.MainWindow, "请选择要修改的数据");
                return;
            }
            var department = (DB.dbLabelInfo.Department)dgData.SelectedItem;
            var dialog = new EditDepartment(department.ID);
            var res = dialog.ShowDialog();
            if (!(res ?? false))
            {
                return;
            }
            loadData();
        }

        private void tbAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditDepartment(null);
            var res = dialog.ShowDialog();
            if (!(res ?? false))
            {
                return;
            }
            loadData();
        }

        private void tbDel_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem == null || dgData.SelectedItem.GetType() != typeof(DB.dbLabelInfo.Department) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.Department))
            {
                UI.Popup.Error(Global.MainWindow, "请选择要删除的数据");
                return;
            }
            var department = (DB.dbLabelInfo.Department)dgData.SelectedItem;
            var res = UI.Popup.Confirm(Global.MainWindow, "确定要删除“" + department.Name + "”吗？");
            if (res != MessageBoxResult.OK)
            {
                return;
            }
            var loginBll = new Bll.Login();
            var ret = loginBll.DelDepartment(department.ID);
            if (ret <= 0)
            {
                UI.Popup.Error(Global.MainWindow, "删除失败");
                return;
            }
            UI.Popup.Succ(Global.MainWindow, "删除成功");
            loadData();
        }
    }
}

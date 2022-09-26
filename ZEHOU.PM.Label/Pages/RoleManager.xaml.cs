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
    /// RoleManager.xaml 的交互逻辑
    /// </summary>
    public partial class RoleManager : Page
    {
        string tabName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        List<DB.dbLabelInfo.Role> roleList = null;
        public RoleManager()
        {
            InitializeComponent();
            loadData();
        }

        private void loadData() {
            var loginBll = new Bll.Login();
            roleList = loginBll.GetRoles();
            dgData.ItemsSource = null;
            dgData.ItemsSource = roleList;
        }

        

        private List<CheckBox> _allCheckBox = null;
        private List<CheckBox> allCheckBox
        {
            get
            {
                if (_allCheckBox == null)
                {
                    _allCheckBox = new List<CheckBox>();
                    foreach (TreeViewItem tvia in trRoles.Items)
                    {
                        foreach(TreeViewItem tvib in tvia.Items)
                        {
                            var sp = (StackPanel)tvib.Header;
                            var ck = (CheckBox)sp.Children[0];
                            _allCheckBox.Add(ck);
                        }
                    }
                }
                return _allCheckBox;
            }
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem==null || dgData.SelectedItem.GetType()!=typeof(DB.dbLabelInfo.Role) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.Role)) {
                allCheckBox.ForEach(a => {
                    a.IsChecked = false;
                });
                return;
            }
            var role = (DB.dbLabelInfo.Role)dgData.SelectedItem;
            var funcs = role.FunctionID.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            allCheckBox.ForEach(a => {
                if (funcs.Contains(a.Tag + ""))
                {
                    a.IsChecked = true;
                }
                else
                {
                    a.IsChecked = false;
                }
            });
        }

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            allCheckBox.ForEach(a => {
                a.IsChecked = true;
            });
        }

        private void btnSelectNull_Click(object sender, RoutedEventArgs e)
        {
            allCheckBox.ForEach(a => {
                a.IsChecked = false;
            });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem == null || dgData.SelectedItem.GetType() != typeof(DB.dbLabelInfo.Role) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.Role))
            {
                UI.Popup.Error(Global.MainWindow,"请选择要修改的数据");
                return;
            }
            var funcIds = string.Join(",", allCheckBox.Where(a => a.IsChecked ?? false).Select(a => a.Tag + ""));
            var role = (DB.dbLabelInfo.Role)dgData.SelectedItem;
            role.FunctionID = funcIds;
            var loginBll = new Bll.Login();

            var res = loginBll.EditRoleFunc(role);
            if (res <= 0)
            {
                UI.Popup.Error(Global.MainWindow, "修改失败");
                return;
            }
            UI.Popup.Succ(Global.MainWindow, "修改成功");
        }

        private void tbEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem == null || dgData.SelectedItem.GetType() != typeof(DB.dbLabelInfo.Role) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.Role))
            {
                UI.Popup.Error(Global.MainWindow, "请选择要修改的数据");
                return;
            }
            var role = (DB.dbLabelInfo.Role)dgData.SelectedItem;
            var dialog = new EditRole(role.ID);
            var res = dialog.ShowDialog();
            if (!(res ?? false)) {
                return;
            }
            loadData();
        }

        private void tbAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditRole(null);
            var res = dialog.ShowDialog();
            if (!(res ?? false))
            {
                return;
            }
            loadData();
        }

        private void tbDel_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem == null || dgData.SelectedItem.GetType() != typeof(DB.dbLabelInfo.Role) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.Role))
            {
                UI.Popup.Error(Global.MainWindow, "请选择要删除的数据");
                return;
            }
            var role = (DB.dbLabelInfo.Role)dgData.SelectedItem;
            var res = UI.Popup.Confirm(Global.MainWindow,"确定要删除“"+role.Name+"”吗？");
            if (res != MessageBoxResult.OK)
            {
                return;
            }
            var loginBll = new Bll.Login();
            var ret = loginBll.DelRole(role.ID);
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

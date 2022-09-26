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
    /// EditDepartment.xaml 的交互逻辑
    /// </summary>
    public partial class EditDepartment : ZHWindow
    {
        DB.dbLabelInfo.Department _department = null;
        bool isNew = false;
        public EditDepartment(string id)
        {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(id))
            {
                _department = new DB.dbLabelInfo.Department { IsUse = true, ID = "", Name = "", Code="" };
                btnAdd.Content = "添 加";
                this.Title = "添加新部门";
                isNew = true;
            }
            else
            {
                var loginBll = new Bll.Login();
                _department = loginBll.GetDepartmentById(id);
                if (_department == null)
                {
                    UI.Popup.Error(this, "职位不存在，无法修改");
                    Close();
                    return;
                }
                isNew = false;
                txtID.IsReadOnly = true;
                btnAdd.Content = "修 改";
                this.Title = "修改部门";
            }
            this.DataContext = _department;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_department.ID))
            {
                UI.Popup.Error(this, "编号不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(_department.Name))
            {
                UI.Popup.Error(this, "名称不能为空");
                return;
            }
            _department.ID = _department.ID.Trim();
            _department.Name = _department.Name.Trim();
            int ret;
            var loginBll = new Bll.Login();
            if (isNew)
            {
                ret = loginBll.AddDepartment(_department);
                if (ret == -1)
                {
                    UI.Popup.Error(this, "编号已存在");
                    return;
                }
                if (ret <= 0)
                {
                    UI.Popup.Error(this, "添加失败");
                }
                this.DialogResult = true;
                UI.Popup.Succ(this, "添加成功");
                Close();
                return;
            }
            ret = loginBll.EditDepartment(_department);
            if (ret <= 0)
            {
                UI.Popup.Error(this, "修改失败");
            }
            this.DialogResult = true;
            UI.Popup.Succ(this, "修改成功");
            Close();
        }
    }
}

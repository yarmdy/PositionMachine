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
    /// EditPost.xaml 的交互逻辑
    /// </summary>
    public partial class EditPost : ZHWindow
    {
        DB.dbLabelInfo.Post _post = null;
        bool isNew = false;
        public EditPost(string id)
        {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(id))
            {
                _post = new DB.dbLabelInfo.Post { IsUse = true,  ID = "", Name = "", OrderID = 0 };
                btnAdd.Content = "添 加";
                this.Title = "添加新职位";
                isNew = true;
            }
            else
            {
                var loginBll = new Bll.Login();
                _post = loginBll.GetPostById(id);
                if (_post == null)
                {
                    UI.Popup.Error(this, "职位不存在，无法修改");
                    Close();
                    return;
                }
                isNew = false;
                txtID.IsReadOnly = true;
                btnAdd.Content = "修 改";
                this.Title = "修改职位";
            }
            this.DataContext = _post;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_post.ID))
            {
                UI.Popup.Error(this, "编号不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(_post.Name))
            {
                UI.Popup.Error(this, "名称不能为空");
                return;
            }
            _post.ID = _post.ID.Trim();
            _post.Name = _post.Name.Trim();
            int ret;
            var loginBll = new Bll.Login();
            if (isNew)
            {
                ret = loginBll.AddPost(_post);
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
            ret = loginBll.EditPost(_post);
            if (ret <= 0)
            {
                UI.Popup.Error(this, "修改失败");
            }
            this.DialogResult = true;
            UI.Popup.Succ(this, "修改成功");
            Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var re = new System.Text.RegularExpressions.Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }
    }
}

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
    /// PostManager.xaml 的交互逻辑
    /// </summary>
    public partial class PostManager : Page
    {
        string tabName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        List<DB.dbLabelInfo.Post> postList = null;
        public PostManager()
        {
            InitializeComponent();
            loadData();
        }
        private void loadData()
        {
            var loginBll = new Bll.Login();
            postList = loginBll.GetPosts();
            dgData.ItemsSource = null;
            dgData.ItemsSource = postList;
        }

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }

        private void tbEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem == null || dgData.SelectedItem.GetType() != typeof(DB.dbLabelInfo.Post) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.Post))
            {
                UI.Popup.Error(Global.MainWindow, "请选择要修改的数据");
                return;
            }
            var post = (DB.dbLabelInfo.Post)dgData.SelectedItem;
            var dialog = new EditPost(post.ID);
            var res = dialog.ShowDialog();
            if (!(res ?? false))
            {
                return;
            }
            loadData();
        }

        private void tbAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditPost(null);
            var res = dialog.ShowDialog();
            if (!(res ?? false))
            {
                return;
            }
            loadData();
        }

        private void tbDel_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItems.Count <= 0 || dgData.SelectedItems.Count > 1 || dgData.SelectedItem == null || dgData.SelectedItem.GetType() != typeof(DB.dbLabelInfo.Post) && dgData.SelectedItem.GetType().BaseType != typeof(DB.dbLabelInfo.Post))
            {
                UI.Popup.Error(Global.MainWindow, "请选择要删除的数据");
                return;
            }
            var post = (DB.dbLabelInfo.Post)dgData.SelectedItem;
            var res = UI.Popup.Confirm(Global.MainWindow, "确定要删除“" + post.Name + "”吗？");
            if (res != MessageBoxResult.OK)
            {
                return;
            }
            var loginBll = new Bll.Login();
            var ret = loginBll.DelPost(post.ID);
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

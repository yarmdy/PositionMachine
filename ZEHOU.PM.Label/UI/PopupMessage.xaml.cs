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

namespace ZEHOU.PM.Label.UI
{
    /// <summary>
    /// PopupMessage.xaml 的交互逻辑
    /// </summary>
    public partial class PopupMessage : Window
    {
        byte? _code = null;
        public PopupMessage(string title,string content,string btn1Name="确定",string btn2Name=null,byte? code=null)
        {
            InitializeComponent();
            Title = title;
            lbContent.Content = content;

            btnOK.Content = btn1Name;
            btnRetry.Content = btn2Name;

            btnOK.Visibility = btnOK.Content==null? Visibility.Collapsed: Visibility.Visible;
            btnRetry.Visibility = btnRetry.Content==null? Visibility.Collapsed: Visibility.Visible;

            _code = code;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            var errCode = _code??0;
        }
    }
}

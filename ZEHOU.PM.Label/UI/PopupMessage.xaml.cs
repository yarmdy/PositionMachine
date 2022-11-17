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
        Action _action = null;
        public PopupMessage(string title,string content,string btn1Name="确定",string btn2Name=null,Action action = null)
        {
            InitializeComponent();
            Title = title;
            lbContent.Content = content;

            btnOK.Content = btn1Name;
            btnRetry.Content = btn2Name;

            btnOK.Visibility = btnOK.Content==null? Visibility.Collapsed: Visibility.Visible;
            btnRetry.Visibility = btnRetry.Content==null? Visibility.Collapsed: Visibility.Visible;

            _action = action;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_action != null)
            {
                //_label.TubeLabelStatus = 0;
                //Global.LabelController.SendLabelList();
                _action();
            }
            Close();
        }
    }
}

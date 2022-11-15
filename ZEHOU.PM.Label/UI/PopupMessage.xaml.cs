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
        LabelInfoNotify _label = null;
        public PopupMessage(string title,string content,LabelInfoNotify label =null)
        {
            InitializeComponent();
            Title = title;
            lbContent.Content = content;
            _label = label;
            if (_label != null)
            {
                btnRetry.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _label.TubeLabelStatus = 0;
            Global.LabelController.SendLabelList();
            Close();
        }
    }
}

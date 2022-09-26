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

namespace ZEHOU.PM.Label.UI
{
    /// <summary>
    /// LeftButton.xaml 的交互逻辑
    /// </summary>
    [System.ComponentModel.DefaultEvent("Click")]
    public partial class LeftButton : UserControl
    {
        

        public event RoutedEventHandler Click = null;

        public ImageSource Ico
        {
            get
            {
                return imgIco.Source;
            }
            set
            {
                imgIco.Source= value;
            }
        }
        public object Text
        {
            get
            {
                return lbText.Content;
            }
            set
            {
                lbText.Content= value;
            }
        }
        public LeftButton()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderBrush = Brushes.Black;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderBrush = Brushes.Transparent;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Click == null) return;
            Click(this,e);
        }
    }
}

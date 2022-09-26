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
    /// InputCode.xaml 的交互逻辑
    /// </summary>
    public partial class InputCode : ZHWindow
    {
        public InputCodeClass InputCodeClass { get; set; }
        public InputCode()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                lbMsg.Content = "请输入编号";
                return;
            }
            InputCodeClass.Code = txtCode.Text.Trim();
            Close();
        }
    }

    public class InputCodeClass { 
        public string Code { get; set; }
    }
}

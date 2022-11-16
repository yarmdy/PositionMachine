using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
    public partial class InputingBox : Window
    {
        int timeout = 0;
        System.Timers.Timer timer =new System.Timers.Timer(1000);
        public InputingBox()
        {
            InitializeComponent();
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Enabled=false;
            timeout--;
            if (timeout < 0) { timeout = 0;}
            if(timeout <= 0) {
                Dispatcher.Invoke(()=> Hide());
            }
            timer.Enabled=true;
        }

        public void SetCode(string code)
        {
            inpCode.Text = code; 
            Show();
            timeout = 2;
            inpCode.Focus();
        }
        public void Complete()
        {
            inpCode.Text = "";
            timeout = 0;
        }
    }
}

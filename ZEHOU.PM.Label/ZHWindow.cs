using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ZEHOU.PM.Label
{
    public class ZHWindow:Window
    {
        public override void EndInit()
        {
            base.EndInit();
            Icon= new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "favicon.ico")));
        }
    }
}

using NPOI.Util;
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
using ZEHOU.PM.Camera;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class Camera : ZHWindow
    {
        CameraDevice device = null;
        public Camera()
        {
            InitializeComponent();
            var cameraList = CameraLib.GetDeviceList();
            cameraList.ForEach(a=>cbCamera.Items.Add(new SelectObj { Device=a}));
            if (cbCamera.Items.Count <= 0) return;
            cbCamera.SelectedIndex = 0;
        }

        private void ZHWindow_Closed(object sender, EventArgs e)
        {
            if (device != null)
            {
                device.Stop();
                device.OnNewFrame -= Device_OnNewFrame;
            }
            Global.CameraWindow = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
            ((Button)sender).Content = Topmost ? "取消固定" : "窗口固定";
        }

        private void cbCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (device != null)
            {
                device.Stop();
                device.OnNewFrame -= Device_OnNewFrame;
            }
            device = (cbCamera.SelectedItem as SelectObj).Device;
            device.OnNewFrame += Device_OnNewFrame;
            device.Start();
        }

        private void Device_OnNewFrame(Func<ImageSource> obj)
        {
            Dispatcher.Invoke(()=>imgCamera.Source=obj());
        }

        private class SelectObj
        {
            public CameraDevice Device { get; set; }
            public override string ToString()
            {
                return Device?.Name??"";
            }
        }
    }
}

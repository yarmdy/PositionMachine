using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace ZEHOU.PM.Camera
{
    public class CameraLib
    {
        public static List<CameraDevice> GetDeviceList() {
            var filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (filter == null || filter.Count <= 0) { return new List<CameraDevice>(); }
            var res = new List<CameraDevice>();
            foreach (FilterInfo dev in filter)
            {
                res.Add(new CameraDeviceSetter (dev.MonikerString, dev.Name));
            }
            return res;
        }
    }

    public class CameraDevice
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);


        protected string _Id;
        protected string _Name;
        private VideoCaptureDevice _Device;
        public event Action<Func<ImageSource>> OnNewFrame;
        public string Id
        {
            get { return _Id; }
        }
        public string Name { 
            get { return _Name; }
        }
        public void Start() {
            if (_Device != null)
            {
                _Device.SignalToStop();
                _Device.NewFrame -= _Device_NewFrame;
            }
            _Device = new VideoCaptureDevice(_Id);
            _Device.NewFrame += _Device_NewFrame;
            _Device.Start();
        }
        public void Stop()
        {
            if (_Device == null)
            {
                return;
            }
            _Device.SignalToStop();
            _Device.NewFrame -= _Device_NewFrame;
        }

        private void _Device_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            if (OnNewFrame == null) return;
            
            OnNewFrame(() => {
                var bmp = (Bitmap)eventArgs.Frame.Clone();
                var intptr = bmp.GetHbitmap();
                var img = Imaging.CreateBitmapSourceFromHBitmap(intptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(intptr);
                return img;
            });
        }
    }
    class CameraDeviceSetter:CameraDevice
    {
        public CameraDeviceSetter(string id,string name)
        {
            _Id = id;
            _Name = name;
        }
        public void SetIdName(string id, string name)
        {
            _Id = id;
            _Name = name;
        }
    }
}

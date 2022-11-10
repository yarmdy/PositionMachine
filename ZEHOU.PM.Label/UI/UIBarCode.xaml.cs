using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    public partial class UIBarCode : UserControl
    {
        private string _Code = "";
        public string Code { get { return _Code; } set { 
                _Code = value;
                setBarcode();
            } }
        [TypeConverter(typeof(LengthConverter))]
        public double PicWidth { get; set; }
        [TypeConverter(typeof(LengthConverter))]
        public double PicHeight { get; set; }
        public UIBarCode()
        {
            InitializeComponent();
            
        }

        private void setBarcode() {
            if (string.IsNullOrEmpty(Code))
            {
                return;
            }
            var bw = new ZXing.BarcodeWriter();
            bw.Options = new ZXing.Common.EncodingOptions();
            bw.Options.Height = (int)this.PicHeight;
            bw.Options.Width = (int)this.PicWidth;
            bw.Format = ZXing.BarcodeFormat.CODE_128;
            var img = bw.Write(Code+"");
            var ms = new MemoryStream();
            img.Save(ms,System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Position = 0;
            ImageSourceConverter imgcvtr = new ImageSourceConverter();
            imgBarCode.Source = (ImageSource)imgcvtr.ConvertFrom(ms);
        }


        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }
    }
}

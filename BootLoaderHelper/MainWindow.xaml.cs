using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using ZEHOU.PM.Label.SerialPort;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace BootLoaderHelper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private LabelMachineHelper _lpm;
        byte _commId = 0;
        ManualResetEvent _mre = new ManualResetEvent(false);
        bool _succ = false;
        public MainWindow()
        {
            InitializeComponent();
            _lpm = new LabelMachineHelper(System.Configuration.ConfigurationManager.AppSettings["PortName"]+"");
            _lpm.OnBackWriteBin += _lpm_OnBackWriteBin;
        }

        private void _lpm_OnBackWriteBin(DataPackage obj)
        {
            if (obj.CommId != _commId)
            {
                return;
            }
            if (obj.Data[0] != 1)
            {
                _succ = false;
                _mre.Set();
                
                return;
            }
            _succ = true;
            _mre.Set();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "固件文件|*.bin";
            ofd.Multiselect = false;
            ofd.Title = "选择固件文件";
            ofd.FileOk += Ofd_FileOk;
            ofd.ShowDialog();
        }

        private void Ofd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            txt_filename.Text = ((System.Windows.Forms.OpenFileDialog)sender).FileName;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (txt_filename.Text == "")
            {
                MessageBox.Show(this, "请选择固件文件");
                return;
            }
            if (!File.Exists(txt_filename.Text))
            {
                MessageBox.Show(this, "您选择的固件文件不存在，请重新选择");
                return;
            }
            var filename = txt_filename.Text;
            Task.Run(() => {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                Dispatcher.Invoke(() => {
                    proBar.Maximum = fs.Length;
                    proBar.Value = 0;
                });

                while (fs.Position < fs.Length)
                {
                    uint addr = (uint)fs.Position;
                    var data = new byte[1024];
                    var len = 0;
                    while (len < 1024 && fs.Position < fs.Length)
                    {
                        len += fs.Read(data, len, 1024 - len);
                    }
                    var retryCount = 0;
                    while (true)
                    {
                        _succ = false;
                        _commId = _lpm.WriteBin(addr, data);
                        _mre.Reset();
                        _mre.WaitOne(3000);

                        if (_succ || retryCount >= 3)
                        {
                            break;
                        }
                        retryCount++;
                    }
                    if (_succ)
                    {
                        Dispatcher.Invoke(() => {
                            proBar.Value = fs.Position;
                        });
                        continue;
                    }
                    break;
                }
                fs.Close();
                Dispatcher.Invoke(() => {
                    if (!_succ)
                    {
                        MessageBox.Show(this, "固件写入失败，请联系管理员");
                        return;
                    }
                    MessageBox.Show(this, "固件写入成功，点击确认后下位机开机");
                    _lpm.EnterSystem(0);
                    Close();
                });
            });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _lpm.EnterBoot();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JSocket;
using JSocket.JTcp;
using System.Net;

namespace JSocketTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        JTcp tcp = null;
        private void button1_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;
            var ipep = new IPEndPoint(JSocketLib.GetLocalIp().FirstOrDefault(a=>a.AddressFamily==System.Net.Sockets.AddressFamily.InterNetwork),3390);
            if (tcp == null)
            {
                tcp = new JTcp(ipep);
                tcp.OnConnect += Tcp_OnConnect;
            }
            else {
                tcp.Reset(ipep);
            }
            tcp.Start(10);
            btn.Enabled = true;
            MessageBox.Show("监听");
        }

        private void Tcp_OnConnect(System.Net.Sockets.Socket obj)
        {
            var lendata = new byte[4];
            obj.ReceiveAll(lendata);
            var len = BitConverter.ToInt32(lendata,0);
            var data = new byte[len];
            obj.ReceiveAll(data);

            var str = Encoding.UTF8.GetString(data);
            Invoke(new Action(()=>Text=str));
            MessageBox.Show(str,str,MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var tcp = new JTcp();
            var res = tcp.Connect(new IPEndPoint(JSocketLib.GetLocalIp().FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork), 3390));

            if (!res) return;
            var datastr = "hello world";
            var data = Encoding.UTF8.GetBytes(datastr);
            tcp.Send(BitConverter.GetBytes(data.Length));
            tcp.Send(data);
        }
    }
}

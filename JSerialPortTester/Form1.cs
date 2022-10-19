using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JSerialPort;

namespace JSerialPortTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var com = new SerialPortLib("com1");
            com.OnMessageAnalysis += analysis;
            com.OnReceive += received;
        }

        private bool received(byte[] arg)
        {
            return true;
        }

        private bool analysis(byte[] data, out int index, out int length)
        {
            length = data.Length;
            index = 0;
            return true;
        }
    }
}

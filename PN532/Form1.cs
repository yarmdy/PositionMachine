using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using JSerialPort;
using PN532Lib;

namespace PN532
{
    public partial class Form1 : Form
    {
        PN532Helper pn532 = null;
        public Form1()
        {
            InitializeComponent();

            //(int id,int[] list) a = (1, new[] { 1, 2, 3 });
            //(int id,int[] list) b = (2, new[] { 4, 5, 6 });
            //(int id,int[] list) c = (3, new int[] {  });
            //(int id, int[] list)[] abc = {a,b,c };
            //var d = abc.SelectMany((aa, ii) => {
            //    return aa.list.DefaultIfEmpty();
            //}, (bb, cc) => {
            //    return cc;
            //}).ToArray();

            pn532 = new PN532Helper("com3");
            pn532.OnReceive += Pn532_OnReceive;
            pn532.OnSent += Pn532_OnSent;
            pn532.WakeUp();
        }

        private void InvokeListBoxDatas(string data) {
            Invoke(new Action(() => {
                if (listBox1.Items.Count >= 30)
                {
                    listBox1.Items.Clear();
                }
                listBox1.Items.Add(data);
            }));
        }
        private void Pn532_OnSent(byte[] obj)
        {
            InvokeListBoxDatas("发送："+string.Join(" ", obj.Select(a => a.ToString("X2"))));
        }

        private bool Pn532_OnReceive(byte[] arg)
        {
            InvokeListBoxDatas("接收："+string.Join(" ", arg.Select(a => a.ToString("X2"))));
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pn532.WakeUp();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pn532.FindCard(PN532Helper.EnumFindCardNum.One,PN532Helper.EnumBaudRate.BR9600);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var hexList = textBox1.Text.Trim().Split(new char[] { ' '},StringSplitOptions.RemoveEmptyEntries);
            if (hexList.Length <= 0)
            {
                return;
            }
            byte tmp;
            var hasError = hexList.Any(a=>!byte.TryParse(a,System.Globalization.NumberStyles.HexNumber,null,out tmp));
            if (hasError)
            {
                MessageBox.Show(this,"输入的字符串不正确","错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            var data = hexList.Select(a => byte.Parse(a, System.Globalization.NumberStyles.HexNumber)).ToArray();
            if (checkBox1.Checked)
            {
                pn532.Send(data);
                return;
            }
            pn532.Send(pn532.CreateCommand(data));
        }
    }
}

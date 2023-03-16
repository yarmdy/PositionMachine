using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JHttp;

namespace JHttpDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        JHttpH http=new JHttpH();
        private void button1_Click(object sender, EventArgs e)
        {
            var res = http.Get(textBox1.Text);

            textBox2.Text = res;
        }
    }
}

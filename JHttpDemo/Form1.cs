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
            var obj = JDynamicObject.Create(res);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var res = http.POST(textBox1.Text, null, new { id="1",name="aa"});

            textBox2.Text = res;
            var obj = JDynamicObject.Create(res);
        }
    }
}

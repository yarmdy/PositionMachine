using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JHttp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

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
            
            var res = http.Get(textBox1.Text,new{ Accept_Encoding = "br,deflate,gzip" },null);

            textBox2.Text = res;
            var obj = JDynamicObject.Create(res);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var res = http.POST(textBox1.Text, new { Accept_Encoding = "br" }, new { id="1",name="aa"});

            textBox2.Text = res;
            var obj = JDynamicObject.Create(res);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            var body = new { username = "admin", password = Md5("admin") };
            var res = http.Get("http://27.184.146.232:18080/api/user/login", body);
            textBox2.Text = res;
            var obj = JDynamicObject.Create(res);
            if (!(obj is string) && obj.code == 0)
            {
                http.CustomHeader["access-token"] = obj.data.accessToken;
            }
        }
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            var body = new { page = 1, count = 10 };
            var res = http.Get("http://27.184.146.232:18080/api/device/query/devices", body);
            textBox2.Text = res;
        }

        public static string Md5(string strText)
        {
            var source = Encoding.UTF8.GetBytes(strText);

            var MD5CSP = new MD5CryptoServiceProvider();
            var tmp = MD5CSP.ComputeHash(source);    // MD5的计算结果是一个128位的长整数，用字节表示就是16个字节
            

            return string.Join("",tmp.Select(a=>a.ToString("x2")));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var res = http.Get("http://27.184.146.232:18080/api/device/query/devices/43000000801320000008");
            textBox2.Text = res;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var res = http.Get("http://27.184.146.232:18080/api/device/query/devices/43000000801320000008/channels", new { page=1,count=10});
            textBox2.Text = res;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var res = http.Get("http://27.184.146.232:18080/api/play/start/43000000801320000008/43000000801310000008", new { page=1,count=10});
            textBox2.Text = res;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var res = http.POST("http://121.42.8.56:7502/Login/CheckLogin", new { account ="System", password = "4a7d1ed414474e4033ac29ccb8653d9b" });
            textBox2.Text = res;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var res = http.Get("http://121.42.8.56:7502/BasicSettingModule/WaterCompany/LoadData?keywords=&beginTime=2019-01-01&endTime=2023-04-03&_search=false&nd=1680505468708&rows=30&page=1&sidx=&sord=desc");
            textBox2.Text = res;
        }
    }


}

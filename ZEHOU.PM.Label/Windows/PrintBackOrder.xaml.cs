using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Markup;
using log4net;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using NPOI.SS.Formula.Functions;
using static System.Net.Mime.MediaTypeNames;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class PrintBackOrder : ZHWindow
    {
        List<LabelInfoNotify> _info = null;
        PatientInfoNotify _pat = null;
        BackOrderOtherInfo _oth = new BackOrderOtherInfo();
        LabelInfoNotify _local = null;
        Regex _reg = new Regex(@"\{(pat|tub|oth|lst)\:([^{}]*(((?'Open'\{)[^{}]*)+((?'-Open'\})[^{}]*)+)*)\}", RegexOptions.Singleline);

        public FlowDocument FlowDoc { get; set; }
        public PrintBackOrder(List<LabelInfoNotify> info)
        {
            InitializeComponent();
            _info = info;
            var index = 0;
            _info.ForEach(a => a.TubeInfo.Index = ++index);
            _pat = info.FirstOrDefault().Patient;
            _oth.Count = info.Count;
            _local = info.FirstOrDefault();
            dvDoc.Visibility = Visibility.Visible;
            fdrDoc.Visibility = Visibility.Collapsed;
            LoadFlowDoc();
            LoadXps();
        }

        private void LoadFlowDoc() {
            var txt = File.ReadAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "BackOrderTemplate.xaml"));

            //var reg = new Regex(@"(?:\{(pat)\:(.+?)\}|\{(tub)\:(.+?)\}|\{(oth)\:(.+?)\})", RegexOptions.Singleline);
            
            txt = _reg.Replace(txt,new MatchEvaluator(MatchEvaluator));

            FlowDoc = XamlReader.Parse(txt) as FlowDocument;
        }

        public string MatchEvaluator(System.Text.RegularExpressions.Match mtch) {
            var act = mtch.Groups[1].Value;
            var val = mtch.Groups[2].Value;
            var res = "";
            switch (act)
            {
                case "pat":
                    {
                        res = GetObjPropVal(_pat,val);
                        break;
                    }
                case "tub":
                    {
                        res = GetObjPropVal(_local.TubeInfo,val);
                        break;
                    }
                case "oth":
                    {
                        res=GetObjPropVal(_oth,val);
                        break;
                    }
                case "lst":
                    {
                        res = string.Join("", _info.Select(a => {
                            _local = a;
                            return _reg.Replace(val, new MatchEvaluator(MatchEvaluator));
                        }));
                        break;
                    }
            }
            return res;
        }

        public string GetObjPropVal(object obj, string propName) {
            if (obj == null)
            {
                return "";
            }
            var prop = obj.GetType().GetProperty(propName,System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.GetProperty);
            if (prop == null)
            {
                return "";
            }
            var type = prop.PropertyType;
            var val = prop.GetValue(obj);
            if (val == null)
            {
                return "";
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                var dt = (DateTime)val;
                if (dt.TimeOfDay > TimeSpan.FromTicks(0))
                {
                    return dt.ToString("yyyy/MM/dd HH:mm:ss");
                }
                return dt.ToString("yyyy/MM/dd");
            }
            return val + "";
        }

        private void LoadXps()
        {
            //构造一个基于内存的xps document
            MemoryStream ms = new MemoryStream();
            Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
            Uri DocumentUri = new Uri("pack://InMemoryDocument.xps");
            PackageStore.RemovePackage(DocumentUri);
            PackageStore.AddPackage(DocumentUri, package);
            XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Fast, DocumentUri.AbsoluteUri);

            //将flow document写入基于内存的xps document中去
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            writer.Write(((IDocumentPaginatorSource)FlowDoc).DocumentPaginator);

            //获取这个基于内存的xps document的fixed document
            dvDoc.Document = xpsDocument.GetFixedDocumentSequence();

            //关闭基于内存的xps document
            xpsDocument.Close();
        }

        public void Print() {
            var pd = new PrintDialog();
            pd.PrintDocument(((IDocumentPaginatorSource)FlowDoc).DocumentPaginator, "回执单");
        }

        public class BackOrderOtherInfo
        {
            public DateTime Now
            {
                get
                {
                    return DateTime.Now;
                }
            }
            public int Count
            {
                get;set;
            }

        }
    }
}

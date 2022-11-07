using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.IO;
using System.Windows.Markup;
using log4net;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class PrintBackOrder : ZHWindow
    {
        LabelInfoNotify _info = null;
        public FlowDocument FlowDoc { get; set; }
        public PrintBackOrder(LabelInfoNotify info)
        {
            InitializeComponent();
            _info = info;
            dvDoc.Visibility = Visibility.Visible;
            fdrDoc.Visibility = Visibility.Collapsed;
            LoadFlowDoc();
            LoadXps();
        }

        private void LoadFlowDoc() {
            var txt = File.ReadAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "BackOrderTemplate.xaml"));
            FlowDoc = XamlReader.Parse(txt) as FlowDocument;
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
    }
}

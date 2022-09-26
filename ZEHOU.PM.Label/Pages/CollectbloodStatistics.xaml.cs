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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// CollectbloodStatistics.xaml 的交互逻辑
    /// </summary>
    public partial class CollectbloodStatistics : Page
    {
        string tabName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        public CollectbloodStatistics()
        {
            InitializeComponent();
            var datenow = DateTime.Now;
            dpStartTime.SelectedDate = datenow.Date.AddDays(-datenow.Day+1);
            dpEndTime.SelectedDate = datenow.Date;

            loadData();
        }

        private void loadData() {
            var reportBll = new Bll.Report();
            var list = reportBll.GetNurseStatistics(dpStartTime.SelectedDate,dpEndTime.SelectedDate,txtID.Text.Trim());
            var list2 = reportBll.GetTubeStatistics(dpStartTime.SelectedDate,dpEndTime.SelectedDate,txtID.Text.Trim());

            dgData.ItemsSource = null;
            dgData.ItemsSource = list.Select(a => { 
                var res = new NurseModel { ID= a.user.ID,TrueName=a.user.TrueName};
                res.TubeNum = a.tubeNum;
                res.PatientNum = a.patientNum;
                return res;
            }).ToList();

            dgData2.ItemsSource = null;
            dgData2.ItemsSource = list2.Select(a => new TubeModel { TubeColor=a.tubeColor,TubeNum=a.tubeNum}).ToList();
        }

        private void tbSearch_Click(object sender, RoutedEventArgs e)
        {
            loadData();
        }

        private void tbExcel_Click(object sender, RoutedEventArgs e)
        {
            var wb = new XSSFWorkbook();
            var sheet = wb.CreateSheet("护士统计");
            int rowId = 0;
            int colId = 0;
            var rowx = sheet.CreateRow(rowId++);
            var cols = getColumnObjList(dgData).OrderBy(a=>a.index).ToList();
            foreach (var col in cols) {
                var xcol = rowx.CreateCell(colId++);
                xcol.SetCellValue(col.title);
            }
            foreach (object rowobj in dgData.Items) {
                if (rowobj.GetType() != typeof(NurseModel) && rowobj.GetType().BaseType != typeof(NurseModel)) {
                    continue;
                }
                var row = (NurseModel)rowobj;
                colId = 0;
                rowx = sheet.CreateRow(rowId++);
                foreach (var col in cols)
                {
                    var xcol = rowx.CreateCell(colId++);
                    var ty = row.GetType();
                    var wordsProp = ty.GetProperty(col.words);
                    var cvalue = wordsProp==null?"": wordsProp.GetValue(row);
                    xcol.SetCellValue(cvalue+"");
                }
            }
            sheet = wb.CreateSheet("试管统计");
            rowId = 0;
            colId = 0;
            rowx = sheet.CreateRow(rowId++);
            cols = getColumnObjList(dgData2).OrderBy(a => a.index).ToList();
            foreach (var col in cols)
            {
                var xcol = rowx.CreateCell(colId++);
                xcol.SetCellValue(col.title);
            }
            foreach (object rowobj in dgData2.Items)
            {
                if (rowobj.GetType() != typeof(TubeModel) && rowobj.GetType().BaseType != typeof(TubeModel))
                {
                    continue;
                }
                var row = (TubeModel)rowobj;
                colId = 0;
                rowx = sheet.CreateRow(rowId++);
                foreach (var col in cols)
                {
                    var xcol = rowx.CreateCell(colId++);
                    var ty = row.GetType();
                    var wordsProp = ty.GetProperty(col.words);
                    var cvalue = wordsProp == null ? "" : wordsProp.GetValue(row);
                    xcol.SetCellValue(cvalue + "");
                }
            }

            var stream = new System.IO.MemoryStream();
            wb.Write(stream);
            var buffer = stream.ToArray();
            wb.Close();

            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.Filter = "Excel文件(*.xlsx)|*.xlsx";
            dlg.DefaultExt = ".xlsx";
            var result = dlg.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            System.IO.File.WriteAllBytes(dlg.FileName, buffer);
        }

        

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }

        private List<(int index, string title, string words)> getColumnObjList(DataGrid dg)
        {
            var res = new List<(int index, string title, string words)>();
            foreach (DataGridColumn dgc in dg.Columns)
            {
                (int index, string title, string words) item;
                item.title = dgc.Header + "";
                item.index = dgc.DisplayIndex;
                var t = dgc.GetType();
                var bdProp = t.GetProperty("Binding");
                var bd = (Binding)bdProp.GetValue(dgc);
                item.words = bd.Path.Path;
                res.Add(item);
            }
            return res;
        }
    }

    public class NurseModel:DB.dbLabelInfo.User
    {
        public int TubeNum { get; set; }
        public int PatientNum { get; set; }
    }

    public class TubeModel : DB.dbLabelInfo.LR
    {
        public int TubeNum { get; set; }
    }
}

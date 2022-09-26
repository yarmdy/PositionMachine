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
    /// LabelRecord.xaml 的交互逻辑
    /// </summary>
    public partial class LabelRecord : Page
    {
        string tabName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        List<DB.dbLabelInfo.DR> drList = null;
        public LabelRecord()
        {
            InitializeComponent();
            var datenow = DateTime.Now;
            dpStartTime.SelectedDate = datenow.Date.AddDays(-datenow.Day+1);
            dpEndTime.SelectedDate = datenow.Date;

            drList = new Bll.Device().GetDevicesList();
            drList.Insert(0,(new DB.dbLabelInfo.DR { ID="全部设备"}));
            cbDevice.ItemsSource = drList;
            cbDevice.SelectedIndex = 0;

            loadColumn();
            loadData();
        }

        private string lrTableFileName
        {
            get
            {
                if (!System.IO.Directory.Exists(lrTableDir))
                {
                    System.IO.Directory.CreateDirectory(lrTableDir);
                }

                return System.IO.Path.Combine(lrTableDir, $"lrtable.config");
            }
        }
        private string lrTableDir { get { return System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, $"{Global.LocalUser.ID}\\"); } }
        private void loadColumn() {
            if (!System.IO.File.Exists(lrTableFileName))
            {
                return;
            }
            var str = System.IO.File.ReadAllText(lrTableFileName, Encoding.UTF8);
            var data = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(str);

            foreach (DataGridColumn dgc in dgData.Columns)
            {
                var t = dgc.GetType();
                var bdProp = t.GetProperty("Binding");
                var bd = (Binding)bdProp.GetValue(dgc);
                var words = bd.Path.Path;

                var item = data.FirstOrDefault(a => a["words"]+"" ==words);

                if (item == null) continue;
                dgc.DisplayIndex = int.Parse(item["index"]+"");
            }
        }
        private void loadData() {
            var reportBll = new Bll.Report();
            var did = ((DB.dbLabelInfo.DR)cbDevice.SelectedItem)?.ID;
            var list = reportBll.GetLabelRecordList(did=="全部设备"?null:did, dpStartTime.SelectedDate,dpEndTime.SelectedDate,txtBarCode.Text.Trim(),txtPatientID.Text.Trim(),txtPatientName.Text.Trim());

            dgData.ItemsSource = null;
            dgData.ItemsSource = list;
        }

        private void tbSearch_Click(object sender, RoutedEventArgs e)
        {
            loadData();
        }

        private void tbExcel_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.Items.Count <= 0 || dgData.Items.Count==1 && dgData.Items[0].GetType()!= typeof(DB.dbLabelInfo.LR) && dgData.Items[0].GetType().BaseType != typeof(DB.dbLabelInfo.LR))
            {
                UI.Popup.Error(Global.MainWindow,"无记录，无法导出。");
                return;
            }
            var wb = new XSSFWorkbook();
            var sheet = wb.CreateSheet("sheet1");
            int rowId = 0;
            int colId = 0;
            var rowx = sheet.CreateRow(rowId++);
            var cols = getColumnObjList().OrderBy(a=>a.index).ToList();
            foreach (var col in cols) {
                var xcol = rowx.CreateCell(colId++);
                xcol.SetCellValue(col.title);
            }
            foreach (object rowobj in dgData.Items) {
                if (rowobj.GetType() != typeof(DB.dbLabelInfo.LR) && rowobj.GetType().BaseType != typeof(DB.dbLabelInfo.LR)) {
                    continue;
                }
                var row = (DB.dbLabelInfo.LR)rowobj;
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

        private void tbPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }

        bool isTwo = true;
        private void dgData_ColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
        {
            isTwo = !isTwo;
            if (!isTwo)
            {
                return;
            }
            var data = getColumnObjList().Select(a => new { a.index,a.title,a.words});
            var dataStr = JsonConvert.SerializeObject(data,Formatting.Indented);
            System.IO.File.WriteAllText(lrTableFileName,dataStr,Encoding.UTF8);
        }

        private List<(int index, string title, string words)> getColumnObjList() {
            var res = new List<(int index, string title, string words)>();
            foreach (DataGridColumn dgc in dgData.Columns) {
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
}

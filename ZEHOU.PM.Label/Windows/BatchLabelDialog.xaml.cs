using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// BatchLabelDialog.xaml 的交互逻辑
    /// </summary>
    public partial class BatchLabelDialog : ZHWindow
    {
        public List<BatchLabelItem> Result { get; set; }
        private BatchLabelDialogModel model=null;
        public BatchLabelDialog()
        {
            InitializeComponent();
            model = new BatchLabelDialogModel();
            this.DataContext = model;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(model.StartNo) || string.IsNullOrWhiteSpace(model.EndNo) || string.IsNullOrWhiteSpace(model.TubeColor)) {
                UI.Popup.Error(this,"请输入编号开始值、编号结束值、采样类型");
                return;
            }
            model.StartNo = model.StartNo?.Trim()??"";
            model.EndNo = model.EndNo?.Trim() ?? "";
            model.TubeColor = model.TubeColor?.Trim() ?? "";
            model.PName = model.PName?.Trim() ?? "";
            model.Remark = model.Remark?.Trim() ?? "";
            var reg = new Regex(@"^([0-9a-zA-Z]*?)([0-9]+)$",RegexOptions.Singleline);
            if(!reg.IsMatch(model.StartNo) || !reg.IsMatch(model.EndNo))
            {
                UI.Popup.Error(this, "编号开始值或编号结束值格式不正确，只能输入数字或字母，且必须以数字结尾");
                return;
            }
            var matchStart = reg.Match(model.StartNo);
            var matchEnd = reg.Match(model.EndNo);
            if (matchStart.Groups[1].Value != matchEnd.Groups[1].Value) {
                UI.Popup.Error(this, "编号开始值和编号结束值无法匹配");
                return;
            }
            var startNo = long.Parse(matchStart.Groups[2].Value);
            var endNo = long.Parse(matchEnd.Groups[2].Value);
            if (startNo > endNo)
            {
                UI.Popup.Error(this, "编号开始值必须小于编号结束值");
                return;
            }
            int no = 0;
            var noLen = matchEnd.Groups[2].Value.Length;
            model.Source = new long[endNo - startNo + 1].Select(a => new BatchLabelItem {Id=++no,BarCode=$"{matchStart.Groups[1].Value}{(startNo++).ToString().PadLeft(noLen,'0')}",PName=model.PName,Remark=model.Remark,TubeColor=model.TubeColor }).ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (model.Source == null || model.Source.Count <= 0) {
                UI.Popup.Error(this, "没有任何要打印的数据");
                return;
            }
            Result = model.Source;
            Close();
        }
    }

    public class BatchLabelDialogModel: INotifyPropertyChanged
    {
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 开始编号
        /// </summary>
        private string _StartNo;
        /// <summary>
        /// 开始编号
        /// </summary>
        public string StartNo
        {
            get { return _StartNo; }
            set { _StartNo = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("StartNo")); }
        }
        /// <summary>
        /// 结束编号
        /// </summary>
        private string _EndNo;
        /// <summary>
        /// 结束编号
        /// </summary>
        public string EndNo
        {
            get { return _EndNo; }
            set { _EndNo = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("EndNo")); }
        }
        /// <summary>
        /// 颜色
        /// </summary>
        private string _TubeColor;
        /// <summary>
        /// 颜色
        /// </summary>
        public string TubeColor
        {
            get { return _TubeColor; }
            set { _TubeColor = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("TubeColor")); }
        }
        /// <summary>
        /// 姓名
        /// </summary>
        private string _PName;
        /// <summary>
        /// 姓名
        /// </summary>
        public string PName
        {
            get { return _PName; }
            set { _PName = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("PName")); }
        }
        /// <summary>
        /// 备注
        /// </summary>
        private string _Remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Remark")); }
        }

        /// <summary>
        /// 数据源
        /// </summary>
        private List<BatchLabelItem> _Source;
        /// <summary>
        /// 数据源
        /// </summary>
        public List<BatchLabelItem> Source
        {
            get { return _Source; }
            set { _Source = value; if (PropertyChanged == null) return; PropertyChanged(this, new PropertyChangedEventArgs("Source")); }
        }
    }

    public class BatchLabelItem {
        /// <summary>
        /// 序号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 条码号
        /// </summary>
        public string BarCode { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string TubeColor
        { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string PName
        { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        { get; set; }

    }
}

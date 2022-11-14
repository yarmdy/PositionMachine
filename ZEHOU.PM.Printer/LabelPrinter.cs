using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZEHOU.PM.Printer
{
    public class LabelPrinter {
        public string Name { get; set; }
        public string Model { get; set; }
        public string IoSettings { get; set; }

        private bool doing = false;

        private List<PrinterListItem> _printList = new List<PrinterListItem>();
        public event Action<LabelPrinter,PrinterEventArgs> OnError;
        public event Action<LabelPrinter,PrinterEventArgs> OnListAdd;
        public event Action<LabelPrinter,PrinterEventArgs> OnListRemove;
        public event Action<LabelPrinter,PrinterEventArgs> OnPrint;

        public List<PrinterListItem> PrintList
        {
            get {
                return _printList.ToList();
            }
        }
        public LabelPrinter() {
        }

        public LabelPrinter(string name,string model,string ioSettings) { 
            Name = name;
            Model = model;
            IoSettings = ioSettings;
        }

        public void Print(PrinterListItem info)
        {
            DateTime dt = DateTime.Now;
            lock (_printList)
            {
                _printList.Add(info);
                if (OnListAdd != null)
                {
                    OnListAdd(this,new PrinterEventArgs { Item=info});
                }

                if (doing)
                {
                    return;
                }
                doing = true;
            }
            Print();
        }
        private void Print()
        {

            bool had = _printList.Count > 0;
            if (!had)
            {
                goto finish;
            }
            try
            {
                IntPtr printer = IntPtr.Zero;
                var ret = LabelPrinterApi.PrinterCreator(ref printer, Model);
                if (Constants.E_SUCCESS != ret) {
                    if (OnError!=null) {
                        OnError(this,new PrinterEventArgs {ErrorCode=ret,Message=$"【{Name}】打印机初始化失败",BackObj=_printList.Where(a=>a.BackObj!=null).Select(a=>a.BackObj).ToList() });
                        _printList.Clear();
                    }
                    goto finish;
                }
                if (IoSettings.ToLower().Contains("usb"))
                {
                    ret = LabelPrinterApi.PortOpen(printer, IoSettings);
                    if (Constants.E_SUCCESS != ret)
                    {
                        if (OnError != null)
                        {
                            OnError(this, new PrinterEventArgs { ErrorCode = ret, Message = $"【{Name}】打印机打开失败", BackObj = _printList.Where(a => a.BackObj != null).Select(a => a.BackObj).ToList() });
                            _printList.Clear();
                        }
                        goto finish;
                    }
                }
                

                while (had)
                {
                    var item = (PrinterListItem)null;
                    lock (_printList)
                    {
                        item = _printList[0];
                        _printList.RemoveAt(0);
                        if (OnListRemove != null)
                        {
                            OnListRemove(this, new PrinterEventArgs { Item = item });
                        }
                        had = _printList.Count > 0;
                    }
                    try
                    {
                        int tmp = 0;
                        ret = LabelPrinterApi.DirectIO(printer, item.Data, item.Data.Length, null, 0, ref tmp);
                        if (ret != 0) {
                            OnError(this, new PrinterEventArgs { ErrorCode = -999, Message = $"【{Name}】打印失败", LogInfo = "发送打印命令失败", BackObj = item.BackObj });
                            continue;
                        }
                        if (OnPrint != null)
                        {
                            OnPrint(this, new PrinterEventArgs { Item = item, ErrorCode = ret });
                        }
                    }
                    catch (Exception ex) {
                        OnError(this, new PrinterEventArgs { ErrorCode = -999, Message = $"【{Name}】打印失败", LogInfo = ex.Message, BackObj = item.BackObj });
                    }
                    
                    
                }

                LabelPrinterApi.PortClose(printer);
                LabelPrinterApi.PrinterDestroy(printer);
            }
            catch(Exception ex)
            {
                OnError(this, new PrinterEventArgs { ErrorCode = -999, Message = $"【{Name}】打印失败",LogInfo=ex.Message, BackObj = _printList.Where(a => a.BackObj != null).Select(a => a.BackObj).ToList() });
                _printList.Clear();
            }
            finish:
            doing = false;
        }
    }


    public class PrinterEventArgs : EventArgs { 
        public int ErrorCode { get; set; }

        public string Message { get; set; }

        public string LogInfo { get; set; }

        public PrinterListItem Item { get; set; }

        public object BackObj { get; set; }
    }

    public class PrinterListItem { 
        public string Name { get; set; }
        public string Desc { get; set; }

        public byte[] Data { get; set; }

        public object BackObj { get; set; }
    }
}

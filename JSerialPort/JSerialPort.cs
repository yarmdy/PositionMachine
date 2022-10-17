using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace JSerialPort {
    /// <summary>
    /// 串口控制类
    /// </summary>
    public class SerialPortLib {
        #region 私有属性
        private bool running = true;
        private SerialPort _serialPort = null;
        private Task _prossReceived = null;
        private Task _prossSend = null;
        private Task _prossOpen = null;
        private bool _isOpen = false;
        private List<byte> _buffer = new List<byte>();
        private AutoResetEvent _bufferAre = new AutoResetEvent(true);
        private List<byte[]> _sendList = new List<byte[]>();
        private AutoResetEvent _sendListAre = new AutoResetEvent(true);

        private AutoResetEvent _prossReceivedAre = new AutoResetEvent(false);
        private AutoResetEvent _prossSendAre = new AutoResetEvent(false);
        #endregion
        
        #region 私有方法
        /// <summary>
        /// 接收方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            var maxLen = sp.BytesToRead;
            if (maxLen <= 0) return;
            var data = new byte[maxLen];
            var readLen = 0;
            while (readLen < maxLen)
            {
                var len = sp.Read(data, readLen, maxLen);
                readLen += len;
            }
            if (OnBaseReceive != null)
            {
                OnBaseReceive(data.ToArray());
            }
            addToBuffer(data);
        }

        private DateTime lastReceive = DateTime.MinValue;
        /// <summary>
        /// 维护缓冲
        /// </summary>
        /// <param name="data"></param>
        private void addToBuffer(byte[] data)
        {
            _bufferAre.WaitOne();
            if(_buffer.Count>0 && (DateTime.Now- lastReceive).TotalSeconds > 0.1)
            {
                prossError(-10,"指令超時，移除错误命令\r\n"+string.Join(" ", _buffer.Select(a=>a.ToString("X2"))),null);
                _buffer.Clear();
            }
            lastReceive=DateTime.Now;
            _buffer.AddRange(data);
            _bufferAre.Set();
            _prossReceivedAre.Set();
        }

        /// <summary>
        /// 获取一条命令
        /// </summary>
        /// <returns></returns>
        private byte[] getLastCommand()
        {
            byte[] data = null;
            _bufferAre.WaitOne();
            int index=0, count=0;
            if (_buffer.Count <= 0) goto finish;
            var res = getCommondPos(_buffer.ToArray(),out index, out count);
            if (!res) goto finish;
            data = _buffer.Skip(index).Take(count).ToArray();
        finish:
            if (count + index > 0)
            {
                _buffer.RemoveRange(0, index + count);
            }
            _bufferAre.Set();
            return data;
        }
        /// <summary>
        /// 处理接收
        /// </summary>
        private void prossReceived()
        {
            while (running)
            {
                var data = getLastCommand();
                if (data == null)
                {
                    _prossReceivedAre.WaitOne();
                    continue;
                }
                try
                {
                    var res = prossReceiveFunc(data);
                }
                catch (Exception ex)
                {
                    prossError(2, ex.Message, ex);
                }
            }
        }
        /// <summary>
        /// 获取一条发送命令
        /// </summary>
        /// <returns></returns>
        private byte[] getLastSendCommand()
        {
            byte[] data = null;
            _sendListAre.WaitOne();
            if (_sendList.Count <= 0) goto finish;
            data = _sendList[0];
            _sendList.RemoveAt(0);
        finish:
            _sendListAre.Set();
            return data;
        }
        /// <summary>
        /// 处理发送
        /// </summary>
        private void prossSend()
        {
            while (running)
            {
                var data = getLastSendCommand();
                if (data == null)
                {
                    _prossSendAre.WaitOne();
                    continue;
                }
                try
                {
                    _serialPort.Write(data, 0, data.Length);
                    if (SentTimeSpan >0)
                    {
                        Thread.Sleep(SentTimeSpan);
                    }
                    if (OnSent != null) { 
                        OnSent(data);
                    }
                }
                catch (Exception ex)
                {
                    prossError(1, ex.Message, ex);
                }
            }
        }
        /// <summary>
        /// 解析出一条命令
        /// </summary>
        /// <param name="data">缓冲区所有字节</param>
        /// <param name="count">返回一条合法命令的长度</param>
        /// <returns></returns>
        private bool getCommondPos(byte[] data,out int index, out int length)
        {
            if (OnMessageAnalysis != null)
            {
                return OnMessageAnalysis(data, out index,out length);
            }
            index = 0;
            length = data.Length;
            return length > 0;
        }
        /// <summary>
        /// 接收到时运行
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool prossReceiveFunc(byte[] data)
        {
            if (OnReceive != null) return OnReceive(data);
            return true;
        }

        /// <summary>
        /// 处理发送
        /// </summary>
        private void prossOpen()
        {
            while (running)
            {
                Thread.Sleep(10);
                if (_serialPort.IsOpen != _isOpen) {
                    _isOpen = _serialPort.IsOpen;
                    if (_isOpen && OnConnect != null) {
                        OnConnect();
                    }else if(!_isOpen && OnDisconnect != null)
                    {
                        OnDisconnect();
                    }

                }
                if (!_serialPort.IsOpen)
                {
                    try
                    {
                        _serialPort.Open();
                    }
                    catch (Exception ex) {
                        prossError(0,ex.Message,ex);
                    }
                    
                }
                Thread.Sleep(9990);
            }
        }
        /// <summary>
        /// 处理错误，这里是基础库，不要再这里直接用log4net写日志，结构不好
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        protected void prossError(int code,string msg,Exception ex) {
            if (OnError == null) return;

            OnError(code, msg, ex);
        }
        /// <summary>
        /// 析构
        /// </summary>
        ~SerialPortLib()
        {
            running = false;
            _serialPort.Close();
            _prossOpen.Wait();
            _prossReceived.Wait();
            _prossSend.Wait();
            _prossOpen.Dispose();
            _prossReceived.Dispose();
            _prossSend.Dispose();
            _serialPort.Dispose();
            _bufferAre.Dispose();
            _sendListAre.Dispose();
            _prossReceivedAre.Dispose();
            _prossSendAre.Dispose();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 分析一条命令委托
        /// </summary>
        /// <param name="data">要分析的数据</param>
        /// <param name="index">传出分析出的命令开始位置</param>
        /// <param name="length">传出分析出的命令长度</param>
        /// <returns>返回成功失败，成功会去除分析出的命令，失败会抛弃分析出的命令，不要随便抛弃命令，具体看例子怎么用的，除非是明显的错误可以抛弃</returns>
        public delegate bool AnalysisDelegate(byte[] data, out int index,out int length);
        /// <summary>
        /// 分析一条命令事件
        /// </summary>
        public event AnalysisDelegate OnMessageAnalysis;
        /// <summary>
        /// 接收事件 经过分析清洗的严谨命令，一般都用这个
        /// </summary>
        public event Func<byte[],bool> OnReceive;
        /// <summary>
        /// 发送事件
        /// </summary>
        public event Action<byte[]> OnSent;
        /// <summary>
        /// 原始接收到的数据，一般不要用，除非是分析或日志需求
        /// </summary>
        public event Action<byte[]> OnBaseReceive;

        /// <summary>
        /// 断开连接事件
        /// </summary>
        public event Action OnDisconnect;
        /// <summary>
        /// 连接上事件
        /// </summary>
        public event Action OnConnect;
        /// <summary>
        /// 报错事件
        /// </summary>
        public event Action<int,string,Exception> OnError;
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="dataBits"></param>
        /// <param name="readTimeout"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        public SerialPortLib(string portName, int baudRate = 115200, int dataBits = 8, int readTimeout = SerialPort.InfiniteTimeout, Parity parity = Parity.None, StopBits stopBits = StopBits.One) {
            //初始化串口
            _serialPort = new SerialPort();
            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
            _serialPort.DataBits = dataBits;
            _serialPort.ReadTimeout = readTimeout;
            _serialPort.Parity = parity;
            _serialPort.StopBits = stopBits;
            _serialPort.DataReceived += _serialPort_DataReceived;
            //初始化接收处理线程
            _prossReceived = new Task(new Action(prossReceived));
            //初始化发送处理线程
            _prossSend = new Task(new Action(prossSend));

            _prossOpen = new Task(new Action(prossOpen));

            //开启程序
            _prossReceived.Start();
            _prossSend.Start();
            

            try
            {
                _serialPort.Open();
            }
            catch (Exception ex) {
                prossError(0,ex.Message,ex);
            }
            _prossOpen.Start();
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            _sendListAre.WaitOne();
            _sendList.Add(data);
            _sendListAre.Set();
            _prossSendAre.Set();
        }
        /// <summary>
        /// 是否打开
        /// </summary>
        public bool IsOpen {
            get {
                return _serialPort.IsOpen;
            }
        }

        public int SentTimeSpan { get; set; }

        #region 静态函数
        /// <summary>
        /// 公用，相当于数组版的IndexOf，可以识别连续数组
        /// </summary>
        /// <param name="source"></param>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int IndexOfBytes(byte[] source, byte[] data, int start = 0)
        {
            if (source == null || source.Length <= 0 || data == null || data.Length <= 0 || start + data.Length > source.Length) return -1;
            var index = Array.IndexOf(source, data[0], start);
            if (index < 0) return -1;
            if (data.Length == 1) return index;
            for (int i = 1; i < data.Length; i++)
            {
                if (data[i] == source[++start]) continue;
                return IndexOfBytes(source, data, start);
            }
            return index;
        }
        #endregion

        #region 测试方法
        public void TestAddSend(byte[] data) {
            //_buffer.AddRange(data);
            addToBuffer(data);
        }
        #endregion
    }
}
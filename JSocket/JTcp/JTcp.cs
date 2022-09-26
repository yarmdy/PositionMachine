using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace JSocket.JTcp
{
    public class JTcp
    {
        #region 私有属性
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private IPEndPoint _ipep = null;
        private bool _isListen = false;
        private List<Socket> _socketList = new List<Socket>();
        private List<Socket> _prossingSocketList = new List<Socket>();
        private AutoResetEvent _socketListAre = new AutoResetEvent(true);
        private AutoResetEvent _prossingSocketListAre = new AutoResetEvent(true);
        private Task _listenTask = null;
        private List<Task> _prossTaskList = null;
        private ManualResetEvent _prossAre = new ManualResetEvent(false);

        private IPEndPoint _conIpep = null;
        #endregion

        #region 私有方法
        private void addToSocketList(Socket skt)
        {
            _socketListAre.WaitOne();
            _socketList.Add(skt);
            _socketListAre.Set();
            _prossAre.Set();
        }
        private void addToProssingSocketList(Socket skt)
        {
            _prossingSocketListAre.WaitOne();
            _prossingSocketList.Add(skt);
            _prossingSocketListAre.Set();
        }
        private void removeProssingSocketList(Socket skt)
        {
            _prossingSocketListAre.WaitOne();
            _prossingSocketList.Remove(skt);
            _prossingSocketListAre.Set();
        }
        private Socket getSocketClient()
        {
            Socket skt = null;
            _socketListAre.WaitOne();
            if (_socketList.Count <= 0) {
                _prossAre.Reset();
                goto finish;
            };
            skt = _socketList[0];
            _socketList.RemoveAt(0);
        finish:
            _socketListAre.Set();
            return skt;
        }
        private void listen()
        {
            while (_isListen)
            {
                try
                {
                    var skt = _socket.Accept();
                    addToSocketList(skt);
                }
                catch (Exception ex)
                {
                }
            }
        }
        private void pross()
        {
            while (_isListen)
            {
                try
                {
                    var skt = getSocketClient();
                    if (skt == null)
                    {
                        _prossAre.WaitOne();
                        continue;
                    }
                    addToProssingSocketList(skt);
                    OnConnectPross(skt);
                    removeProssingSocketList(skt);
                }
                catch (Exception ex)
                {
                }
            }
        }
        #endregion

        #region 事件
        public event Action<Socket> OnConnect;
        #endregion

        #region 虚函数
        public virtual void OnConnectPross(Socket skt) {
            if (OnConnect == null) return;
            OnConnect(skt);
        }
        #endregion

        public JTcp() {
            
        }
        public JTcp(IPEndPoint ipep)
        {
            if (ipep == null) throw new ArgumentNullException("ipep","不能为空");
            _ipep = ipep;
            _socket.Bind(ipep);
        }
        public IPEndPoint Ipep {
            get { return _ipep; }
        }
        public bool IsBound
        {
            get
            {
                return _socket.IsBound;
            }
        }
        public bool Start(int threadCount) {
            if (!IsBound || _isListen) return false;
            _isListen = true;
            _socket.Listen(0);
            _listenTask = new Task(listen);
            _prossTaskList = new int[threadCount].Select(a => new Task(pross)).ToList();
            _listenTask.Start();
            _prossTaskList.ForEach(a=>a.Start());

            return true;
        }
        public bool Reset(IPEndPoint ipep=null) {
            if (_isListen) {
                _isListen = false;
                _socket.Close();
                _listenTask.Wait();
                _listenTask.Dispose();
                _socketList.ForEach(a=>a.Close());
                _socketList.Clear();
                _prossingSocketList.ForEach(a=>a.Close());
                _prossingSocketList.Clear();
                _prossAre.Set();
                _prossTaskList.ForEach(a => {
                    a.Wait();
                    a.Dispose();
                });
                _prossAre.Reset();
                _listenTask = null;
                _prossTaskList = null;
                
            }
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _ipep = ipep;
            _conIpep = null;
            if (ipep != null)
            {
                _socket.Bind(ipep);
            }
            return true;
        }
        public bool Connect(IPEndPoint ipep) {
            try {
                _socket.SendTimeout = 3000;
                _socket.Connect(ipep);
            } catch (Exception ex) {
                return false;
            }
            if (!_socket.Connected) return false;
            _conIpep = ipep;
            return true;
        }

        public bool Send(byte[] data) {
            var len = _socket.Send(data);
            return len == data.Length;
        }
    }
}

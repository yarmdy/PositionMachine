using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace JSocket
{
    public static class JSocketLib
    {
        public static int ReceiveAll(this Socket skt,byte[] data) {
            if (data == null || data.Length <= 0) return 0;
            var maxLen = data.Length;
            var len = 0;
            while (len < maxLen) {
                var res = skt.Receive(data,len,maxLen,SocketFlags.None);
                len += res;
            }
            return maxLen;
        }

        public static IPAddress[] GetLocalIp() {
            return Dns.GetHostAddresses(Dns.GetHostName());
        }
    }
}

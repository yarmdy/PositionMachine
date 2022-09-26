using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using JSerialPort;

namespace PN532Lib
{
    public class PN532Helper : SerialPortLib
    {
        #region 常量
        /// <summary>
        /// 发送标志
        /// </summary>
        public const byte _c_Send = 0xd4;
        /// <summary>
        /// 接收标志
        /// </summary>
        public const byte _c_Receive = 0xd5;
        /// <summary>
        /// 寻卡命令 发送
        /// </summary>
        public const byte _c_FindCard_s = 0x4a;
        /// <summary>
        /// 寻卡命令 接收
        /// </summary>
        public const byte _c_FindCard_r = 0x4b;
        /// <summary>
        /// 读写 发送
        /// </summary>
        public const byte _c_Valid_s = 0x40;
        /// <summary>
        /// 读写 接收
        /// </summary>
        public const byte _c_Valid_r = 0x41;
        #endregion
        #region 私有
        /// <summary>
        /// 常规命令头
        /// </summary>
        private byte[] phead
        {
            get
            {
                return new[] { (byte)0x00, (byte)0x00, (byte)0xff };
            }
        }
        /// <summary>
        /// 加长命令头
        /// </summary>
        private byte[] pheadL
        {
            get
            {
                return new[] { (byte)0x00, (byte)0x00, (byte)0xff, (byte)0xff, (byte)0xff };
            }
        }
        /// <summary>
        /// 命令尾
        /// </summary>
        private byte[] ptail
        {
            get
            {
                return new[] { (byte)0x00 };
            }
        }
        /// <summary>
        /// 唤醒命令
        /// </summary>
        private byte[] wakeUpData
        {
            get
            {
                return new byte[] { 0x55, 0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0x03, 0xfd, 0xd4, 0x14, 0x01, 0x17, 0x00 };
            }
        }
        /// <summary>
        /// 寻卡命令生成
        /// </summary>
        /// <param name="num"></param>
        /// <param name="br"></param>
        /// <returns></returns>
        private byte[] getFindCardData(EnumFindCardNum num, EnumBaudRate br) { 
            return new byte[4] {_c_Send,_c_FindCard_s,(byte)num,(byte)br };
        }
        /// <summary>
        /// 获取校验数据值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private byte getPlus0(byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("校验包不能为空");
            }
            byte plusV = 0;
            data.ToList().ForEach(a => plusV += a);
            return (byte)((~plusV) + 1);
        }
        
        /// <summary>
        /// 接收事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool PN532Helper_OnReceive(byte[] arg)
        {
            return true;
        }
        /// <summary>
        /// 分析命令 获得一个合法的命令
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private bool PN532Helper_OnMessageAnalysis(byte[] data, out int index, out int length)
        {
            index = 0;
            length = 0;
            var headIndex = IndexOfBytes(data, phead, 0);
            if (headIndex < 0)
            {
                return false;
            }
            if (data.Length < headIndex + 6)
            {
                return false;
            }
            if (data[headIndex + 3] == 0x00 && data[headIndex + 4] == 0xff && data[headIndex + 5] == 0x00 || data[headIndex + 3] == 0xff && data[headIndex + 4] == 0x00 && data[headIndex + 5] == 0x00)
            {
                index = headIndex;
                length = 6;
                return true;
            }
            if ((byte)(data[headIndex + 3] + data[headIndex + 4]) != 0)
            {
                index = headIndex;
                length = 3;
                return false;
            }
            var len = data[headIndex + 3];
            if (data.Length < headIndex + 5 + len + 2)
            {
                return false;
            }
            var datapart = data.Skip(headIndex + 5).Take(len).ToArray();
            var datapart0 = getPlus0(datapart);
            if (datapart0 != data[headIndex + 5 + len])
            {
                index = headIndex;
                length = 3;
                return false;
            }
            if (data[headIndex + 5 + len + 1] != 0)
            {
                index = headIndex;
                length = 3;
                return false;
            }
            index = headIndex;
            length = 3 + 2 + len + 2;
            return true;
        }
        #endregion

        #region 枚举
        /// <summary>
        /// 寻卡数量
        /// </summary>
        public enum EnumFindCardNum : byte { 
            One=0x01,
            Two=0x02
        }
        /// <summary>
        /// 波特率
        /// </summary>
        public enum EnumBaudRate : byte
        {
            BR9600 = 0x00
        }
        #endregion
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="dataBits"></param>
        /// <param name="readTimeout"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        public PN532Helper(string portName, int baudRate = 115200, int dataBits = 8, int readTimeout = -1, Parity parity = Parity.None, StopBits stopBits = StopBits.One) : base(portName, baudRate, dataBits, readTimeout, parity, stopBits)
        {
            OnMessageAnalysis += PN532Helper_OnMessageAnalysis;
            OnReceive += PN532Helper_OnReceive;

        }
        /// <summary>
        /// 创建命令
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public byte[] CreateCommand(byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("数据包不能为空");
            }
            var plen = data.Length;
            var res = new List<byte>();
            if (plen <= 0xff)
            {
                res.AddRange(phead);
                var dlen = (byte)plen;
                var lcs = getPlus0(new byte[] { dlen });
                res.Add(dlen);
                res.Add(lcs);
            }
            else
            {
                res.AddRange(pheadL);
                var dlen = (ushort)plen;
                var blen = new byte[] { (byte)(dlen >> 8), (byte)(dlen << 24 >> 24) };
                var lcs = getPlus0(blen);
                res.AddRange(blen);
                res.Add(lcs);
            }
            var dcs = getPlus0(data);
            res.AddRange(data);
            res.Add(dcs);
            res.AddRange(ptail);
            return res.ToArray();
        }
        /// <summary>
        /// 唤醒
        /// </summary>
        public void WakeUp() {
            Send(wakeUpData);
        }
        /// <summary>
        /// 寻卡
        /// </summary>
        /// <param name="num"></param>
        /// <param name="br"></param>
        public void FindCard(EnumFindCardNum num,EnumBaudRate br)
        {
            var data = getFindCardData(num, br);
            Send(CreateCommand(data));
        }
    }
}

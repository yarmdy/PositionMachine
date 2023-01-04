using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using JSerialPort;

namespace ZEHOU.PM.Label.SerialPort
{
    /// <summary>
    /// 贴标机实现类
    /// </summary>
    public class LabelMachineHelper : LabelMachineHelperBase
    {
        #region 常量
        /// <summary>
        /// crc多项式
        /// </summary>
        private const ushort _c_CRCPolynomial = 0xA001;
        /// <summary>
        /// crc初值
        /// </summary>
        private const ushort _c_InitialValue = 0xFFFF;
        /// <summary>
        /// 下位机id
        /// </summary>
        private const byte _c_LMachineId = 0x50;
        /// <summary>
        /// 回应ok
        /// </summary>
        private const byte _c_RespondOK = 0x01;

        /// <summary>
        /// 非标仓号
        /// </summary>
        private const byte _c_SpecialBinId = 0xFF;
        /// <summary>
        /// 回执单仓号
        /// </summary>
        private const byte _c_BackOrderBinId = 0xFE;

        #endregion

        #region 私有
        /// <summary>
        /// 常规命令头
        /// </summary>
        private byte[] phead
        {
            get
            {
                return new[] { (byte)'@', (byte)'Z', (byte)'H' };
            }
        }
        
        /// <summary>
        /// 命令尾
        /// </summary>
        private byte[] ptail
        {
            get
            {
                return new[] { (byte)'\r',(byte)'\n' };
            }
        }

        
        /// <summary>
        /// 获取校验数据值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private byte[] getCRC16(byte[] data)
        {
            byte CRC16Lo;
            byte CRC16Hi;   //CRC寄存器           
            byte CL; byte CH;       //多项式码&HA001             
            byte SaveHi; byte SaveLo;
            byte[] tmpData;
            //int I;
            int Flag;
            CRC16Lo = 0xFF;
            CRC16Hi = 0xFF;
            CL = 0x01; //00000001
            CH = 0xA0;// 10100000
            tmpData = data;
            for (int i = 0; i < tmpData.Length; i++)
            {
                CRC16Lo = (byte)(CRC16Lo ^ tmpData[i]); //每一个数据与CRC寄存器进行异或       
                for (Flag = 0; Flag <= 7; Flag++)
                {
                    SaveHi = CRC16Hi;
                    SaveLo = CRC16Lo;
                    CRC16Hi = (byte)(CRC16Hi >> 1);      //高位右移一位   
                    CRC16Lo = (byte)(CRC16Lo >> 1);      //低位右移一位        
                    if ((SaveHi & 0x01) == 0x01) //如果高位字节最后一位为1       
                    {
                        CRC16Lo = (byte)(CRC16Lo | 0x80);   //则低位字节右移后前面补1     
                    }             //否则自动补0                
                    if ((SaveLo & 0x01) == 0x01) //如果LSB为1，则与多项式码进行异或                    
                    {
                        CRC16Hi = (byte)(CRC16Hi ^ CH);
                        CRC16Lo = (byte)(CRC16Lo ^ CL);
                    }
                }
            }
            byte[] ReturnData = new byte[2];
            ReturnData[1] = CRC16Hi;       //CRC高位       
            ReturnData[0] = CRC16Lo;       //CRC低位     
            return ReturnData;
        }

        /// <summary>
        /// 接收事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool PN532Helper_OnReceive(byte[] arg)
        {
            if (AfterReceive != null)
            {
                AfterReceive(arg);
            }
            try
            {
                var dataPackage = new DataPackage();
                dataPackage.Func = arg[5];
                dataPackage.Comm = arg[6];
                dataPackage.CommId = arg[7];
                dataPackage.Len = BitConverter.ToUInt16(arg, 8);
                dataPackage.Data = arg.Skip(10).Take(dataPackage.Len).ToArray();

                if (dataPackage.Func == (byte)EnumFunc.GOWORK)
                {
                    switch (dataPackage.Comm)
                    {
                        case (byte)EnumGOWORKComm.DOLABEL:
                            {
                                if (OnBackLabel != null)
                                {
                                    OnBackLabel(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumGOWORKComm.LABELLIST:
                            {
                                if (OnBackLabelList != null)
                                {
                                    OnBackLabelList(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumGOWORKComm.CANCELLIST:
                            {
                                if (OnBackCancelList != null)
                                {
                                    OnBackCancelList(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumGOWORKComm.SPECIALLABEL:
                            {
                                if (OnBackSpecialLabel != null)
                                {
                                    OnBackSpecialLabel(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumGOWORKComm.BACKORDER:
                            {
                                if (OnBackBackOrder != null)
                                {
                                    OnBackBackOrder(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumGOWORKComm.LIGHTSTATUS:
                            {
                                if (OnBackLightTest != null)
                                {
                                    OnBackLightTest(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumGOWORKComm.DROPTUBECONFIRM:
                            {
                                if (OnBackDropTubeConfirm != null)
                                {
                                    OnBackDropTubeConfirm(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumGOWORKComm.FILLBIN:
                            {
                                if (OnBackFillBin != null)
                                {
                                    OnBackFillBin(dataPackage);
                                }
                            }
                            break;
                    }
                }
                else if (dataPackage.Func == (byte)EnumFunc.READPARAM)
                {
                    switch (dataPackage.Comm)
                    {
                        case (byte)EnumREADPARAMComm.READPARAM:
                            {
                                if (OnBackReadParams != null)
                                {
                                    OnBackReadParams(dataPackage);
                                }
                            }
                            break;
                    }
                }
                else if (dataPackage.Func == (byte)EnumFunc.EDITPARAM)
                {
                    switch (dataPackage.Comm)
                    {
                        case (byte)EnumEDITPARAMComm.SAVEPARAM:
                            {
                                if (OnBackSaveParams != null)
                                {
                                    OnBackSaveParams(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumEDITPARAMComm.APPLYPARAM:
                            {
                                if (OnBackApplyParams != null)
                                {
                                    OnBackApplyParams(dataPackage);
                                }
                            }
                            break;
                    }
                }
                else if (dataPackage.Func == (byte)EnumFunc.TEST)
                {
                    switch (dataPackage.Comm)
                    {
                        case (byte)EnumTESTComm.VERTICALMOTOR:
                            {
                                if (OnBackVerticalMotor != null)
                                {
                                    OnBackVerticalMotor(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumTESTComm.PINCHROLLERMOTOR:
                            {
                                if (OnBackPinchRollerMotor != null)
                                {
                                    OnBackPinchRollerMotor(dataPackage);
                                }
                            }
                            break;

                        case (byte)EnumTESTComm.HORIZONTALMOTOR:
                            {
                                if (OnBackHorizontalMotor != null)
                                {
                                    OnBackHorizontalMotor(dataPackage);
                                }
                            }
                            break;

                        case (byte)EnumTESTComm.MAINMOTOR:
                            {
                                if (OnBackMainMotor != null)
                                {
                                    OnBackMainMotor(dataPackage);
                                }
                            }
                            break;

                        case (byte)EnumTESTComm.MANIPULATOR:
                            {
                                if (OnBackManipulator != null)
                                {
                                    OnBackManipulator(dataPackage);
                                }
                            }
                            break;

                        case (byte)EnumTESTComm.CARD:
                            {
                                if (OnBackCard != null)
                                {
                                    OnBackCard(dataPackage);
                                }
                            }
                            break;

                        case (byte)EnumTESTComm.EXITMOTOR:
                            {
                                if (OnBackExitMotor != null)
                                {
                                    OnBackExitMotor(dataPackage);
                                }
                            }
                            break;

                        case (byte)EnumTESTComm.BIN:
                            {
                                if (OnBackBin != null)
                                {
                                    OnBackBin(dataPackage);
                                }
                            }
                            break;

                    }
                }
                else if (dataPackage.Func == (byte)EnumFunc.UPWORKRES)
                {
                    switch (dataPackage.Comm)
                    {
                        case (byte)EnumUPWORKRESComm.YOURTURN:
                            {
                                Reply(dataPackage.Func, dataPackage.Comm, dataPackage.CommId);
                                if (OnMyTurn != null)
                                {
                                    OnMyTurn(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumUPWORKRESComm.LABELSTATUS:
                            {
                                Reply(dataPackage.Func, dataPackage.Comm, dataPackage.CommId);
                                if (OnLabelStatus != null)
                                {
                                    OnLabelStatus(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumUPWORKRESComm.EMPTYBIN:
                            {
                                Reply(dataPackage.Func, dataPackage.Comm, dataPackage.CommId);
                                if (OnEmptyBin != null)
                                {
                                    OnEmptyBin(dataPackage);
                                }
                            }
                            break;
                    }
                }
                else if (dataPackage.Func == (byte)EnumFunc.UPPARAM)
                {
                    switch (dataPackage.Comm)
                    {
                        case (byte)EnumUPPARAMComm.PARAMS:
                            {
                                if (OnUpParam != null)
                                {
                                    OnUpParam(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumUPPARAMComm.MOTORSTEPS:
                            {
                                if (OnUpMotorSteps != null)
                                {
                                    OnUpMotorSteps(dataPackage);
                                }
                            }
                            break;
                    }
                }
                else if (dataPackage.Func == (byte)EnumFunc.UPSTATUS)
                {
                    switch (dataPackage.Comm)
                    {
                        case (byte)EnumUPSTATUSComm.LIGHT:
                            {
                                if (OnLightStatus != null)
                                {
                                    OnLightStatus(dataPackage);
                                }
                            }
                            break;
                    }
                }
                else if (dataPackage.Func == (byte)EnumFunc.BOOT)
                {
                    switch (dataPackage.Comm)
                    {
                        case (byte)EnumBootComm.ENTERSYSTEM:
                            {
                                if (OnEnterSystem != null)
                                {
                                    OnEnterSystem(dataPackage);
                                }
                            }
                            break;
                        case (byte)EnumBootComm.WRITEBIN:
                            {
                                if (OnBackWriteBin != null)
                                {
                                    OnBackWriteBin(dataPackage);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    //do nothing
                }
                return true;
            }
            catch (Exception ex)
            {
                prossError(-99, ex.Message, ex);
                return false;
            }
        }
        /// <summary>
        /// 分析命令 获得一个合法的命令 这个例子诠释了分析事件的用法
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private bool PN532Helper_OnMessageAnalysis(byte[] data, out int index, out int length)
        {
            index = 0;
            length = 0;
            if (data.Length < 15) {
                return false;
            }
            var headIndex = IndexOfBytes(data, phead, 0);
            if (headIndex < 0)
            {
                prossError(-1,"找不到头,忽略\r\n"+String.Join(" ",data.Select(a=>a.ToString("X2"))),null);
                index = 0;
                length = data.Length;
                return false;
            }
            if (data.Length < headIndex + 15)
            {
                //prossError(-1, "找到的太少,等待\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
                return false;
            }

            if (data[headIndex + 3]!= MachineId || data[headIndex + 4]!=_c_LMachineId)
            {
                prossError(-1, "机器码不匹配，忽略指令\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
                index = headIndex;
                length = 3;
                return false;
            }

            var len = BitConverter.ToUInt16(data, headIndex + 8);
            if(data.Length< headIndex + 8 + 2 + len+2+2)
            {
                //prossError(-1, "长度不对,等待\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
                return false;
            }
            var crc = getCRC16(data.Skip(headIndex).Take(10+len).ToArray());
            if (IndexOfBytes(data, crc, headIndex + 10 + len) != headIndex + 10 + len)
            {
                prossError(-1, "校验失败,忽略指令\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
                index = headIndex;
                length = 3;
                return false;
            }
            if (IndexOfBytes(data, ptail, headIndex + 10 + len + 2) != headIndex + 10 + len + 2) {
                prossError(-1, "尾部不匹配，忽略指令\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
                index = headIndex;
                length = 3;
                return false;
            }

            //prossError(-1, "匹配成功\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
            index = headIndex;
            length = 10 + len + 2 + 2;
            return true;
        }
        /// <summary>
        /// 命令号
        /// </summary>
        private byte _commId = 0;
        /// <summary>
        /// 获取一条命令号
        /// </summary>
        /// <returns></returns>
        private byte getCommId()
        {
            return _commId++;
        }
        /// <summary>
        /// ascii码转换，自己私有的方法，不要公用，不同用途，结构不好 必要过多的引用依赖方便维护
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private byte[] gb2312Data(string code) {
            return Encoding.GetEncoding("gb2312").GetBytes(code);
        }
        #endregion

        #region 枚举
        /// <summary>
        /// 功能枚举
        /// </summary>
        private enum EnumFunc : byte { 
            /// <summary>
            /// 工作命令
            /// </summary>
            GOWORK=0XA1,
            /// <summary>
            /// 读取参数
            /// </summary>
            READPARAM=0XA2,
            /// <summary>
            /// 修改参数
            /// </summary>
            EDITPARAM = 0XA3,
            /// <summary>
            /// 测试命令
            /// </summary>
            TEST = 0XA4,
            /// <summary>
            /// 上传工作结果
            /// </summary>
            UPWORKRES=0XD1,
            /// <summary>
            /// 上传参数
            /// </summary>
            UPPARAM =0XD2,
            /// <summary>
            /// 上传状态
            /// </summary>
            UPSTATUS=0XD3,
            /// <summary>
            /// 启动
            /// </summary>
            BOOT = 0XFF
        }
        /// <summary>
        /// 工作命令
        /// </summary>
        private enum EnumGOWORKComm : byte
        {
            /// <summary>
            /// 贴标
            /// </summary>
            DOLABEL = 0X01,
            /// <summary>
            /// 贴标清单
            /// </summary>
            LABELLIST = 0X02,
            /// <summary>
            /// 取消清单
            /// </summary>
            CANCELLIST = 0X03,
            /// <summary>
            /// 非标
            /// </summary>
            SPECIALLABEL = 0X04,
            /// <summary>
            /// 回执单
            /// </summary>
            BACKORDER = 0X05,
            /// <summary>
            /// 开关状态
            /// </summary>
            LIGHTSTATUS = 0X06,
            /// <summary>
            /// 掉管确认
            /// </summary>
            DROPTUBECONFIRM = 0X07,
            /// <summary>
            /// 补仓
            /// </summary>
            FILLBIN = 0X08
        }
        /// <summary>
        /// 读取参数命令
        /// </summary>
        private enum EnumREADPARAMComm : byte {
            /// <summary>
            /// 读取参数
            /// </summary>
            READPARAM = 0X01,
        }
        /// <summary>
        /// 修改参数命令
        /// </summary>
        private enum EnumEDITPARAMComm : byte
        {
            /// <summary>
            /// 保存参数
            /// </summary>
            SAVEPARAM = 0X01,
            /// <summary>
            /// 同步参数
            /// </summary>
            APPLYPARAM = 0X02,
        }
        /// <summary>
        /// 测试命令
        /// </summary>
        private enum EnumTESTComm : byte
        {
            /// <summary>
            /// 纵向电机
            /// </summary>
            VERTICALMOTOR = 0X01,
            /// <summary>
            /// 压轮电机
            /// </summary>
            PINCHROLLERMOTOR = 0X02,
            /// <summary>
            /// 横向电机
            /// </summary>
            HORIZONTALMOTOR = 0X03,
            /// <summary>
            /// 主轮电机
            /// </summary>
            MAINMOTOR = 0X04,
            /// <summary>
            /// 机械手
            /// </summary>
            MANIPULATOR = 0X05,
            /// <summary>
            /// 卡片
            /// </summary>
            CARD = 0X06,
            /// <summary>
            /// 出口电机
            /// </summary>
            EXITMOTOR = 0X07,
            /// <summary>
            /// 料仓
            /// </summary>
            BIN = 0X08,
            // <summary>
            /// 料仓
            /// </summary>
            MOTOR = 0X09,
        }

        /// <summary>
        /// 上报工作状态
        /// </summary>
        private enum EnumUPWORKRESComm : byte
        {
            /// <summary>
            /// 到你了
            /// </summary>
            YOURTURN = 0X01,
            /// <summary>
            /// 贴标状态
            /// </summary>
            LABELSTATUS = 0X02,
            /// <summary>
            /// 空仓了
            /// </summary>
            EMPTYBIN = 0X03

        }
        /// <summary>
        /// 参数上报
        /// </summary>
        private enum EnumUPPARAMComm : byte
        {
            /// <summary>
            /// 参数
            /// </summary>
            PARAMS = 0X01,
            /// <summary>
            /// 电机步数
            /// </summary>
            MOTORSTEPS = 0X02,
        }
        /// <summary>
        /// 开关状态上报
        /// </summary>
        private enum EnumUPSTATUSComm : byte {
            /// <summary>
            /// 光电状态
            /// </summary>
            LIGHT = 0X01,
        }

        /// <summary>
        /// 启动
        /// </summary>
        private enum EnumBootComm : byte
        {
            /// <summary>
            /// 进入系统
            /// </summary>
            ENTERSYSTEM = 0X01,
            /// <summary>
            /// 刷写固件
            /// </summary>
            WRITEBIN = 0X02,
            /// <summary>
            /// 进入引导
            /// </summary>
            ENTERBOOT = 0XFF
        }

        #endregion

        #region 事件
        /// <summary>
        /// 反馈贴标命令
        /// </summary>
        public override event Action<DataPackage> OnBackLabel;
        /// <summary>
        /// 反馈清单命令
        /// </summary>
        public override event Action<DataPackage> OnBackLabelList;
        /// <summary>
        /// 反馈取消清单命令
        /// </summary>
        public override event Action<DataPackage> OnBackCancelList;
        /// <summary>
        /// 反馈非标命令
        /// </summary>
        public override event Action<DataPackage> OnBackSpecialLabel;
        /// <summary>
        /// 反馈回执单命令
        /// </summary>
        public override event Action<DataPackage> OnBackBackOrder;
        /// <summary>
        /// 反馈光电测试命令
        /// </summary>
        public override event Action<DataPackage> OnBackLightTest;
        /// <summary>
        /// 轮到我了
        /// </summary>
        public override event Action<DataPackage> OnMyTurn;
        /// <summary>
        /// 贴标状态
        /// </summary>
        public override event Action<DataPackage> OnLabelStatus;
        /// <summary>
        /// 光电状态
        /// </summary>
        public override event Action<DataPackage> OnLightStatus;
        /// <summary>
        /// 反馈掉管确认
        /// </summary>
        public override event Action<DataPackage> OnBackDropTubeConfirm;
        /// <summary>
        /// 反馈补管
        /// </summary>
        public override event Action<DataPackage> OnBackFillBin;

        /// <summary>
        /// 读取参数
        /// </summary>
        public override event Action<DataPackage> OnBackReadParams;
        /// <summary>
        /// 保存参数
        /// </summary>
        public override event Action<DataPackage> OnBackSaveParams;
        /// <summary>
        /// 同步参数
        /// </summary>
        public override event Action<DataPackage> OnBackApplyParams;


        /// <summary>
        /// 纵向电机
        /// </summary>
        public override event Action<DataPackage> OnBackVerticalMotor;
        /// <summary>
        /// 压轮电机
        /// </summary>
        public override event Action<DataPackage> OnBackPinchRollerMotor;
        /// <summary>
        /// 横向电机
        /// </summary>
        public override event Action<DataPackage> OnBackHorizontalMotor;
        /// <summary>
        /// 主轮电机
        /// </summary>
        public override event Action<DataPackage> OnBackMainMotor;
        /// <summary>
        /// 机械手
        /// </summary>
        public override event Action<DataPackage> OnBackManipulator;
        /// <summary>
        /// 卡片
        /// </summary>
        public override event Action<DataPackage> OnBackCard;
        /// <summary>
        /// 出口电机
        /// </summary>
        public override event Action<DataPackage> OnBackExitMotor;
        /// <summary>
        /// 料仓
        /// </summary>
        public override event Action<DataPackage> OnBackBin;

        /// <summary>
        /// 上传参数
        /// </summary>
        public override event Action<DataPackage> OnUpParam;
        /// <summary>
        /// 上传步数
        /// </summary>
        public override event Action<DataPackage> OnUpMotorSteps;
        /// <summary>
        /// 接收前
        /// </summary>
        public override event Action<byte[]> AfterReceive;
        /// <summary>
        /// 空仓
        /// </summary>
        public override event Action<DataPackage> OnEmptyBin;
        /// <summary>
        /// 请求进入系统
        /// </summary>
        public override event Action<DataPackage> OnEnterSystem;
        /// <summary>
        /// 返回刷写固件
        /// </summary>
        public override event Action<DataPackage> OnBackWriteBin;

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
        public LabelMachineHelper(string portName, int baudRate = 115200, int dataBits = 8, int readTimeout = -1, Parity parity = Parity.None, StopBits stopBits = StopBits.One) : base(portName, baudRate, dataBits, readTimeout, parity, stopBits)
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
        private byte[] CreateCommand(byte func,byte comm,byte commId,byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("数据包不能为空");
            }
            var res = new List<byte>();
            res.AddRange(phead);
            res.Add(_c_LMachineId);
            res.Add(MachineId);
            res.Add(func);
            res.Add(comm);
            res.Add(commId);
            var len = (ushort)data.Length;

            var blen = BitConverter.GetBytes(len).ToArray();
            res.AddRange(blen);
            res.AddRange(data);
            var crc = getCRC16(res.ToArray());
            res.AddRange(crc);
            res.AddRange(ptail);
            return res.ToArray();
        }

        /// <summary>
        /// 贴标
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="code"></param>
        /// <param name="printData"></param>
        public override byte StartLabel(byte[] binIds, string code, byte[] printData) { 
            var data = new List<byte>();
            data.Add((byte)binIds.Length);
            data.AddRange(binIds.OrderBy(a=>a).ToArray());
            var codeData = gb2312Data(code);
            var codeLen = (byte)codeData.Length;
            var printLen = BitConverter.GetBytes((ushort)printData.Length);
            data.Add(codeLen);
            data.AddRange(codeData);
            data.AddRange(printLen);
            data.AddRange(printData);
            var commId = getCommId();
            var comm = CreateCommand((byte)EnumFunc.GOWORK,(byte)EnumGOWORKComm.DOLABEL,commId,data.ToArray());
            Send(comm);
            return commId;
        }
        /// <summary>
        /// 开始清单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="num"></param>
        /// <param name="binNum"></param>
        /// <param name="binLbNum"></param>
        public override byte StartLabelList(byte orderId, byte num, byte binNum, byte[] binLbNum) {
            var data = new List<byte>();
            data.Add(orderId);
            data.Add(num);
            data.Add(binNum);
            if (binLbNum == null)
            {
                binLbNum = new byte[0];
            }
            data.AddRange(binLbNum.Take(binNum));
            if (binLbNum.Length < binNum) {
                data.AddRange(new byte[binNum-binLbNum.Length]);
            }
            var commId = getCommId();
            var comm = CreateCommand((byte)EnumFunc.GOWORK,(byte)EnumGOWORKComm.LABELLIST,commId,data.ToArray());
            Send(comm);
            return commId;
        }
        /// <summary>
        /// 取消清单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="num"></param>
        /// <param name="binNum"></param>
        /// <param name="binLbNum"></param>
        public override byte CancelLabelList(byte orderId)
        {
            var data = new List<byte>();
            data.Add(orderId);
            var commId = getCommId();
            var comm = CreateCommand((byte)EnumFunc.GOWORK, (byte)EnumGOWORKComm.CANCELLIST, commId, data.ToArray());
            Send(comm);
            return commId;
        }
        /// <summary>
        /// 打印非标
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="code"></param>
        /// <param name="printData"></param>
        public override byte StartSpecialLabel(string code, byte[] printData)
        {
            var data = new List<byte>();
            data.Add(_c_SpecialBinId);
            var codeData = gb2312Data(code);
            var codeLen = (byte)codeData.Length;
            var printLen = BitConverter.GetBytes((ushort)printData.Length);
            data.Add(codeLen);
            data.AddRange(codeData);
            data.AddRange(printLen);
            data.AddRange(printData);
            var commId = getCommId();
            var comm = CreateCommand((byte)EnumFunc.GOWORK, (byte)EnumGOWORKComm.SPECIALLABEL, commId, data.ToArray());
            Send(comm);
            return commId;
        }
        /// <summary>
        /// 打印回执单
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="code"></param>
        /// <param name="printData"></param>
        public override byte StartBackOrder(string code, byte[] printData)
        {
            var data = new List<byte>();
            data.Add(_c_BackOrderBinId);
            var codeData = gb2312Data(code);
            var codeLen = (byte)codeData.Length;
            var printLen = BitConverter.GetBytes((ushort)printData.Length);
            data.Add(codeLen);
            data.AddRange(codeData);
            data.AddRange(printLen);
            data.AddRange(printData);
            var commId = getCommId();
            var comm = CreateCommand((byte)EnumFunc.GOWORK, (byte)EnumGOWORKComm.BACKORDER, commId, data.ToArray());
            Send(comm);
            return commId;
        }

        /// <summary>
        /// 测试光电
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="num"></param>
        /// <param name="binNum"></param>
        /// <param name="binLbNum"></param>
        public override byte StartLightTest(EnumOpenClose openClose)
        {
            var data = new List<byte>();
            data.Add((byte)openClose);
            var commId = getCommId();
            var comm = CreateCommand((byte)EnumFunc.GOWORK, (byte)EnumGOWORKComm.LIGHTSTATUS, commId, data.ToArray());
            Send(comm);
            return commId;
        }
        /// <summary>
        /// 读取参数命令
        /// </summary>
        public override byte ReadParam() {
            var data = new List<byte>();
            data.Add((byte)0x00);
            var commId = getCommId();
            var comm = CreateCommand((byte)EnumFunc.READPARAM, (byte)EnumREADPARAMComm.READPARAM, commId, data.ToArray());
            Send(comm);
            return commId;
        }
        /// <summary>
        /// 保存参数命令
        /// </summary>
        /// <param name="length"></param>
        /// <param name="array"></param>
        public override byte SaveParam(ushort[] array)
        {
            var data = new List<byte>();
            array.ToList().ForEach(a=>data.AddRange(BitConverter.GetBytes(a)));
            var commId = getCommId();
            var comm = CreateCommand((byte)EnumFunc.EDITPARAM, (byte)EnumEDITPARAMComm.SAVEPARAM, commId, data.ToArray());
            Send(comm);
            return commId;
        }
        /// <summary>
        /// 同步参数命令
        /// </summary>
        /// <param name="length"></param>
        /// <param name="array"></param>
        public override byte ApplyParam(ushort[] array)
        {
            var data = new List<byte>();
            array.ToList().ForEach(a => data.AddRange(BitConverter.GetBytes(a)));
            var commId = getCommId();
            var comm = CreateCommand((byte)EnumFunc.EDITPARAM, (byte)EnumEDITPARAMComm.APPLYPARAM, commId, data.ToArray());
            Send(comm);
            return commId;
        }
        /// <summary>
        /// 测试命令
        /// </summary>
        /// <param name="length"></param>
        /// <param name="array"></param>
        public override byte TestMachine(byte comm,byte act)
        {
            var data = new List<byte>();
            data.Add(act);
            var commId = getCommId();
            var commdata = CreateCommand((byte)EnumFunc.TEST, comm, commId, data.ToArray());
            Send(commdata);
            return commId;
        }
        /// <summary>
        /// 测试料仓命令
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="comm"></param>
        /// <param name="act"></param>
        public override byte TestBin(byte binId, byte act)
        {
            var data = new List<byte>();
            data.Add(binId);
            data.Add(act);
            var commId = getCommId();
            var commdata = CreateCommand((byte)EnumFunc.TEST, (byte)EnumTESTComm.BIN, commId, data.ToArray());
            Send(commdata);
            return commId;
        }
        /// <summary>
        /// 故障确认
        /// </summary>
        /// <param name="act"></param>
        public override byte FaultConfirm(byte act)
        {
            var data = new List<byte>();
            data.Add(act);
            var commId = getCommId();
            var commdata = CreateCommand((byte)EnumFunc.GOWORK, (byte)EnumGOWORKComm.DROPTUBECONFIRM, commId, data.ToArray());
            Send(commdata);
            return commId;
        }
        /// <summary>
        /// 补仓
        /// </summary>
        /// <param name="binno"></param>
        /// <returns></returns>
        public override byte FillBin(byte binno)
        {
            var data = new List<byte>();
            data.Add(binno);
            var commId = getCommId();
            var commdata = CreateCommand((byte)EnumFunc.GOWORK, (byte)EnumGOWORKComm.FILLBIN, commId, data.ToArray());
            Send(commdata);
            return commId;
        }
        /// <summary>
        /// 对下位机反馈
        /// </summary>
        /// <param name="func"></param>
        /// <param name="comm"></param>
        /// <param name="commId"></param>
        private void Reply(byte func, byte comm, byte commId) {
            var data = new List<byte>();
            data.Add(_c_RespondOK);
            var str = CreateCommand(func, comm, commId, data.ToArray());
            Send(str);
        }

        public override byte TestMotor(byte act, short val)
        {
            var data = new List<byte>();
            data.Add(act);
            data.AddRange(BitConverter.GetBytes(val));
            var commId = getCommId();
            var commdata = CreateCommand((byte)EnumFunc.TEST, (byte)EnumTESTComm.MOTOR, commId, data.ToArray());
            Send(commdata);
            return commId;
        }

        public override byte EnterSystem(byte sysNo)
        {
            var data = new List<byte>();
            data.Add(sysNo);
            var commId = getCommId();
            var commdata = CreateCommand((byte)EnumFunc.BOOT, (byte)EnumBootComm.ENTERSYSTEM, commId, data.ToArray());
            Send(commdata);
            return commId;
        }
        public override byte WriteBin(uint addr, byte[] binData)
        {
            var data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(addr));
            data.AddRange(BitConverter.GetBytes((ushort)binData.Length));
            data.AddRange(binData);
            var commId = getCommId();
            var commdata = CreateCommand((byte)EnumFunc.BOOT, (byte)EnumBootComm.WRITEBIN, commId, data.ToArray());
            Send(commdata);
            return commId;
        }
        public override byte EnterBoot()
        {
            var data = new List<byte>();
            data.Add(1);
            var commId = getCommId();
            var commdata = CreateCommand((byte)EnumFunc.BOOT, (byte)EnumBootComm.ENTERBOOT, commId, data.ToArray());
            Send(commdata);
            return commId;
        }
    }
}

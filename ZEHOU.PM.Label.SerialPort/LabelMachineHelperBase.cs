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
    public abstract class LabelMachineHelperBase : SerialPortLib
    {
        #region 枚举
        /// <summary>
        /// 开关枚举
        /// </summary>
        public enum EnumOpenClose : byte { 
            CLOSE=0x00,
            OPEN=0x01
        }

        #endregion

        #region 事件
        /// <summary>
        /// 反馈贴标命令
        /// </summary>
        public abstract event Action<DataPackage> OnBackLabel;
        /// <summary>
        /// 反馈清单命令
        /// </summary>
        public abstract event Action<DataPackage> OnBackLabelList;
        /// <summary>
        /// 反馈取消清单命令
        /// </summary>
        public abstract event Action<DataPackage> OnBackCancelList;
        /// <summary>
        /// 反馈非标命令
        /// </summary>
        public abstract event Action<DataPackage> OnBackSpecialLabel;
        /// <summary>
        /// 反馈回执单命令
        /// </summary>
        public abstract event Action<DataPackage> OnBackBackOrder;
        /// <summary>
        /// 反馈光电测试命令
        /// </summary>
        public abstract event Action<DataPackage> OnBackLightTest;

        /// <summary>
        /// 反馈掉管确认
        /// </summary>
        public abstract event Action<DataPackage> OnBackDropTubeConfirm;
        /// <summary>
        /// 反馈补仓
        /// </summary>
        public abstract event Action<DataPackage> OnBackFillBin;
        /// <summary>
        /// 轮到我了
        /// </summary>
        public abstract event Action<DataPackage> OnMyTurn;
        /// <summary>
        /// 贴标状态
        /// </summary>
        public abstract event Action<DataPackage> OnLabelStatus;
        /// <summary>
        /// 光电状态
        /// </summary>
        public abstract event Action<DataPackage> OnLightStatus;
        /// <summary>
        /// 空仓
        /// </summary>
        public abstract event Action<DataPackage> OnEmptyBin;

        /// <summary>
        /// 读取参数
        /// </summary>
        public abstract event Action<DataPackage> OnBackReadParams;
        /// <summary>
        /// 保存参数
        /// </summary>
        public abstract event Action<DataPackage> OnBackSaveParams;
        /// <summary>
        /// 同步参数
        /// </summary>
        public abstract event Action<DataPackage> OnBackApplyParams;


        /// <summary>
        /// 纵向电机
        /// </summary>
        public abstract event Action<DataPackage> OnBackVerticalMotor;
        /// <summary>
        /// 压轮电机
        /// </summary>
        public abstract event Action<DataPackage> OnBackPinchRollerMotor;
        /// <summary>
        /// 横向电机
        /// </summary>
        public abstract event Action<DataPackage> OnBackHorizontalMotor;
        /// <summary>
        /// 主轮电机
        /// </summary>
        public abstract event Action<DataPackage> OnBackMainMotor;
        /// <summary>
        /// 机械手
        /// </summary>
        public abstract event Action<DataPackage> OnBackManipulator;
        /// <summary>
        /// 卡片
        /// </summary>
        public abstract event Action<DataPackage> OnBackCard;
        /// <summary>
        /// 出口电机
        /// </summary>
        public abstract event Action<DataPackage> OnBackExitMotor;
        /// <summary>
        /// 料仓
        /// </summary>
        public abstract event Action<DataPackage> OnBackBin;

        /// <summary>
        /// 上传参数
        /// </summary>
        public abstract event Action<DataPackage> OnUpParam;
        /// <summary>
        /// 上传步数
        /// </summary>
        public abstract event Action<DataPackage> OnUpMotorSteps;
        /// <summary>
        /// 接收前
        /// </summary>
        public abstract event Action<byte[]> AfterReceive;
        /// <summary>
        /// 返回刷写固件
        /// </summary>
        public abstract event Action<DataPackage> OnBackWriteBin;
        /// <summary>
        /// 请求进入系统
        /// </summary>
        public abstract event Action<DataPackage> OnEnterSystem;


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
        public LabelMachineHelperBase(string portName, int baudRate = 115200, int dataBits = 8, int readTimeout = -1, Parity parity = Parity.None, StopBits stopBits = StopBits.One) : base(portName, baudRate, dataBits, readTimeout, parity, stopBits)
        {
            
        }

        /// <summary>
        /// 机器Id
        /// </summary>
        public byte MachineId { get; set; }

        /// <summary>
        /// 贴标
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="code"></param>
        /// <param name="printData"></param>
        public abstract byte StartLabel(byte[] binIds, string code, byte[] printData);
        /// <summary>
        /// 开始清单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="num"></param>
        /// <param name="binNum"></param>
        /// <param name="binLbNum"></param>
        public abstract byte StartLabelList(byte orderId, byte num, byte binNum, byte[] binLbNum);
        /// <summary>
        /// 取消清单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="num"></param>
        /// <param name="binNum"></param>
        /// <param name="binLbNum"></param>
        public abstract byte CancelLabelList(byte orderId);
        /// <summary>
        /// 打印非标
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="code"></param>
        /// <param name="printData"></param>
        public abstract byte StartSpecialLabel(string code, byte[] printData);
        /// <summary>
        /// 打印回执单
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="code"></param>
        /// <param name="printData"></param>
        public abstract byte StartBackOrder(string code, byte[] printData);

        /// <summary>
        /// 测试光电
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="num"></param>
        /// <param name="binNum"></param>
        /// <param name="binLbNum"></param>
        public abstract byte StartLightTest(EnumOpenClose openClose);
        /// <summary>
        /// 读取参数命令
        /// </summary>
        public abstract byte ReadParam();
        /// <summary>
        /// 保存参数命令
        /// </summary>
        /// <param name="length"></param>
        /// <param name="array"></param>
        public abstract byte SaveParam(ushort[] array);
        /// <summary>
        /// 同步参数命令
        /// </summary>
        /// <param name="length"></param>
        /// <param name="array"></param>
        public abstract byte ApplyParam(ushort[] array);
        /// <summary>
        /// 测试命令
        /// </summary>
        /// <param name="length"></param>
        /// <param name="array"></param>
        public abstract byte TestMachine(byte comm, byte act);
        /// <summary>
        /// 测试料仓命令
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="comm"></param>
        /// <param name="act"></param>
        public abstract byte TestBin(byte binId, byte act);

        /// <summary>
        /// 测试电机
        /// </summary>
        /// <param name="binId"></param>
        /// <param name="comm"></param>
        /// <param name="act"></param>
        public abstract byte TestMotor(byte act, short val);

        /// <summary>
        /// 故障确认
        /// </summary>
        /// <param name="act"></param>
        public abstract byte FaultConfirm(byte act);

        /// <summary>
        /// 补仓
        /// </summary>
        /// <param name="act"></param>
        public abstract byte FillBin(byte binno);

        public abstract byte EnterSystem(byte sysNo);
        public abstract byte WriteBin(uint addr, byte[] binData);
    }
    /// <summary>
    /// 传输数据包
    /// </summary>

    public class DataPackage
    { 
        public byte Func { get; set; }
        public byte Comm { get; set; }
        public byte CommId { get; set; }
        public ushort Len { get; set; }
        public byte[] Data { get; set; }
    }
}

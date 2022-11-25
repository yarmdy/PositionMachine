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
    public class LabelMachineHelperOld32 : LabelMachineHelperBase
    {
        public LabelMachineHelperOld32(string portName, int baudRate = 115200, int dataBits = 8, int readTimeout = -1, Parity parity = Parity.None, StopBits stopBits = StopBits.One) : base(portName, baudRate, dataBits, readTimeout, parity, stopBits)
        {
        }

        public override event Action<DataPackage> OnBackLabel;
        public override event Action<DataPackage> OnBackLabelList;
        public override event Action<DataPackage> OnBackCancelList;
        public override event Action<DataPackage> OnBackSpecialLabel;
        public override event Action<DataPackage> OnBackBackOrder;
        public override event Action<DataPackage> OnBackLightTest;
        public override event Action<DataPackage> OnMyTurn;
        public override event Action<DataPackage> OnLabelStatus;
        public override event Action<DataPackage> OnLightStatus;
        public override event Action<DataPackage> OnBackReadParams;
        public override event Action<DataPackage> OnBackSaveParams;
        public override event Action<DataPackage> OnBackApplyParams;
        public override event Action<DataPackage> OnBackVerticalMotor;
        public override event Action<DataPackage> OnBackPinchRollerMotor;
        public override event Action<DataPackage> OnBackHorizontalMotor;
        public override event Action<DataPackage> OnBackMainMotor;
        public override event Action<DataPackage> OnBackManipulator;
        public override event Action<DataPackage> OnBackCard;
        public override event Action<DataPackage> OnBackExitMotor;
        public override event Action<DataPackage> OnBackBin;
        public override event Action<DataPackage> OnUpParam;
        public override event Action<byte[]> AfterReceive;
        public override event Action<DataPackage> OnEmptyBin;
        public override event Action<DataPackage> OnBackDropTubeConfirm;
        public override event Action<DataPackage> OnBackFillBin;
        public override event Action<DataPackage> OnUpMotorSteps;
        public override event Action<DataPackage> OnBackWriteBin;
        public override event Action<DataPackage> OnEnterSystem;

        public override byte ApplyParam(ushort[] array)
        {
            throw new NotImplementedException();
        }

        public override byte CancelLabelList(byte orderId)
        {
            throw new NotImplementedException();
        }

        public override byte EnterBoot()
        {
            throw new NotImplementedException();
        }

        public override byte EnterSystem(byte sysNo)
        {
            throw new NotImplementedException();
        }

        public override byte FaultConfirm(byte act)
        {
            throw new NotImplementedException();
        }

        public override byte FillBin(byte binno)
        {
            throw new NotImplementedException();
        }

        public override byte ReadParam()
        {
            throw new NotImplementedException();
        }

        public override byte SaveParam(ushort[] array)
        {
            throw new NotImplementedException();
        }

        public override byte StartBackOrder(string code, byte[] printData)
        {
            throw new NotImplementedException();
        }

        public override byte StartLabel(byte[] binIds, string code, byte[] printData)
        {
            throw new NotImplementedException();
        }

        public override byte StartLabelList(byte orderId, byte num, byte binNum, byte[] binLbNum)
        {
            throw new NotImplementedException();
        }

        public override byte StartLightTest(EnumOpenClose openClose)
        {
            throw new NotImplementedException();
        }

        public override byte StartSpecialLabel(string code, byte[] printData)
        {
            throw new NotImplementedException();
        }

        public override byte TestBin(byte binId, byte act)
        {
            throw new NotImplementedException();
        }

        public override byte TestMachine(byte comm, byte act)
        {
            throw new NotImplementedException();
        }

        public override byte TestMotor(byte act, short val)
        {
            throw new NotImplementedException();
        }

        public override byte WriteBin(uint addr, byte[] binData)
        {
            throw new NotImplementedException();
        }
    }
}

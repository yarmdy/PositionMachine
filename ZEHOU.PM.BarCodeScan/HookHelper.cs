using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace ZEHOU.PM.BarCodeScan
{
    public class HookHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public class KeyBoardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        //委托 
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        static int hHook = 0;
        public const int WH_KEYBOARD_LL = 13;
        //LowLevel键盘截获，如果是WH_KEYBOARD＝2，并不能对系统键盘截取，Acrobat Reader会在你截取之前获得键盘。 
        static HookProc KeyBoardHookProcedure;

        public static Action<string> OnScan;

        //设置钩子 
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //抽掉钩子 
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll")]
        //调用下一个钩子 
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        public static void Hook_Start()
        {
            if (hHook != 0)
            {
                return;
            }
            KeyBoardHookProcedure = new HookProc(KeyBoardHookProc);
            hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyBoardHookProcedure,
                    GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            //如果设置钩子失败. 
            if (hHook == 0)
            {
                Hook_Clear();
            }
        }

        /// <summary>
        /// 取消钩子事件
        /// </summary>
        public static void Hook_Clear()
        {
            bool retKeyboard = true;
            if (hHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hHook);
                hHook = 0;
            }
        }

        static StringBuilder _barcode = new StringBuilder();
        static DateTime lastTime = DateTime.Now;
        public static double KeyPressTimeout { get; set; } = 50;
        public static int KeyBoardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode < 0 || wParam!=0x0100)
            {
                goto result;
            }

            KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));
            Keys k = (Keys)Enum.Parse(typeof(Keys), kbh.vkCode.ToString());

            if (!(kbh.vkCode >= 0x41 && kbh.vkCode <= 0x5A || kbh.vkCode >= 0x30 && kbh.vkCode <= 0x39 || kbh.vkCode==0x0D)) {
                goto result;
            }

            var now = DateTime.Now;
            
            if((now- lastTime).TotalMilliseconds > KeyPressTimeout)
            {
                _barcode.Clear();
            }
            lastTime = now;
            
            
            if (kbh.vkCode != 13)
            {
                _barcode.Append((char)((byte)kbh.vkCode));
                goto result;
            }

            if (_barcode.Length <= 0)
            {
                goto result;
            }

            var barCode = _barcode.ToString();
            _barcode.Clear();

            if (OnScan==null) { 
                goto result;
            }

            OnScan(barCode);

        result:
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZEHOU.PM.Command
{
    public class StringExtend
    {
        public static byte[] Str2Bytes(string str)
        {
            int index = 0;
            byte[] res = null;
            try {
                res = str.Select(a => new { i = index++, c = a }).GroupBy(a => a.i / 2).Select(a => byte.Parse(string.Join("", a.Select(b => b.c)), System.Globalization.NumberStyles.HexNumber)).ToArray();
            } catch { }
            return res;
        }
    }
}

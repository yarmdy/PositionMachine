using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZEHOU.PM.Command
{
    public static class ClassExtend
    {
        public static T2 G<T1,T2>(this Dictionary<T1, T2> dic, T1 key) { 
            return dic.ContainsKey(key) ? dic[key] : default(T2);
        }

        public static T G<T>(this T[] array, int index)
        {
            if (index >= array.Length || index < 0) { 
                return default(T);
            }
            return array[index];
        }
    }
}

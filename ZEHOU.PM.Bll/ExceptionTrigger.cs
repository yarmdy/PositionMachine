using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZEHOU.PM.Bll
{
    public static class ExceptionTrigger
    {
        public static event Action<string,Exception> OnException;

        public static void ProssException(string msg, Exception ex) {
            if (OnException == null) return;
            OnException(msg, ex);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
            Bll.ExceptionTrigger.OnException += ExceptionTrigger_OnException;
            base.OnStartup(e);
        }

        private void ExceptionTrigger_OnException(string arg1, Exception arg2)
        {
            UILog.Error(arg1,arg2);
        }
    }
}

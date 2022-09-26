using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using log4net;
using ZEHOU.PM.Printer;

namespace ZEHOU.PM.Label
{
    public static class Global
    {
        public static ZEHOU.PM.DB.dbLabelInfo.User LocalUser { get; set; }
        public static List<ZEHOU.PM.DB.dbLabelInfo.Role> LocalRoles { get; set; }

        public static string LocalRolesName
        {
            get
            {
                return string.Join(",", LocalRoles.Select(a => a.Name));
            }
        }

        public static List<string> LocalFuncs
        {
            get
            {
                return LocalRoles.SelectMany(a => a.FunctionID.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Distinct().ToList();
            }
        }

        public static ZEHOU.PM.DB.dbLabelInfo.Department LocalDepartment { get; set; }
        public static ZEHOU.PM.DB.dbLabelInfo.Post LocalPost { get; set; }

        public static MainWindow MainWindow { get; set; }
        public static AutoLabel AutoLabel { get; set; }

        public static TabControl TabControl { get; set; }

        public static TabItem GetTabItemByName(string name)
        {
            if (TabControl == null) return null;
            foreach (TabItem item in TabControl.Items)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
            //if (TabControl == null) return null;
            //foreach (object itemo in TabControl.Items)
            //{
            //    if (itemo.GetType() != typeof(TabModel)) continue;
            //    var item=itemo as TabModel;
            //    if (item.Name == name)
            //    {
            //        return item;
            //    }
            //}
            //return null;
        }

        public static void AddTab(string itemName,string header) {
            if (TabControl == null) return;
            
            var tabItem = Global.GetTabItemByName(itemName);
            if (tabItem == null)
            {
                var page = (Page)Activator.CreateInstance(Type.GetType("ZEHOU.PM.Label." + itemName, false, true));
                if (page == null)
                {
                    return;
                }
                var headObj = new DockPanel();
                var closeBtn = new Button();
                closeBtn.Name = itemName;
                closeBtn.Content = "×";
                closeBtn.Style = (System.Windows.Style)closeBtn.FindResource("tabclose");
                closeBtn.Click += CloseBtn_Click;
                DockPanel.SetDock(closeBtn, Dock.Right);
                var txt=new TextBlock();
                txt.Text = header;
                txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                headObj.Children.Add(closeBtn);
                headObj.Children.Add(txt);
                tabItem = new TabItem();
                tabItem.Name = itemName;
                var frame = new Frame();
                frame.Content = page;
                tabItem.Header = headObj;
                tabItem.Content = frame;
                TabControl.Items.Add(tabItem);
                //tabItem = new TabModel { Header = header, Source = $"/{itemName}.xaml", Name = itemName };
                //TabControl.Items.Add(tabItem);
            }
            TabControl.SelectedItem = tabItem;
        }

        private static void CloseBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            removeTab(((Control)sender).Name);
        }

        public static void removeTab(string name)
        {
            if (TabControl == null) return;
            var item = GetTabItemByName(name);
            if(item==null) return;

            if (((Frame)(item.Content)).Content is IPageClose) {
                ((IPageClose)((Frame)(item.Content)).Content).PageClose();
            }

            TabControl.Items.Remove(item);
        }

        public static SerialPort.LabelMachineHelper LPM { get; set; }

        public static LabelController LabelController { get; set; }

        public static ILog SerialInfoLog = LogManager.GetLogger("SerialInfoLog");
        public static ILog SerialSendLog = LogManager.GetLogger("SerialInfoSend");
        public static ILog SerialReceiveLog = LogManager.GetLogger("SerialInfoReceive");

        public static ILog SerialBaseReceiveLog = LogManager.GetLogger("SerialInfoBaseReceive");

        public static ILog Log = LogManager.GetLogger("Info");

        public static LabelBindingNotify BindingInfo
        {
            get;set;
        }

        public static Dictionary<string,int> TubeColorDic { get;set; }

        public static SynchronizationContext SynchronizationContext { get; set; }

        public static string TubeColorConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TubeColor.config");

        public static bool IsMachineNonStandard
        {
            get
            {
                return Config.Configs.Settings["PortName"] == Config.Configs.Settings["NonStandard"];
            }
        }
        public static bool IsMachineBackOrder
        {
            get
            {
                return Config.Configs.Settings["PortName"] == Config.Configs.Settings["BackOrder"];
            }
        }
        public static bool IsSamePrinter {
            get
            {
                return Config.Configs.Settings["NonStandard"] == Config.Configs.Settings["BackOrder"];
            }
        }

        public static LabelPrinter NonStandardPrinter { get; set; }
        public static LabelPrinter BackOrderPrinter { get; set; }

        public static PassiveClass PassiveClass { get; set; }
    }
}

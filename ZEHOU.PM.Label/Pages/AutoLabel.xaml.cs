﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZEHOU.PM.Label.UI;

namespace ZEHOU.PM.Label
{
    /// <summary>
    /// AutoLabel.xaml 的交互逻辑 自动贴标
    /// </summary>
    public partial class AutoLabel : Page,IPageClose
    {
        string tabName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        public AutoLabel()
        {
            InitializeComponent();
            Global.AutoLabel = this;
            DataContext = Global.BindingInfo;
            BarCodeScan.HookHelper.IsOpened = true;
        }

        private void tbClose_Click(object sender, RoutedEventArgs e)
        {
            Global.removeTab(tabName);
        }

        public void layerMsg(string msg, string title = "操作提示") { 
            tbLayerTitle.Text = title;
            tbLayerContent.Text = msg;
            gdLayer.Visibility = Visibility.Visible;
        }
        public void layerClose()
        {
            gdLayer.Visibility = Visibility.Collapsed;
        }

        private void tbPause_Click(object sender, RoutedEventArgs e)
        {
            Global.BindingInfo.SysInfo.SysStatus = Global.BindingInfo.SysInfo.SysStatus < 0 ? 0 : -1;
            
        }

        private void tbPrintBack2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tbInputCode_Click(object sender, RoutedEventArgs e)
        {
            var inputInfo = new InputCodeClass();
            var winInput = new InputCode() { InputCodeClass= inputInfo };
            BarCodeScan.HookHelper.IsOpened = false;
            winInput.ShowDialog();
            txtId.Focus();
            BarCodeScan.HookHelper.IsOpened = true;
            if (string.IsNullOrEmpty(inputInfo.Code)) {
                return;
            }

            UILog.Info("手动输入编号：" + inputInfo.Code);
            Global.LabelController.GetABarCode(inputInfo.Code);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Global.LabelController.AddToQueue();
        }

        public void PageClose()
        {
            Global.AutoLabel.DataContext = null;
            BarCodeScan.HookHelper.IsOpened = false;
        }


        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Handled) return;
            e.Handled = true;
            var mouseEvent = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            mouseEvent.RoutedEvent = PreviewMouseWheelEvent;
            mouseEvent.Source = lvMyTubes;
            lvMyTubes.RaiseEvent(mouseEvent);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Global.LabelController.PrintBackOrder();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2)
            {
                return;
            }
            var data =  (LabelInfoNotify)((Image)sender).DataContext;
            //data.TubeLabelStatus = -0xa1;
            //return;
            var dataList = new System.Collections.ObjectModel.ObservableCollection<LabelInfoNotify> { };
            dataList.Add(data);
            data.IsChecked = true;
            Global.LabelController.AddToQueue(dataList);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;

            if (!(btn.DataContext is BinStatusInfo))
            {
                return;
            }
            var obj = (BinStatusInfo)btn.DataContext;
            obj.CommId = Global.LPM.FillBin(obj.BinId);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ElementHelper.GetChildren<ScrollViewer>(lvMyTubes).ForEach(a => ElementHelper.UseTheScrollViewerScrolling(a));
        }
    }
}

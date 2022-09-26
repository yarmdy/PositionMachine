using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZEHOU.PM.Label.UI
{
    public class Popup
    {
        public static MessageBoxResult Error(Window owner,string msg,string title="操作提示") {
            return MessageBox.Show(owner,msg,title,MessageBoxButton.OK,MessageBoxImage.Error);
        }
        public static MessageBoxResult Succ(Window owner, string msg, string title = "操作提示")
        {
            return MessageBox.Show(owner, msg, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public static MessageBoxResult Confirm(Window owner, string msg, string title = "操作提示")
        {
            return MessageBox.Show(owner, msg, title, MessageBoxButton.OKCancel, MessageBoxImage.Question,MessageBoxResult.OK);
        }
        public static MessageBoxResult Choose(Window owner, string msg, string title = "操作提示")
        {
            return MessageBox.Show(owner, msg, title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
        }
        public static MessageBoxResult ChooseC(Window owner, string msg, string title = "操作提示")
        {
            return MessageBox.Show(owner, msg, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
        }
    }
}

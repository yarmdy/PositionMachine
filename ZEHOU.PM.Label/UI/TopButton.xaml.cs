using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ZEHOU.PM.Label.UI
{
    /// <summary>
    /// TopButton.xaml 的交互逻辑
    /// </summary>
    [System.ComponentModel.DefaultEvent("Click")]
    public partial class TopButton : UserControl
    {
        public static readonly DependencyProperty IcoProperty =
            DependencyProperty.Register("Ico", typeof(ImageSource), typeof(TopButton), new PropertyMetadata(null,new PropertyChangedCallback(IcoChange)));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TopButton), new PropertyMetadata(null, new PropertyChangedCallback(TextChange)));

        public static void IcoChange(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            TopButton topButton = (TopButton)d;
            if (topButton == null)
            {
                return;
            }
            topButton.imgIco.Source = (ImageSource)e.NewValue;
        }

        public static void TextChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TopButton topButton = (TopButton)d;
            if (topButton == null)
            {
                return;
            }
            topButton.lbText.Content = e.NewValue;
        }

        private static Brush enterColor = new SolidColorBrush(Color.FromArgb(0xff,0xdd,0xdd,0xff));
        private static Brush downColor = new SolidColorBrush(Color.FromArgb(0xff,0xaa,0xaa,0xff));

        public event RoutedEventHandler Click = null;

        [Bindable(true)]
        [Category("Appearance")]
        public ImageSource Ico
        {
            get
            {
                return (ImageSource)GetValue(IcoProperty);
            }
            set
            {
                SetValue(IcoProperty, value);
            }
        }
        [Bindable(true)]
        [Category("Appearance")]
        public object Text
        {
            get
            {
                return GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }
        public TopButton()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Background = enterColor;
            BorderBrush = Brushes.Black;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = Brushes.Transparent;
            BorderBrush = Brushes.Transparent;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Click == null) return;
            Click(this,e);
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Background = downColor;
        }

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Background = Brushes.Transparent;
        }
    }
}

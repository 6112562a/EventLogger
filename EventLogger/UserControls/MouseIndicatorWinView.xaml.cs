using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EventLogger.Annotations;

namespace EventLogger
{
    /// <summary>
    /// MouseIndicatorWinView.xaml 的交互逻辑
    /// </summary>
    public partial class MouseIndicatorWinView : Window,INotifyPropertyChanged
    {
        public MouseIndicatorWinView()
        {
            InitializeComponent();
            MouseVisibility=Visibility.Visible;
            MouseColor = new SolidColorBrush(Color.FromRgb(255,0,0));
        }

        private Visibility _mouseVisibility;
        /// <summary>
        /// 鼠标暗示器的显隐
        /// </summary>
        public Visibility MouseVisibility
        {
            get { return _mouseVisibility; }
            set
            {
                _mouseVisibility = value;
                OnPropertyChanged();
            }
        }

        private Brush _mouseColor;
        /// <summary>
        /// 鼠标暗示器颜色
        /// </summary>
        public Brush MouseColor
        {
            get { return _mouseColor; }
            set
            {
                _mouseColor = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EventLogger.Annotations;

namespace EventLogger.UserControls
{
    /// <summary>
    /// SetDefaultTimeSpanControl.xaml 的交互逻辑
    /// </summary>
    public partial class SetDefaultTimeSpanControl : Window,INotifyPropertyChanged
    {
        private int _hours;
        private int _minutes;
        private int _seconds;

        #region 继承

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        public SetDefaultTimeSpanControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 时
        /// （定时时间）
        /// </summary>
        public int Hours
        {
            get { return _hours; }
            set
            {
                if (value == _hours) return;
                _hours = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 分
        /// （定时时间）
        /// </summary>
        public int Minutes
        {
            get { return _minutes; }
            set
            {
                if (value == _minutes) return;
                _minutes = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 秒
        /// （定时时间）
        /// </summary>
        public int Seconds
        {
            get { return _seconds; }
            set
            {
                if (value == _seconds) return;
                _seconds = value;
                OnPropertyChanged();
            }
        }

        private void OKBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}

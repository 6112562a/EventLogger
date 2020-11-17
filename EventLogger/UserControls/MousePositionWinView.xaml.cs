using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EventLogger.Annotations;
using MouseKeyboardLibrary;
using WPFComponent.Common.SystemConfig;

namespace EventLogger
{
    /// <summary>
    /// MousePositionWinView.xaml 的交互逻辑
    /// </summary>
    public partial class MousePositionWinView : Window,INotifyPropertyChanged
    {
        private double _xPosition;
        private double _yPositoin;

        #region Implements
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public MousePositionWinView()
        {
            InitializeComponent();
        }

        public double XPosition
        {
            get { return _xPosition; }
            set
            {
                if (value.Equals(_xPosition)) return;
                _xPosition = value;
                OnPropertyChanged();
            }
        }

        public double YPositoin
        {
            get { return _yPositoin; }
            set
            {
                if (value.Equals(_yPositoin)) return;
                _yPositoin = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<HookMouseEventArgs> _mouseHisDataList;
        /// <summary>
        /// 鼠标历史记录
        /// </summary>
        public ObservableCollection<HookMouseEventArgs> MouseHisDataList
        {
            get { return _mouseHisDataList ?? (_mouseHisDataList = new ObservableCollection<HookMouseEventArgs>()); }
            set
            {
                _mouseHisDataList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 鼠标钩子
        /// </summary>
        MouseHook mouseHook = new MouseHook();
        /// <summary>
        /// 记录鼠标
        /// </summary>
        /// <param name="e"></param>
        private void MouseHookMouseLog(HookMouseEventArgs e)
        {
            XPosition = e.XPosition;
            YPositoin = e.YPosition;
            if (e.ClickButton == MouseButton.Left && e.MouseEventType == MouseEventType.MouseDown)
            {
                MouseHisDataList.Add(e);
            }
        }
        private void MousePositionWinView_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var item = e.Source as FrameworkElement;
            var mousePoint = Mouse.GetPosition(item);
            XPosition = mousePoint.X;
            YPositoin = mousePoint.Y;
        }
        /// <summary>
        /// 鼠标热键触发
        /// </summary>
        private void mouseHotkey_OnHotKeyHit()
        {
            MouseHookToggleButton.IsChecked = !MouseHookToggleButton.IsChecked;
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MousePositionWinView_OnLoaded(object sender, RoutedEventArgs e)
        {
            //---注册热键---
            var handle = new WindowInteropHelper(this).Handle;
            var keyFlag_mouse = HotKey.KeyFlags.MOD_ALT;
            var key_mouse = Key.A;
            var mouseHotkey = new HotKey(this, handle, keyFlag_mouse | HotKey.KeyFlags.MOD_NOREPEAT, key_mouse);
            if (mouseHotkey.IsRegisterOK)
            {
                mouseHotkey.OnHotKeyHit += mouseHotkey_OnHotKeyHit;
            }
            else
            {
                MessageBox.Show(this, string.Format("注册热键{0}+{1}失败!",keyFlag_mouse,key_mouse), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MousePositionWinView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            mouseHook.Stop();
        }
        /// <summary>
        /// 开启MouseHook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseHookToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            //尝试捕捉记录
            mouseHook.MouseLog += MouseHookMouseLog;
            mouseHook.MonitorMouseMove = true;
            mouseHook.Start();
        }
        /// <summary>
        /// 关闭MouseHook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseHookToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            mouseHook.MouseLog -= MouseHookMouseLog;
            mouseHook.Stop();
        }
    }
}

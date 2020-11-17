using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EventLogger.Annotations;
using EventLogger.Hook;
using EventLogger.Log;
using EventLogger.UserControls;
using Microsoft.Win32;
using MouseKeyboardLibrary;
using WPFComponent.Common.SystemConfig;
using WPFComponent.Common.Threads;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace EventLogger
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Implements

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string[] strArgs)
        {
            InitializeComponent();
            //-----参数的处理-----
            var compareKey = string.Empty;
            foreach (var strArg in strArgs)
            {
                compareKey = "-Mouse=";
                if (strArg.StartsWith(compareKey))
                {
                    //指定Mouse历史文件路径
                    var fileName = strArg.Remove(0, compareKey.Length);
                    var mouseHis = CustomEventLogger.GetHookMouseEventArgses(fileName);
                    MouseHisDataList = new ObservableCollection<HookMouseEventArgs>(mouseHis);
                }
                else
                {
                    compareKey = "-Key=";
                    if (strArg.StartsWith(compareKey))
                    {
                        //指定Key历史文件路径
                        var fileName = strArg.Remove(0, compareKey.Length);
                        var keyHis = CustomEventLogger.GetHookKeyEventArgses(fileName);
                        KeyHisDataList = new ObservableCollection<HookKeyEventArgs>(keyHis);
                    }
                    else
                    {
                        //设定关键词
                        var keyValueIndex = strArg.IndexOf('=');//获取关键词分隔符
                        var key = strArg.Substring(1, keyValueIndex-1);//取得关键词
                        var value = strArg.Substring(keyValueIndex+1);//取得替换值
                        foreach (var hookKeyEventArgse in KeyHisDataList)
                        {
                            if (hookKeyEventArgse.InputString == key)
                            {
                                hookKeyEventArgse.InputString = value;
                            }
                        }
                    }
                }
            }
            //-----直接执行-----
            argAction = () =>
            {
                ManagedTask.FactoryStart(() =>
                {
                    SimulateAll();
                    UIThread.Invoke(Close);//执行完关闭
                });
            };
        }

        #region Fields

        private string _args;

        private Action argAction;

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

        private ObservableCollection<HookKeyEventArgs> _keyHisDataList;
        /// <summary>
        /// 键盘历史记录
        /// </summary>
        public ObservableCollection<HookKeyEventArgs> KeyHisDataList
        {
            get { return _keyHisDataList ?? (_keyHisDataList = new ObservableCollection<HookKeyEventArgs>()); }
            set
            {
                _keyHisDataList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 鼠标钩子
        /// </summary>
        MouseHook mouseHook = new MouseHook();
        /// <summary>
        /// 键盘钩子
        /// </summary>
        KeyboardHook keyboardHook = new KeyboardHook();

        private bool _isRepeat;
        private int _repeatTimes;
        private string _statusTip;

        /// <summary>
        /// 是否重复执行
        /// </summary>
        public bool IsRepeat
        {
            get { return _isRepeat; }
            set
            {
                if (value == _isRepeat) return;
                _isRepeat = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 重复执行次数
        /// </summary>
        public int RepeatTimes
        {
            get { return _repeatTimes; }
            set
            {
                if (value == _repeatTimes) return;
                _repeatTimes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 状态栏提示文本
        /// </summary>
        public string StatusTip
        {
            get { return _statusTip; }
            set
            {
                if (value == _statusTip) return;
                _statusTip = value;
                OnPropertyChanged();
            }
        }

        #endregion
        /// <summary>
        /// 窗体初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //---注册热键---
            //-鼠标-
            var handle = new WindowInteropHelper(this).Handle;
            var mouseHotkey = new HotKey(this, handle, HotKey.KeyFlags.MOD_ALT | HotKey.KeyFlags.MOD_NOREPEAT, Key.Q);
            if (mouseHotkey.IsRegisterOK)
            {
                mouseHotkey.OnHotKeyHit += mouseHotkey_OnHotKeyHit;
            }
            else
            {
                MessageBox.Show(this, "注册热键ALT+Q失败!", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //-键盘-
            var keyboardHotKey = new HotKey(this, handle, HotKey.KeyFlags.MOD_ALT | HotKey.KeyFlags.MOD_NOREPEAT, Key.W);
            if (keyboardHotKey.IsRegisterOK)
            {
                keyboardHotKey.OnHotKeyHit += keyBoardHotkey_OnHotKeyHit;
            }
            else
            {
                MessageBox.Show(this, "注册热键ALT+W失败!", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //-重复-
            var keyFlag_repeat = HotKey.KeyFlags.MOD_ALT;
            var key_repeat = Key.R;
            var repeatHotkey = new HotKey(this, handle, keyFlag_repeat | HotKey.KeyFlags.MOD_NOREPEAT, key_repeat);
            if (repeatHotkey.IsRegisterOK)
            {
                repeatHotkey.OnHotKeyHit += repeatHotkey_OnHotKeyHit;
            }
            else
            {
                MessageBox.Show(this, string.Format("注册热键{0}+{1}失败!", keyFlag_repeat, key_repeat), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //------设定初始值------
            RepeatTimes = 3;
            StatusTip = "就绪!";
            //----------执行ArgAction-------------
            if (argAction != null)
            {
                argAction();
                argAction = null;
            }
        }

        #region 桌面绘图


        /// <summary>
        /// 在桌面绘制圆圈
        /// </summary>
        /// <param name="mousePoint"></param>
        private void DrawMousePoint(Point mousePoint)
        {

            IntPtr desk = DeskTopDrawing.GetDesktopWindow();

            IntPtr deskDC = DeskTopDrawing.GetDCEx(desk, IntPtr.Zero, 0x403);

            System.Drawing.Graphics g = Graphics.FromHdc(deskDC);

            g.FillEllipse(new SolidBrush(System.Drawing.Color.Red), Convert.ToInt32(mousePoint.X), Convert.ToInt32(mousePoint.Y), 50, 50);

        }

        #endregion

        #region 键鼠记录
        /// <summary>
        /// 键盘热键触发
        /// </summary>
        private void keyBoardHotkey_OnHotKeyHit()
        {
            KeyHookToggleButton.IsChecked = !KeyHookToggleButton.IsChecked;
        }
        /// <summary>
        /// 鼠标热键触发
        /// </summary>
        private void mouseHotkey_OnHotKeyHit()
        {
            MouseHookToggleButton.IsChecked = !MouseHookToggleButton.IsChecked;
        }

        /// <summary>
        /// 设定/取消 重复
        /// </summary>
        private void repeatHotkey_OnHotKeyHit()
        {
            IsRepeat = !IsRepeat;
        }
        /// <summary>
        /// 记录键盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyboardHook_KeyLog(object sender, HookKeyEventArgs e)
        {
            KeyHisDataList.Add(e);
            CustomEventLogger.LogKeyEvent(e);
        }
        /// <summary>
        /// 记录鼠标
        /// </summary>
        /// <param name="e"></param>
        private void MouseHookMouseLog(HookMouseEventArgs e)
        {
            MouseHisDataList.Add(e);
            CustomEventLogger.LogMouseEvent(e);
        }
        /// <summary>
        /// 模拟鼠标操作
        /// </summary>
        private void SimulateMouseBehavior()
        {
            KeyAndMouseSimulator.SimulateMouseBehavior(MouseHisDataList);
        }
        /// <summary>
        /// 模拟键盘操作
        /// </summary>
        private void SimulateKeyBehavior()
        {
            KeyAndMouseSimulator.SimulateKeyBehavior(KeyHisDataList);
        }
        /// <summary>
        /// 连贯模拟鼠标和键盘
        /// </summary>
        private void SimulateAll()
        {
            KeyAndMouseSimulator.SimulateAll(MouseHisDataList, KeyHisDataList);
        }
        /// <summary>
        /// 窗体销毁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            mouseHook.Stop();
            keyboardHook.Stop();
            KeyAndMouseSimulator.MouseIndicatorWinView.Close();
        }
        /// <summary>
        /// 模拟鼠标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseSimulateMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ManagedTask.FactoryStart(SimulateMouseBehavior);
        }

        /// <summary>
        /// 打开记录文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenLogMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "db日志|*.db|所有文件|*.*";
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                if (fileName.Contains("Mouse"))
                {
                    var mouseHis = CustomEventLogger.GetHookMouseEventArgses(fileName);
                    MouseHisDataList = mouseHis == null ? new ObservableCollection<HookMouseEventArgs>() : new ObservableCollection<HookMouseEventArgs>(mouseHis);
                }
                else
                {
                    var keyHis = CustomEventLogger.GetHookKeyEventArgses(fileName);
                    KeyHisDataList = keyHis == null ? new ObservableCollection<HookKeyEventArgs>() : new ObservableCollection<HookKeyEventArgs>(keyHis);
                }
            }
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveLogMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            CustomEventLogger.WriteLog();
            CustomEventLogger.SaveHisLog(MouseHisDataList.ToList());
            CustomEventLogger.SaveHisLog(KeyHisDataList.ToList());
            MessageBox.Show("保存成功！", "恭喜", MessageBoxButton.OK, MessageBoxImage.Information);
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
        /// <summary>
        /// 开启KeyHook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyHookToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            keyboardHook.KeyLog += keyboardHook_KeyLog;
            keyboardHook.Start();
        }
        /// <summary>
        /// 关闭KeyHook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyHookToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            keyboardHook.KeyLog -= keyboardHook_KeyLog;
            keyboardHook.Stop();
        }
        /// <summary>
        /// 模拟操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllSimulateMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ExcuteSimulateAll();
        }
        /// <summary>
        /// 清空历史记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearHisMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            //--清空历史记录--
            KeyHisDataList.Clear();
            //--清空历史记录--
            MouseHisDataList.Clear();
        }
        #endregion

        /// <summary>
        /// 执行模拟操作
        /// </summary>
        private void ExcuteSimulateAll()
        {
            ManagedTask.FactoryStart(() =>
            {
                UIThread.Invoke(() =>{ StatusTip = "开始执行!"; });
                if (IsRepeat)
                {
                    while (IsRepeat && RepeatTimes > 0)
                    {
                        SimulateAll();
                        UIThread.Invoke(() =>
                        {
                            RepeatTimes--;
                        });
                    }
                }
                else
                {
                    SimulateAll();
                }
                UIThread.Invoke(() => { StatusTip = "执行完毕!"; });
            });
        }

        /// <summary>
        /// 测试功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(5000);
                var handle = WindowHWND.ChildWindowFromPoint();
                Console.WriteLine(string.Format("ChildWindowFromPoint:{0}", handle));
                var encoding = Encoding.GetEncoding("GB2312");
                WindowHWND.InputString(handle, "WM_CHAR:this is sparta!", encoding);
                WindowHWND.InputString(handle, "WM_CHAR:这就是斯巴达！", encoding);
                WindowHWND.InputMessage(handle, "WM_SETTEXT:斯巴达克斯。");
            }
        }
        #region 表格右键菜单

        /// <summary>
        /// 添加鼠标记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMenuItem_Mouse_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedIndex = MouseHisDataGrid.SelectedIndex > 0 ? MouseHisDataGrid.SelectedIndex : 0;
            MouseHisDataList.Insert(selectedIndex, new HookMouseEventArgs());
        }
        /// <summary>
        /// 删除鼠标记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMenuItem_Mouse_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedItems = new List<HookMouseEventArgs>();
            foreach (var selectedItem in MouseHisDataGrid.SelectedItems)
            {
                var selectedHookFile = selectedItem as HookMouseEventArgs;
                if (selectedHookFile != null)
                {
                    selectedItems.Add(selectedHookFile);
                }
            }
            foreach (var selectedItem in selectedItems)
            {
                MouseHisDataList.Remove(selectedItem);
            }
        }
        /// <summary>
        /// 添加键盘记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMenuItem_Key_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedIndex = KeyHisDataGrid.SelectedIndex > 0 ? KeyHisDataGrid.SelectedIndex : 0;
            KeyHisDataList.Insert(selectedIndex, new HookKeyEventArgs());
        }
        /// <summary>
        /// 删除键盘记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMenuItem_Key_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedItems = new List<HookKeyEventArgs>();
            foreach (var selectedItem in KeyHisDataGrid.SelectedItems)
            {
                var selectedHookFile = selectedItem as HookKeyEventArgs;
                if (selectedHookFile != null)
                {
                    selectedItems.Add(selectedHookFile);
                }
            }
            foreach (var selectedItem in selectedItems)
            {
                KeyHisDataList.Remove(selectedItem);
            }
        }
        #endregion

        /// <summary>
        /// 组合日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CombineLogMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ReOrderCommandsWinView view=new ReOrderCommandsWinView();
            if (view.ShowDialog() != true) return;

            MouseHisDataList = new ObservableCollection<HookMouseEventArgs>(view.GetMouseCombineResult());
            KeyHisDataList = new ObservableCollection<HookKeyEventArgs>(view.GetKeyCombineResult());
        }

        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetMousePosMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            MousePositionWinView view=new MousePositionWinView();
            view.Show();
        }

        #region 定时执行
        /// <summary>
        /// 执行定时用Timer
        /// <see cref="System.Threading.Timer"/>
        /// </summary>
        private Timer _timer;
        /// <summary>
        /// 定时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetExcuteTimeSpanMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            SetDefaultTimeSpanControl view = new SetDefaultTimeSpanControl();
            if (view.ShowDialog() == true)
            {
                //---取得时间范围----
                var hours = view.Hours;
                var minutes = view.Minutes;
                var seconds = view.Seconds;
                var timespan = new TimeSpan(0, hours, minutes, seconds);
                var excuteTime = DateTime.Now + timespan;
                //指定时间间隔，不重复执行
                _timer = new Timer(OnTimerTicked, excuteTime, 0, 1000);
            }
        }
        /// <summary>
        /// 计时器执行
        /// </summary>
        /// <param name="state"></param>
        private void OnTimerTicked(object state)
        {
            var excuteTime = state as DateTime?;
            if (excuteTime != null)
            {
                var difTimeSpan = excuteTime.Value - DateTime.Now;
                var totalSeconds = difTimeSpan.TotalSeconds;
                UIThread.Invoke(() =>
                {
                    StatusTip = string.Format("执行时间：{0:u}，倒计时:{1}秒", excuteTime.Value, Convert.ToInt32(totalSeconds));
                });
                if (totalSeconds<1)
                {
                    ExcuteSimulateAll();
                    _timer.Dispose();
                }
            }
        }
        

        #endregion
       
    }
}

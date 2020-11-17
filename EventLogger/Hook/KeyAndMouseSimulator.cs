using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MouseKeyboardLibrary;
using WPFComponent.Common.SystemConfig;
using WPFComponent.Common.Threads;

namespace EventLogger.Hook
{
    public class KeyAndMouseSimulator
    {
        private static MouseIndicatorWinView _mouseIndicatorWinView;
        /// <summary>
        /// 鼠标暗示器窗体
        /// </summary>
        public static MouseIndicatorWinView MouseIndicatorWinView
        {
            get
            {
                if (_mouseIndicatorWinView == null)
                {
                    UIThread.Invoke(() =>
                    {//主线程创建此对象，防止STA线程错误
                        _mouseIndicatorWinView = new MouseIndicatorWinView();
                    });
                }
                return _mouseIndicatorWinView;
            }
            set { _mouseIndicatorWinView = value; }
        }

        /// <summary>
        /// 展示鼠标点击
        /// </summary>
        /// <param name="view"></param>
        /// <param name="mousePoint"></param>
        public static void ShowMouseIndicator(MouseIndicatorWinView view, Point mousePoint, System.Windows.Media.Brush mouseColorBrush)
        {
            view.Show();
            view.MouseColor = mouseColorBrush;
            view.Topmost = true;
            view.Left = mousePoint.X - 20;
            view.Top = mousePoint.Y - 20;
            view.MouseVisibility = Visibility.Visible;
        }

        /// <summary>
        /// 隐藏鼠标点击暗示
        /// </summary>
        /// <param name="view"></param>
        public static void HideMouseIndicator(MouseIndicatorWinView view)
        {
            view.MouseVisibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 模拟鼠标操作
        /// </summary>
        public static void SimulateMouseBehavior(IList<HookMouseEventArgs> MouseHisDataList)
        {
            if (MouseHisDataList.Count == 0) return;
            //----遍历历史记录，重新执行一遍-----
            var lastHis = MouseHisDataList.First();
            var hisDataList = MouseHisDataList.ToList();
            foreach (var hookMouseEventArgse in hisDataList)
            {
                var timeDiff = hookMouseEventArgse.EventTimeStamp - lastHis.EventTimeStamp;
                Thread.Sleep(timeDiff);//休眠
                MouseSimulator.Position = hookMouseEventArgse.MousePosition;
                switch (hookMouseEventArgse.MouseEventType)
                {
                    case MouseEventType.MouseDown:
                        MouseSimulator.MouseDown(hookMouseEventArgse.ClickButton);
                        UIThread.Invoke(() =>
                        {
                            ShowMouseIndicator(MouseIndicatorWinView, hookMouseEventArgse.MousePosition, new SolidColorBrush(Color.FromRgb(255, 0, 0)));
                        });
                        break;
                    case MouseEventType.MouseUp:
                        MouseSimulator.MouseUp(hookMouseEventArgse.ClickButton);
                        UIThread.Invoke(() =>
                        {
                            ShowMouseIndicator(MouseIndicatorWinView, hookMouseEventArgse.MousePosition, new SolidColorBrush(Color.FromRgb(26, 58, 246)));
                        });
                        break;
                    case MouseEventType.MouseWheel:
                        MouseSimulator.MouseWheel(hookMouseEventArgse.MouseWheelDelta);
                        break;
                    case MouseEventType.MouseMove:
                        MouseSimulator.MouseMove(hookMouseEventArgse.MousePosition.X, hookMouseEventArgse.MousePosition.Y, true);
                        HideMouseIndicator(MouseIndicatorWinView);
                        break;

                }
                lastHis = hookMouseEventArgse;
            }
        }
        /// <summary>
        /// 模拟键盘操作
        /// </summary>
        public static void SimulateKeyBehavior(IList<HookKeyEventArgs> KeyHisDataList)
        {
            //----遍历历史记录，重新执行一遍-----
            var lastHis = KeyHisDataList.First();
            var hisDataList = KeyHisDataList.ToList();
            foreach (var hookKeyEventArgs in hisDataList)
            {
                var timeDiff = hookKeyEventArgs.EventTimeStamp - lastHis.EventTimeStamp;
                Thread.Sleep(timeDiff);//休眠
                switch (hookKeyEventArgs.InputKeyType)
                {
                    case KeyType.KeyDown:
                        KeyboardSimulator.KeyDown(hookKeyEventArgs.Key);
                        break;
                    case KeyType.KeyUp:
                        KeyboardSimulator.KeyUp(hookKeyEventArgs.Key);
                        break;
                    case KeyType.None:
                        //处理字符串连贯输入
                        var handle = WindowHWND.ChildWindowFromPoint();
                        var encoding = Encoding.GetEncoding("GB2312");
                        WindowHWND.InputString(handle, hookKeyEventArgs.InputString, encoding);
                        break;
                }
                lastHis = hookKeyEventArgs;
            }
        }
        /// <summary>
        /// 连贯模拟鼠标和键盘
        /// </summary>
        public static void SimulateAll(IList<HookMouseEventArgs> MouseHisDataList, IList<HookKeyEventArgs> KeyHisDataList)
        {
            Dictionary<object, DateTime> allEvents = new Dictionary<object, DateTime>();
            foreach (var hookMouseEventArgse in MouseHisDataList)
            {
                allEvents.Add(hookMouseEventArgse, hookMouseEventArgse.EventTimeStamp);
            }
            foreach (var hookKeyEventArgse in KeyHisDataList)
            {
                allEvents.Add(hookKeyEventArgse, hookKeyEventArgse.EventTimeStamp);
            }
            var orderedEventsList = allEvents.OrderBy(x => x.Value).ToList();
            if (orderedEventsList.Count == 0) return;
            var lastOpTime = orderedEventsList.First().Value;//上一次操作时间
            foreach (var keyValuePair in orderedEventsList)
            {
                var timeDiff = keyValuePair.Value - lastOpTime;
                Thread.Sleep(timeDiff);//休眠
                if (keyValuePair.Key is HookMouseEventArgs)
                {
                    var hookMouseEventArgse = keyValuePair.Key as HookMouseEventArgs;
                    MouseSimulator.Position = hookMouseEventArgse.MousePosition;
                    switch (hookMouseEventArgse.MouseEventType)
                    {
                        case MouseEventType.MouseDown:
                            MouseSimulator.MouseDown(hookMouseEventArgse.ClickButton);
                            UIThread.Invoke(() =>
                            {
                                ShowMouseIndicator(MouseIndicatorWinView, hookMouseEventArgse.MousePosition, new SolidColorBrush(Color.FromRgb(255, 0, 0)));
                            });
                            break;
                        case MouseEventType.MouseUp:
                            MouseSimulator.MouseUp(hookMouseEventArgse.ClickButton);
                            UIThread.Invoke(() =>
                            {
                                ShowMouseIndicator(MouseIndicatorWinView, hookMouseEventArgse.MousePosition, new SolidColorBrush(Color.FromRgb(26, 58, 246)));
                            });
                            break;
                        case MouseEventType.MouseWheel:
                            MouseSimulator.MouseWheel(hookMouseEventArgse.MouseWheelDelta);
                            break;
                        case MouseEventType.MouseMove:
                            MouseSimulator.MouseMove(hookMouseEventArgse.MousePosition.X, hookMouseEventArgse.MousePosition.Y, true);
                            break;

                    }
                }
                else if (keyValuePair.Key is HookKeyEventArgs)
                {
                    var hookKeyEventArgs = keyValuePair.Key as HookKeyEventArgs;
                    switch (hookKeyEventArgs.InputKeyType)
                    {
                        case KeyType.KeyDown:
                            KeyboardSimulator.KeyDown(hookKeyEventArgs.Key);
                            break;
                        case KeyType.KeyUp:
                            KeyboardSimulator.KeyUp(hookKeyEventArgs.Key);
                            break;
                        case KeyType.None:
                            //处理字符串连贯输入
                            var handle = WindowHWND.ChildWindowFromPoint();
                            var encoding = Encoding.GetEncoding("GB2312");
                            WindowHWND.InputString(handle, hookKeyEventArgs.InputString, encoding);
                            Debug.WriteLine(string.Format("Handle:{0} Encoding:{1} Content:{2}",handle,encoding.BodyName,hookKeyEventArgs.InputString));
                            break;
                    }
                }
                lastOpTime = keyValuePair.Value;
            }
        }
    }
}

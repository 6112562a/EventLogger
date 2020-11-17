using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using EventLogger.Annotations;

namespace MouseKeyboardLibrary
{

    #region MouseEventType Enum

    public enum MouseEventType
    {
        None,
        MouseDown,
        MouseUp,
        DoubleClick,
        MouseWheel,
        MouseMove
    }

    #endregion
   
    /// <summary>
    /// 处理<see cref="HookMouseEventArgs"/>事件
    /// </summary>
    /// <param name="e"></param>
    public delegate void HookMouseEventHandler(HookMouseEventArgs e);

    /// <summary>
    /// Captures global mouse events
    /// </summary>
    public class MouseHook : GlobalHook
    {


        #region Fields

        public bool MonitorMouseMove { get; set; }


        #endregion


        #region Events

        public event HookMouseEventHandler MouseLog;

        #endregion

        #region Constructor

        public MouseHook()
        {

            _hookType = WH_MOUSE_LL;

        }

        #endregion

        #region Methods

        protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {

            if (nCode > -1 && (MouseLog != null))
            {

                MouseLLHookStruct mouseHookStruct =
                    (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));
                int clickCount=1;
                MouseButton button = GetButton(wParam, out clickCount);
                MouseEventType eventType = GetEventType(wParam);
                var deltaData = (eventType == MouseEventType.MouseWheel
                    ? (short) ((mouseHookStruct.mouseData >> 16) & 0xffff)
                    : 0);

                // Prevent multiple Right Click events (this probably happens for popup menus)
                if (button == MouseButton.Right && mouseHookStruct.flags != 0)
                {
                    eventType = MouseEventType.None;
                }
                var mouseEventArgs = new HookMouseEventArgs
                {
                    ClickButton = button,
                    MouseEventType = eventType,
                    ClickCount = clickCount,
                    MousePosition = new Point(mouseHookStruct.pt.x, mouseHookStruct.pt.y),
                    MouseWheelDelta = deltaData,
                    EventTimeStamp = DateTime.Now
                };
                if (MonitorMouseMove||eventType != MouseEventType.MouseMove)
                {
                    MouseLog(mouseEventArgs);
                }

            }

            return CallNextHookEx(_handleToHook, nCode, wParam, lParam);

        }

        private MouseButton GetButton(Int32 wParam,out int clickCount)
        {

            switch (wParam)
            {

                case WM_LBUTTONDOWN:
                case WM_LBUTTONUP:
                    clickCount = 1;
                    return MouseButton.Left;
                case WM_LBUTTONDBLCLK:
                    clickCount = 2;
                    return MouseButton.Left;
                case WM_RBUTTONDOWN:
                case WM_RBUTTONUP:
                    clickCount = 1;
                    return MouseButton.Right;
                case WM_RBUTTONDBLCLK:
                    clickCount = 2;
                    return MouseButton.Right;
                case WM_MBUTTONDOWN:
                case WM_MBUTTONUP:
                    clickCount = 1;
                    return MouseButton.Middle;
                case WM_MBUTTONDBLCLK:
                    clickCount = 2;
                    return MouseButton.Middle;
                default:
                    clickCount = 1;
                    return MouseButton.XButton1;

            }

        }

        private MouseEventType GetEventType(Int32 wParam)
        {

            switch (wParam)
            {

                case WM_LBUTTONDOWN:
                case WM_RBUTTONDOWN:
                case WM_MBUTTONDOWN:
                    return MouseEventType.MouseDown;
                case WM_LBUTTONUP:
                case WM_RBUTTONUP:
                case WM_MBUTTONUP:
                    return MouseEventType.MouseUp;
                case WM_LBUTTONDBLCLK:
                case WM_RBUTTONDBLCLK:
                case WM_MBUTTONDBLCLK:
                    return MouseEventType.DoubleClick;
                case WM_MOUSEWHEEL:
                    return MouseEventType.MouseWheel;
                case WM_MOUSEMOVE:
                    return MouseEventType.MouseMove;
                default:
                    return MouseEventType.None;

            }
        }

        #endregion

    }

}

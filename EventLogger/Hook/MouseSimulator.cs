using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace MouseKeyboardLibrary
{

    /// <summary>
    /// Operations that simulate mouse events
    /// </summary>
    public static class MouseSimulator
    {

        #region Windows API Code

        [DllImport("user32.dll")]
        static extern int ShowCursor(bool show);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dX, int dY, int dwData, int extraInfo);

        const int MOUSEEVENTF_MOVE = 0x1;
        const int MOUSEEVENTF_LEFTDOWN = 0x2;
        const int MOUSEEVENTF_LEFTUP = 0x4;
        const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        const int MOUSEEVENTF_RIGHTUP = 0x10;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        const int MOUSEEVENTF_MIDDLEUP = 0x40;
        const int MOUSEEVENTF_WHEEL = 0x800;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a structure that represents both X and Y mouse coordinates
        /// </summary>
        public static Point Position
        {
            get
            {
                Point nowPoint=new Point();
                MouseHelper.GetCursorPos(out nowPoint);
                return nowPoint;
            }
            set
            {
                MouseHelper.SetCursorPos(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }

        /// <summary>
        /// Gets or sets only the mouse's x coordinate
        /// </summary>
        public static double X
        {
            get
            {
                return Position.X;
            }
            set
            {
                MouseHelper.SetCursorPos(Convert.ToInt32(value), Convert.ToInt32(Y));
            }
        }

        /// <summary>
        /// Gets or sets only the mouse's y coordinate
        /// </summary>
        public static double Y
        {
            get
            {
                return Position.Y;
            }
            set
            {
                MouseHelper.SetCursorPos(Convert.ToInt32(X), Convert.ToInt32(value));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Press a mouse button down
        /// </summary>
        /// <param name="button"></param>
        public static void MouseDown(MouseButton button)
        {
            var code = MouseHelper.GetMouseApiCode(button, MouseEventType.MouseDown);
            mouse_event(code, 0, 0, 0, 0);
        }


        /// <summary>
        /// Let a mouse button up
        /// </summary>
        /// <param name="button"></param>
        public static void MouseUp(MouseButton button)
        {
            var code = MouseHelper.GetMouseApiCode(button, MouseEventType.MouseUp);
            mouse_event(code, 0, 0, 0, 0);
        }


        /// <summary>
        /// Click a mouse button (down then up)
        /// </summary>
        /// <param name="button"></param>
        public static void Click(MouseButton button)
        {
            MouseDown(button);
            MouseUp(button);
        }

        /// <summary>
        /// Double click a mouse button (down then up twice)
        /// </summary>
        /// <param name="button"></param>
        public static void DoubleClick(MouseButton button)
        {
            Click(button);
            Click(button);
        }

        /// <summary>
        /// Show a hidden current on currently application
        /// </summary>
        public static void Show()
        {
            ShowCursor(true);
        }

        /// <summary>
        /// Hide mouse cursor only on current application's forms
        /// </summary>
        public static void Hide()
        {
            ShowCursor(false);
        }
        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="isAbsolute">MOUSEEVENTF_ABSOLUTE</param>
        public static void MouseMove(int x, int y,bool isAbsolute)
        {
            if (isAbsolute)
            {
                mouse_event(MOUSEEVENTF_MOVE+MOUSEEVENTF_ABSOLUTE, x, y, 0, 0);
            }
            else
            {
                mouse_event(MOUSEEVENTF_MOVE, x, y, 0, 0);
            }
        }
        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="isAbsolute">MOUSEEVENTF_ABSOLUTE</param>
        public static void MouseMove(double x, double y, bool isAbsolute)
        {
            var iX = Convert.ToInt32(x);
            var iY = Convert.ToInt32(y);
            MouseMove(iX,iY,isAbsolute);
        }
        /// <summary>
        /// 滚轮事件
        /// </summary>
        /// <param name="delta"></param>
        public static void MouseWheel(int delta)
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, delta, 0);
        }

        #endregion

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MouseKeyboardLibrary
{
    class MouseHelper
    {
        /// <summary>
        /// 设置鼠标的坐标
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        [DllImport("User32")]
        public static extern void SetCursorPos(int x, int y);

        /// <summary>
        /// 获取鼠标的坐标
        /// </summary>
        /// <param name="pt">传址参数，坐标point类型</param>
        /// <returns>获取成功返回真</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out Point pt);

        const int MOUSEEVENTF_MOVE = 0x1;
        const int MOUSEEVENTF_LEFTDOWN = 0x2;
        const int MOUSEEVENTF_LEFTUP = 0x4;
        const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        const int MOUSEEVENTF_RIGHTUP = 0x10;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        const int MOUSEEVENTF_MIDDLEUP = 0x40;
        const int MOUSEEVENTF_WHEEL = 0x800;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        /// <summary>
        /// 获取鼠标对应Win Api Code
        /// </summary>
        /// <param name="btn">鼠标按键类型（左右中键）</param>
        /// <param name="eventType">鼠标事件类型</param>
        /// <returns>对应Win Api Code</returns>
        public static int GetMouseApiCode(MouseButton btn, MouseEventType eventType)
        {
            if (eventType == MouseEventType.MouseDown || eventType == MouseEventType.MouseUp)
            {
                var isDown = eventType == MouseEventType.MouseDown;
                switch (btn)
                {
                    case MouseButton.Left:
                        return isDown ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_LEFTUP;
                        break;
                    case MouseButton.Middle:
                        return isDown ? MOUSEEVENTF_MIDDLEDOWN : MOUSEEVENTF_MIDDLEUP;
                        break;
                    case MouseButton.Right:
                        return isDown ? MOUSEEVENTF_RIGHTDOWN : MOUSEEVENTF_RIGHTUP;
                        break;
                }
            }
            return 0;
        }
    }
}

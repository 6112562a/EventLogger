using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using WPFComponent.Adorners;

namespace WPFComponent.Common.SystemConfig
{
    public class WindowHWND
    {
        public struct POINT { public Int32 X; public Int32 Y; }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
        }

        #region SendMessage 参数
        private const int WM_SETTEXT = 0x000C; //应用程序发送此消息来设置一个窗口的文本
        private const int WM_KEYDOWN = 0X100;
        private const int WM_KEYUP = 0X101;
        private const int WM_SYSCHAR = 0X106;
        private const int WM_SYSKEYUP = 0X105;
        private const int WM_SYSKEYDOWN = 0X104;
        private const int WM_CHAR = 0X102;
        #endregion
        /// <summary>
        /// 检索处理顶级窗口的类名和窗口名称匹配指定的字符串。
        /// 这个函数不搜索子窗口。
        /// </summary>
        /// <param name="lpClassName">类名</param>
        /// <param name="lpWindowName">窗口名</param>
        /// <returns>如果有指定的类名和窗口的名字则表示成功返回一个窗口的句柄。否则返回零。</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 在窗口列表中寻找与指定条件相符的第一个子窗口
        /// </summary>
        /// <param name="parent">要查找的子窗口所在的父窗口的句柄（如果设置了hwndParent，则表示从这个hwndParent指向的父窗口中搜索子窗口）。</param>
        /// <param name="childe">
        /// 子窗口句柄。查找从在Z序中的下一个子窗口开始。
        /// 子窗口必须为hwndParent窗口的直接子窗口而非后代窗口。
        /// 如果HwndChildAfter为NULL，查找从hwndParent的第一个子窗口开始。
        /// 如果hwndParent 和 hwndChildAfter同时为NULL，则函数查找所有的顶层窗口及消息窗口。
        /// </param>
        /// <param name="strclass">
        /// 指向一个指定了类名的空结束字符串，或一个标识类名字符串的成员的指针。
        /// 如果该参数为一个成员，则它必须为前次调用theGlobaIAddAtom函数产生的全局成员。
        /// 该成员为16位，必须位于lpClassName的低16位，高位必须为0。
        /// </param>
        /// <param name="FrmText">指向一个指定了窗口名（窗口标题）的空结束字符串。如果该参数为 NULL，则为所有窗口全匹配。</param>
        /// <returns>Long，找到的窗口的句柄。如未找到相符窗口，则返回零。</returns>
        [DllImport("User32.dll ")]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childe, string strclass, string FrmText);

        /// <summary>
        /// 获得包含指定点的窗口的句柄
        /// </summary>
        /// <param name="Point">指定一个被检测的点的POINT结构</param>
        /// <returns>返回值为包含该点的窗口的句柄。如果包含指定点的窗口不存在，返回值为NULL。如果该点在静态文本控件之上，返回值是在该静态文本控件的下面的窗口的句柄</returns>
        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [DllImport("user32.dll")]
        static extern IntPtr ChildWindowFromPoint(IntPtr hWnd, POINT Point);

        /// <summary>
        /// 回父窗口中包含了指定点的第一个子窗口的句柄。
        /// </summary>
        /// <param name="hWnd">hWnd 父窗口的句柄</param>
        /// <param name="xPoint">xPoint 点的X坐标，以像素为单位</param>
        /// <param name="yPoint">yPoint 点的Y坐标，以像素为单位</param>
        /// <returns>发现包含了指定点的第一个子窗口的句柄。如未发现任何窗口，则返回hWnd（父窗口的句柄）。如指定点位于父窗口外部，则返回零。</returns>
        [DllImport("user32.dll")]
        static extern IntPtr ChildWindowFromPoint(IntPtr hWnd, int xPoint, int yPoint);

        /// <summary>
        /// 检取光标的位置，以屏幕坐标表示
        /// </summary>
        /// <param name="lpPoint">POINT结构指针，该结构接收光标的屏幕坐标</param>
        /// <returns>
        /// 如果成功，返回值非零；如果失败，返回值为零。
        /// 若想获得更多错误信息，请调用GetLastError函数。
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        /// <summary>
        /// 在WinUser.h中根据是否已定义Unicode被分别定义为SetWindowTextW和SetWindowTextA，
        /// 这两个函数改变指定窗口的标题栏的文本内容（如果窗口有标题栏）。
        /// 如果指定窗口是一个控件，则改变控件的文本内容。
        /// 特别需要引起重视的是，
        /// 【SetWindowText函数不改变在其他应用程序中的控件的文本内容，如果需要可以用另外一个SendMessage函数发送一条WM_SETTEXT消息】。
        /// </summary>
        /// <param name="hWnd">要改变文本内容的窗口或控件的句柄。不能改变在其他应用程序中的控件的文本内容，如果需要可以用另外一个SengMessage函数发送一条WM_SETTEX消息。</param>
        /// <param name="lpString">指向一个空结束的字符串的指针，该字符串将作为窗口或控件的新文本。</param>
        /// <returns>如果函数成功，返回值为非零(在win7中，对其他程序的文本框赋值会返回1，但是无法改变其内容)；如果函数失败，返回值为零。若想获得更多错误信息，请调用GetLastError函数。</returns>
        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string lpString);

        /// <summary>
        /// 发送消息
        /// (需要用HWND_BROADCAST通信的应用程序应当使用函数RegisterWindowMessage来为应用程序间的通信取得一个唯一的消息。)
        /// </summary>
        /// <param name="hWnd">其窗口程序将接收消息的窗口的句柄。如果此参数为HWND_BROADCAST，则消息将被发送到系统中所有顶层窗口，包括无效或不可见的非自身拥有的窗口、被覆盖的窗口和弹出式窗口，但消息不被发送到子窗口。</param>
        /// <param name="Msg">指定被发送的消息。</param>
        /// <param name="wParam">指定附加的消息特定信息。</param>
        /// <param name="lParam">指定附加的消息特定信息。</param>
        /// <returns>返回值指定消息处理的结果，依赖于所发送的消息。</returns>
        [DllImport("user32.dll ", EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, string wParam, string lParam);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// 获取一个前台窗口的句柄（窗口与用户当前的工作）
        /// </summary>
        /// <returns>返回值是一个前台窗口的句柄。在某些情况下，如一个窗口失去激活时，前台窗口可以是NULL。</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 设置指定窗口的显示状态
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="nCmdShow">指定窗口如何显示。如果发送应用程序的程序提供了STARTUPINFO结构，则应用程序第一次调用ShowWindow时该参数被忽略。否则，在第一次调用ShowWindow函数时，该值应为在函数WinMain中nCmdShow参数。在随后的调用中，该参数可以为下列值之一：
        /// SW_FORCEMINIMIZE：在WindowNT5.0中最小化窗口，即使拥有窗口的线程被挂起也会最小化。在从其他线程最小化窗口时才使用这个参数。nCmdShow=11。
        /// SW_HIDE：隐藏窗口并激活其他窗口。nCmdShow=0。
        /// SW_MAXIMIZE：最大化指定的窗口。nCmdShow=3。
        /// SW_MINIMIZE：最小化指定的窗口并且激活在Z序中的下一个顶层窗口。nCmdShow=6。
        /// SW_RESTORE：激活并显示窗口。如果窗口最小化或最大化，则系统将窗口恢复到原来的尺寸和位置。在恢复最小化窗口时，应用程序应该指定这个标志。nCmdShow=9。
        /// SW_SHOW：在窗口原来的位置以原来的尺寸激活和显示窗口。nCmdShow=5。
        /// SW_SHOWDEFAULT：依据在STARTUPINFO结构中指定的SW_FLAG标志设定显示状态，STARTUPINFO 结构是由启动应用程序的程序传递给CreateProcess函数的。nCmdShow=10。
        /// SW_SHOWMAXIMIZED：激活窗口并将其最大化。nCmdShow=3。
        /// SW_SHOWMINIMIZED：激活窗口并将其最小化。nCmdShow=2。
        /// SW_SHOWMINNOACTIVE：窗口最小化，激活窗口仍然维持激活状态。nCmdShow=7。
        /// SW_SHOWNA：以窗口原来的状态显示窗口。激活窗口仍然维持激活状态。nCmdShow=8。
        /// SW_SHOWNOACTIVATE：以窗口最近一次的大小和状态显示窗口。激活窗口仍然维持激活状态。nCmdShow=4。
        /// SW_SHOWNORMAL：激活并显示一个窗口。如果窗口被最小化或最大化，系统将其恢复到原来的尺寸和大小。应用程序在第一次显示窗口的时候应该指定此标志。nCmdShow=1。</param>
        /// <returns>如果窗口之前可见，则返回值为非零。如果窗口之前被隐藏，则返回值为零。</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        /// <summary>
        /// 该函数返回指定窗口的边框矩形的尺寸。
        /// 该尺寸以相对于屏幕坐标左上角的屏幕坐标给出。
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="lpRect">指向一个RECT结构的指针，该结构接收窗口的左上角和右下角的屏幕坐标。</param>
        /// <returns>如果函数成功，返回值为非零：如果函数失败，返回值为零。若想获得更多错误信息，请调用GetLastError函数。</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        /// <summary>
        /// 获得指定窗口所属的类的类名。
        /// </summary>
        /// <param name="hWnd">窗口的句柄及间接给出的窗口所属的类。</param>
        /// <param name="lpString">指向接收窗口类名字符串的缓冲区的指针。</param>
        /// <param name="nMaxCont">指定由参数lpClassName指示的缓冲区的字节数。如果类名字符串大于缓冲区的长度，则多出的部分被截断。</param>
        /// <returns>如果函数成功，返回值为拷贝到指定缓冲区的字符个数：如果函数失败，返回值为0。若想获得更多错误信息，请调用GetLastError函数。</returns>
        [DllImport("user32.dll", EntryPoint = "GetClassName")]
        public static extern int GetClassName(int hWnd, StringBuilder lpString, int nMaxCont);
        /*示例用法
         StringBuilder name = new StringBuilder(256);  
         GetWindowText(hwnd, name, 256);  
         MessageBox.Show(name.ToString());  
         GetClassName(hwnd, name, 256);  
         MessageBox.Show(name.ToString());  
         */


        #region 自定义功能



        /// <summary>
        /// 向光标所在控件输入字符串
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="strInputString"></param>
        /// <returns></returns>
        public static bool InputMessage(IntPtr hWnd, string strInputString)
        {
            var result = SendMessage(hWnd, WM_SETTEXT, "", strInputString);
            if (result != 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 向光标所在控件输入字符串
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="strInputString"></param>
        /// <returns></returns>
        public static bool InputMessageInPtr(IntPtr hWnd, string strInputString)
        {
            IntPtr intPtr = Marshal.StringToHGlobalAnsi(strInputString);
            try
            {

                var result = SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, intPtr);
                if (result != IntPtr.Zero)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //最后释放掉
                Marshal.FreeHGlobal(intPtr);
            }
        }
        /// <summary>
        /// 获取光标位置
        /// </summary>
        /// <returns></returns>
        public static POINT GetCursorPos()
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                return p;
            }
            throw new Exception();
        }
        /// <summary>
        /// 获取光标所在窗体句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr WindowFromPoint()
        {
            POINT p = GetCursorPos();
            return WindowFromPoint(p);
        }

        /// <summary>
        /// 获取窗体的RECT位置
        /// </summary>
        /// <param name="windowPtr">窗体句柄</param>
        /// <returns>位置RECT</returns>
        public static RECT GetWindowSize(IntPtr windowPtr)
        {
            RECT rc = new RECT();
            GetWindowRect(windowPtr, ref rc);
            /*
            int width = rc.Right - rc.Left; //窗口的宽度
            int height = rc.Bottom - rc.Top; //窗口的高度
            int x = rc.Left;
            int y = rc.Top;
             */
            return rc;
        }

        /// <summary>
        /// 获取光标所在控件句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr ChildWindowFromPoint()
        {
            POINT p = GetCursorPos();
            var parentHwnd = WindowFromPoint(p);
            var parentSize = GetWindowSize(parentHwnd);
            var controlHwnd = ChildWindowFromPoint(parentHwnd, p.X - parentSize.Left, p.Y - parentSize.Top);
            return controlHwnd;
        }

        /// <summary>
        /// 向指定句柄发送字符串.
        /// 使用指定编码方式发送字符串
        /// </summary>
        /// <param name="k">目标句柄</param>
        /// <param name="input">待输入字符串</param>
        /// <param name="encoding">编码格式</param>
        public static void InputString(IntPtr k, string input,Encoding encoding)
        {
            //发送字节码，具体编码使用传入编码
            byte[] ch = (encoding.GetBytes(input));
            for (int i = 0; i < ch.Length; i++)
            {
                SendMessage(k, WM_CHAR, ch[i], 0);
            }
        }
        #endregion
    }
}

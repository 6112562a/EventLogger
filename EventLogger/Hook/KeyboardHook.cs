using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace MouseKeyboardLibrary
{

    public enum KeyType
    {
        KeyDown,
        KeyUp,
        None
    }
   
    /// <summary>
    /// HookKeyEventArgs的处理方法
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void HookKeyEventHandler(object sender, HookKeyEventArgs e);


    /// <summary>
    /// Captures global keyboard events
    /// </summary>
    public class KeyboardHook : GlobalHook
    {

        #region Events
        /// <summary>
        /// 记录键盘日志
        /// </summary>
        public event HookKeyEventHandler KeyLog;

        #endregion

        #region Constructor

        public KeyboardHook()
        {

            _hookType = WH_KEYBOARD_LL;

        }

        #endregion

        #region Methods

        protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {

            bool handled = false;

            if (nCode > -1 && (KeyLog != null))
            {

                KeyboardHookStruct keyboardHookStruct =
                    (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                var key = KeyInterop.KeyFromVirtualKey(keyboardHookStruct.vkCode);
                var hookKey = new HookKeyEventArgs()
                {
                    InputKeyType = GetKeyType(wParam),
                    Key = key,
                    EventTimeStamp = DateTime.Now
                };
                KeyLog(this, hookKey);

            }
            return CallNextHookEx(_handleToHook, nCode, wParam, lParam);
        }

        private KeyType GetKeyType(int wParam)
        {
            switch (wParam)
            {

                case WM_KEYDOWN:
                case WM_SYSKEYDOWN:
                    return KeyType.KeyDown;
                case WM_KEYUP:
                case WM_SYSKEYUP:
                    return KeyType.KeyUp;
            }
            return KeyType.None;
        }
        #endregion

    }

}

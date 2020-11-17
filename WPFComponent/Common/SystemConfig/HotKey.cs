using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace WPFComponent.Common.SystemConfig
{
    public class HotKey
    {
        public delegate void HotKeyEventHandler();

        [Flags]
        public enum KeyFlags
        {
            MOD_ALT = 0x0001,
            MOD_CONTROL = 0x0002,
            MOD_SHIFT = 0x0004,
            MOD_WIN = 0x0008,
            MOD_NOREPEAT=0x4000
        }
        /// <summary>
        /// 热键消息编号
        /// </summary>
        //private const int WM_HotKeys = 0x0312;
        /// <summary>
        /// 热键字典
        /// </summary>
        private static readonly Dictionary<int,HotKey> DicHotKeys=new Dictionary<int, HotKey>();

        /// <summary>
        /// 窗体句柄
        /// </summary>
        private readonly IntPtr _Handle;

        /// <summary>
        /// 热键编号
        /// </summary>
        private readonly int _KeyId;

        /// <summary>
        /// 热键注册窗体
        /// </summary>
        private readonly Window _Window;

        /// <summary>
        /// 热键控制键
        /// </summary>
        private int _ControlKey;

        /// <summary>
        /// 热键主键
        /// </summary>
        private int _Key;

        /// <summary>
        /// 是否热键注册完成
        /// </summary>
        public bool IsRegisterOK;

        /// <summary>
        /// 热键触发事件
        /// </summary>
        public event HotKeyEventHandler OnHotKeyHit;
        /// <summary>
        /// 析构
        /// </summary>
        ~HotKey()
        {
            UnregisterHotKey(_Handle, _KeyId);
        }
        /// <summary>
        /// 注册热键构造
        /// </summary>
        /// <param name="win">窗体</param>
        /// <param name="handle">句柄</param>
        /// <param name="control">控制键（Ctrl/Alt/Shift/Win等）</param>
        /// <param name="keycode">键盘虚拟码</param>
        public HotKey(Window win, IntPtr handle, KeyFlags control, Key keycode)
        {
            _Handle = handle;
            _Window = win;
            _ControlKey = (int)control;
            _Key = KeyInterop.VirtualKeyFromKey(keycode);
            _KeyId = _Key*10 + _ControlKey;
            if (DicHotKeys.ContainsKey(_KeyId)) return;
            //---注册热键---
            IsRegisterOK = RegisterHotKey(_Handle, _KeyId, _ControlKey, _Key);
            if (IsRegisterOK&&!DicHotKeys.ContainsKey(_KeyId))
            {
                if (HookHotKey(this))
                {
                    DicHotKeys.Add(_KeyId,this);
                }
            }

        }

        #region Win Api

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, int vk);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private static bool HookHotKey(HotKey key)
        {
            if (key._Window == null || key._Handle == IntPtr.Zero)
            {
                return false;
            }
            HwndSource source=HwndSource.FromHwnd(key._Handle);
            if (source == null) return false;
            source.AddHook(HotkeyHook);
            return true;
        }
        /// <summary>
        /// 热键处理过程
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private static IntPtr HotkeyHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //Debug.WriteLine("hwnd:{0},msg:{1},wParam:{2},lParam:{3},handle:{4}", hWnd, msg, wParam, lParam, handled);
            var keyId = wParam.ToInt32();
            var matchHotkey = DicHotKeys.Values.FirstOrDefault(x => x._KeyId == keyId);
            if (matchHotkey != null)
            {
                if (matchHotkey.OnHotKeyHit != null)
                {
                    matchHotkey.OnHotKeyHit();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }
        #endregion
    }
}

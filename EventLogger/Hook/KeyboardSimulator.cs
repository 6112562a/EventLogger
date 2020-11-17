using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace MouseKeyboardLibrary
{
    /// <summary>
    /// Standard Keyboard Shortcuts used by most applications
    /// </summary>
    public enum StandardShortcut
    {
        Copy,
        Cut,
        Paste,
        SelectAll,
        Save,
        Open,
        New,
        Close,
        Print
    }
    /// <summary>
    /// Simulate keyboard key presses
    /// </summary>
    public static class KeyboardSimulator
    {

        #region Windows API Code

        const int KEYEVENTF_EXTENDEDKEY = 0x1;
        const int KEYEVENTF_KEYUP = 0x2;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte key, byte scan, int flags, int extraInfo);

        #endregion

        #region Methods

        public static void KeyDown(Key key)
        {
            keybd_event(ParseKey(key), 0, 0, 0);
        }

        public static void KeyUp(Key key)
        {
            keybd_event(ParseKey(key), 0, KEYEVENTF_KEYUP, 0);
        }

        public static void KeyPress(Key key)
        {
            KeyDown(key);
            KeyUp(key);
        }
        /// <summary>
        /// 模拟快捷键
        /// </summary>
        /// <param name="shortcut">快捷键</param>
        public static void SimulateStandardShortcut(StandardShortcut shortcut)
        {
            switch (shortcut)
            {
                case StandardShortcut.Copy:
                    KeyDown(Key.LeftCtrl);
                    KeyPress(Key.C);
                    KeyUp(Key.LeftCtrl);
                    break;
                case StandardShortcut.Cut:
                    KeyDown(Key.LeftCtrl);
                    KeyPress(Key.X);
                    KeyUp(Key.LeftCtrl);
                    break;
                case StandardShortcut.Paste:
                    KeyDown(Key.LeftCtrl);
                    KeyPress(Key.V);
                    KeyUp(Key.LeftCtrl);
                    break;
                case StandardShortcut.SelectAll:
                    KeyDown(Key.LeftCtrl);
                    KeyPress(Key.A);
                    KeyUp(Key.LeftCtrl);
                    break;
                case StandardShortcut.Save:
                    KeyDown(Key.LeftCtrl);
                    KeyPress(Key.S);
                    KeyUp(Key.LeftCtrl);
                    break;
                case StandardShortcut.Open:
                    KeyDown(Key.LeftCtrl);
                    KeyPress(Key.O);
                    KeyUp(Key.LeftCtrl);
                    break;
                case StandardShortcut.New:
                    KeyDown(Key.LeftCtrl);
                    KeyPress(Key.N);
                    KeyUp(Key.LeftCtrl);
                    break;
                case StandardShortcut.Close:
                    KeyDown(Key.LeftAlt);
                    KeyPress(Key.F4);
                    KeyUp(Key.LeftAlt);
                    break;
                case StandardShortcut.Print:
                    KeyDown(Key.LeftCtrl);
                    KeyPress(Key.P);
                    KeyUp(Key.LeftCtrl);
                    break;
            }
        }

        static byte ParseKey(Key key)
        {

            // Alt, Shift, and Control need to be changed for API function to work with them
            var value = KeyInterop.VirtualKeyFromKey(key);
            return (byte)value;
        }

        #endregion

    }

}

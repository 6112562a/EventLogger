using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EventLogger.Annotations;

namespace MouseKeyboardLibrary
{
    /// <summary>
    /// 钩子键盘处理参数
    /// </summary>
    [Serializable]
    public class HookKeyEventArgs : INotifyPropertyChanged
    {
        private Key _key;
        private KeyType _inputKeyType;
        private DateTime _eventTimeStamp;
        private string _inputString;

        /// <summary>
        /// 按键
        /// </summary>
        public Key Key
        {
            get { return _key; }
            set
            {
                if (value == _key) return;
                _key = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 按键类别（按下，释放）
        /// </summary>
        public KeyType InputKeyType
        {
            get { return _inputKeyType; }
            set
            {
                if (value == _inputKeyType) return;
                _inputKeyType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 鼠标事件时间戳
        /// </summary>
        public DateTime EventTimeStamp
        {
            get { return _eventTimeStamp; }
            set
            {
                if (value.Equals(_eventTimeStamp)) return;
                _eventTimeStamp = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 输入字符串
        /// </summary>
        public string InputString
        {
            get { return _inputString; }
            set
            {
                if (value == _inputString) return;
                _inputString = value;
                OnPropertyChanged();
            }
        }

        public HookKeyEventArgs()
        {
            InputKeyType=KeyType.None;
        }

        public static HookKeyEventArgs Copy(HookKeyEventArgs hookKeyEvent)
        {
            var newEvent=new HookKeyEventArgs();
            newEvent.Key = hookKeyEvent.Key;
            newEvent.InputKeyType = hookKeyEvent.InputKeyType;
            newEvent.EventTimeStamp = hookKeyEvent.EventTimeStamp;
            newEvent.InputString = hookKeyEvent.InputString;
            return newEvent;
        }


        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

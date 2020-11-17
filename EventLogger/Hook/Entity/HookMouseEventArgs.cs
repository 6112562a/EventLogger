using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using EventLogger.Annotations;
using MouseKeyboardLibrary;

namespace MouseKeyboardLibrary
{
    /// <summary>
    /// 自定义鼠标事件参数
    /// </summary>
    [Serializable]
    public class HookMouseEventArgs : INotifyPropertyChanged
    {
        private Point _mousePosition;
        private MouseEventType _mouseEventType;
        private MouseButton _clickButton;
        private int _clickCount;
        private int _mouseWheelDelta;
        private DateTime _eventTimeStamp;

        /// <summary>
        /// 鼠标屏幕位置
        /// </summary>
        public Point MousePosition
        {
            get { return _mousePosition; }
            set
            {
                if (value.Equals(_mousePosition)) return;
                _mousePosition = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// X位置
        /// </summary>
        public double XPosition
        {
            get { return _mousePosition.X; }
            set
            {
                if (value.Equals(_mousePosition.X)) return;
                _mousePosition.X = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Y位置
        /// </summary>
        public double YPosition
        {
            get { return _mousePosition.Y; }
            set
            {
                if (value.Equals(_mousePosition.Y)) return;
                _mousePosition.Y = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 鼠标事件类型
        /// </summary>
        public MouseEventType MouseEventType
        {
            get { return _mouseEventType; }
            set
            {
                if (value == _mouseEventType) return;
                _mouseEventType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 点击按钮
        /// </summary>
        public MouseButton ClickButton
        {
            get { return _clickButton; }
            set
            {
                if (value == _clickButton) return;
                _clickButton = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 点击次数
        /// </summary>
        public int ClickCount
        {
            get { return _clickCount; }
            set
            {
                if (value == _clickCount) return;
                _clickCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 鼠标滚轮
        /// </summary>
        public int MouseWheelDelta
        {
            get { return _mouseWheelDelta; }
            set
            {
                if (value == _mouseWheelDelta) return;
                _mouseWheelDelta = value;
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
        /// 复制生成一个新对象
        /// </summary>
        /// <param name="hookMouseEvent"></param>
        /// <returns></returns>
        public static HookMouseEventArgs Copy(HookMouseEventArgs hookMouseEvent)
        {
            var newEvent=new HookMouseEventArgs();
            newEvent.XPosition = hookMouseEvent.XPosition;
            newEvent.YPosition = hookMouseEvent.YPosition;
            newEvent.MousePosition = hookMouseEvent.MousePosition;
            newEvent.MouseEventType = hookMouseEvent.MouseEventType;
            newEvent.MouseWheelDelta = hookMouseEvent.MouseWheelDelta;
            newEvent.EventTimeStamp = hookMouseEvent.EventTimeStamp;
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

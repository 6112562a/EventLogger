using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace WPFComponent.Common.Threads
{
    public static class UIThread
    {
        /// <summary>
        /// 判断当前线程是否是UI线程
        /// </summary>
        /// <returns>是--True；否--false</returns>
        public static bool IsCurrentUIThread()
        {
            if (Application.Current == null) return false;//Application周期在退出时会被置空，异步线程调用可能会报错
            return Thread.CurrentThread.ManagedThreadId == Application.Current.Dispatcher.Thread.ManagedThreadId;
        }
        /// <summary>
        /// 使用最高优先级同步调用委托
        /// （可以认为是同步操作）
        /// </summary>
        /// <param name="action">委托</param>
        public static void Invoke(Action action)
        {
            if (Application.Current == null) return;//Application周期在退出时会被置空，异步线程调用可能会报错
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, action);
        }
        /// <summary>
        /// 使用指定权限同步调用委托
        /// </summary>
        /// <param name="action">委托</param>
        /// <param name="priority">指定优先级</param>
        public static void Invoke(Action action, DispatcherPriority priority)
        {
            if (Application.Current == null) return;//Application周期在退出时会被置空，异步线程调用可能会报错
            System.Windows.Application.Current.Dispatcher.Invoke(action, priority);
        }
        /// <summary>
        /// 按照呈现级别异步调用委托
        /// （可以认为是异步操作）
        /// </summary>
        /// <param name="action">委托</param>
        public static void BeginInvoke(Action action)
        {
            if (Application.Current == null) return;//Application周期在退出时会被置空，异步线程调用可能会报错
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render, action);
        }
        /// <summary>
        /// 使用指定优先级异步调用委托
        /// </summary>
        /// <param name="action">委托</param>
        /// <param name="priority">指定优先级</param>
        public static void BeginInvoke(Action action, DispatcherPriority priority)
        {
            if (Application.Current == null) return;//Application周期在退出时会被置空，异步线程调用可能会报错
            System.Windows.Application.Current.Dispatcher.BeginInvoke(priority, action);
        }
        /// <summary>
        /// 休眠以释放CPU控制权。
        /// </summary>
        public static void SleepForReleaseCPU()
        {
            if (IsCurrentUIThread())
            {
                Invoke(()=>{Thread.Sleep(10);},DispatcherPriority.Background);//低优先级执行休眠，避免阻塞UI线程
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }
}

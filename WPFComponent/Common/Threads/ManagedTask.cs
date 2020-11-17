using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponent.Common.Threads
{
    public static class ManagedTask
    {
        /// <summary>
        /// 使用<see cref="Task.Factory"/>方式调用委托
        /// </summary>
        /// <param name="action">委托</param>
        /// <returns>Task</returns>
        public static Task FactoryStart(Action action)
        {
            return Task.Factory.StartNew(() =>
            {
                CatchExForAction.ExceptionToUIThread("Task.Factory.StartNew Error!", action);
            });
        }
    }
}

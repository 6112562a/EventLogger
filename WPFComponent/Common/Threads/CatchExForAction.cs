using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFComponent.Common.Log;

namespace WPFComponent.Common.Threads
{
    public static class CatchExForAction
    {
        /// <summary>
        /// 捕捉异常，记录日志，并反馈到UI层
        /// </summary>
        /// <param name="strErrorTitle">异常Title</param>
        /// <param name="action">调用的委托</param>
        public static void ExceptionToUIThread(string strErrorTitle, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex,strErrorTitle,action.ToString());
                throw;
            }
        }

    }
}

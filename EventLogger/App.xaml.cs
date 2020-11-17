using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EventLogger
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 改动启动方式
        /// 举例：
        /// -Mouse="F:\Projects\EventLogger\EventLogger\bin\Debug\2017-01-11 0906_MouseDB.db" 
        /// -Key="F:\Projects\EventLogger\EventLogger\bin\Debug\2017-01-11 0906_KeyDB.db" 
        /// -key=045304
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //------接收命令行参数，快捷执行------
            if (e.Args.Length > 0)
            {
                Console.WriteLine(e.Args.ToString());
                MainWindow win = new MainWindow(e.Args);
                win.Show();
            }
            else
            {
                MainWindow win = new MainWindow();
                win.Show();
            }
            base.OnStartup(e);
        }
    }
}

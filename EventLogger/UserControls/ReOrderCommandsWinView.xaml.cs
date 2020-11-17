using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EventLogger.Annotations;
using EventLogger.Log;
using Microsoft.Win32;
using MouseKeyboardLibrary;
using WPFComponent.Adorners;
using WPFComponent.Common.UI;

namespace EventLogger
{
    /// <summary>
    /// 日志存档文件对象
    /// </summary>
    public class HookFile : INotifyPropertyChanged
    {
        private string _fileName;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        private int _opDiffTime;
        /// <summary>
        /// 操作间隔
        /// (精确度毫秒)
        /// </summary>
        public int OpDiffTime
        {
            get { return _opDiffTime; }
            set
            {
                _opDiffTime = value;
                OnPropertyChanged();
            }
        }
        
        #region Implements
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    /// <summary>
    /// ReOrderCommandsWinView.xaml 的交互逻辑
    /// </summary>
    public partial class ReOrderCommandsWinView : Window, INotifyPropertyChanged
    {
        #region Implements

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        public ReOrderCommandsWinView()
        {
            InitializeComponent();
        }

        #region Fields

        private ObservableCollection<HookFile> _mouseHisDataList;
        /// <summary>
        /// 鼠标历史记录
        /// </summary>
        public ObservableCollection<HookFile> MouseHisDataList
        {
            get { return _mouseHisDataList ?? (_mouseHisDataList = new ObservableCollection<HookFile>()); }
            set
            {
                _mouseHisDataList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<HookFile> _keyHisDataList;
        /// <summary>
        /// 键盘历史记录
        /// </summary>
        public ObservableCollection<HookFile> KeyHisDataList
        {
            get { return _keyHisDataList ?? (_keyHisDataList = new ObservableCollection<HookFile>()); }
            set
            {
                _keyHisDataList = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region 表格右键菜单

        /// <summary>
        /// 添加鼠标记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMenuItem_Mouse_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "db日志|*.db|所有文件|*.*";
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                var fileData = new HookFile() { FileName = fileName, OpDiffTime = 0 };
                MouseHisDataList.Add(fileData);
            }
        }
        /// <summary>
        /// 删除鼠标记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMenuItem_Mouse_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedItems = new List<HookFile>();
            foreach (var selectedItem in MouseHisDataGrid.SelectedItems)
            {
                var selectedHookFile = selectedItem as HookFile;
                if (selectedHookFile != null)
                {
                    selectedItems.Add(selectedHookFile);
                }
            }
            foreach (var selectedItem in selectedItems)
            {
                MouseHisDataList.Remove(selectedItem);
            }
        }
        /// <summary>
        /// 添加键盘记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMenuItem_Key_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "db日志|*.db|所有文件|*.*";
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                var fileData = new HookFile() { FileName = fileName, OpDiffTime = 0 };
                KeyHisDataList.Add(fileData);
            }
        }
        /// <summary>
        /// 删除键盘记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMenuItem_Key_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedItems = new List<HookFile>();
            foreach (var selectedItem in KeyHisDataGrid.SelectedItems)
            {
                var selectedHookFile = selectedItem as HookFile;
                if (selectedHookFile != null)
                {
                    selectedItems.Add(selectedHookFile);
                }
            }
            foreach (var selectedItem in selectedItems)
            {
                KeyHisDataList.Remove(selectedItem);
            }
        }
        #endregion

        /// <summary>
        /// 获取鼠标组合后的结果
        /// </summary>
        /// <returns></returns>
        public IList<HookMouseEventArgs> GetMouseCombineResult()
        {
            var resultList = new List<HookMouseEventArgs>();
            if (MouseHisDataList.Count == 0) return resultList;
            var firstHookFile = MouseHisDataList.First();//第一行记录
            var firstData = CustomEventLogger.GetHookMouseEventArgses(firstHookFile.FileName);
            //第一行，直接加入集合
            foreach (var hookMouseEventArgse in firstData)
            {
                resultList.Add(hookMouseEventArgse);
            }
            foreach (var hookFile in MouseHisDataList)
            {
                var tmpData = CustomEventLogger.GetHookMouseEventArgses(hookFile.FileName);//当前行历史记录
                if (hookFile != firstHookFile)
                {//重新根据diffTime组合
                    var tmp_Previous = tmpData.First();
                    foreach (var hookMouseEventArgse in tmpData)
                    {
                        var copyEvent = HookMouseEventArgs.Copy(hookMouseEventArgse);//拷贝新对象，用于新集合
                        var diffTimeSpan = copyEvent.EventTimeStamp - tmp_Previous.EventTimeStamp;//得出每次记录的间隔
                        if (tmp_Previous == tmpData.First())
                            diffTimeSpan = TimeSpan.FromMilliseconds(hookFile.OpDiffTime);//新行的首次记录附加到前一个结果后面
                        copyEvent.EventTimeStamp = resultList.Last().EventTimeStamp.Add(diffTimeSpan);//重新设定新的时间戳
                        //--加入集合--
                        resultList.Add(copyEvent);
                        //-设置值-
                        tmp_Previous = hookMouseEventArgse;
                    }
                }
            }
            return resultList;
        }

        /// <summary>
        /// 获取键盘组合后的结果
        /// </summary>
        /// <returns></returns>
        public IList<HookKeyEventArgs> GetKeyCombineResult()
        {
            var resultList = new List<HookKeyEventArgs>();
            if (KeyHisDataList.Count == 0) return resultList;
            var firstHookFile = KeyHisDataList.First();//第一行记录
            var firstData = CustomEventLogger.GetHookKeyEventArgses(firstHookFile.FileName);
            //第一行，直接加入集合
            foreach (var hookKeyEventArgse in firstData)
            {
                resultList.Add(hookKeyEventArgse);
            }
            foreach (var hookFile in KeyHisDataList)
            {
                var tmpData = CustomEventLogger.GetHookKeyEventArgses(hookFile.FileName);//当前行历史记录
                if (hookFile != firstHookFile)
                {//重新根据diffTime组合
                    var tmp_Previous = tmpData.First();
                    foreach (var hookKeyEventArgs in tmpData)
                    {
                        var copyEvent = HookKeyEventArgs.Copy(hookKeyEventArgs);//拷贝新对象，用于新集合
                        var diffTimeSpan = copyEvent.EventTimeStamp - tmp_Previous.EventTimeStamp;//得出每次记录的间隔
                        if (tmp_Previous == tmpData.First())
                            diffTimeSpan = TimeSpan.FromMilliseconds(hookFile.OpDiffTime);//新行的首次记录附加到前一个结果后面
                        copyEvent.EventTimeStamp = resultList.Last().EventTimeStamp.Add(diffTimeSpan);//重新设定新的时间戳
                        //--加入集合--
                        resultList.Add(copyEvent);
                        //-设置值-
                        tmp_Previous = hookKeyEventArgs;
                    }
                }
            }
            return resultList;
        }
        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        //---------------------------支持鼠标拖拽----------------------------------
        /// <summary>
        /// 正在执行拖拽
        /// </summary>
        private bool isDragging;

        private void MouseHisDataGrid_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //---获取拖拽行---
            Point position = e.GetPosition(MouseHisDataGrid);
            var row = UIHelpers.TryFindFromPoint<DataGridRow>(MouseHisDataGrid, position);
            if (row == null) return;
            isDragging = true;//开始拖拽
            DragDropAdorner adorner = new DragDropAdorner(row);
            var mAdornerLayer = AdornerLayer.GetAdornerLayer(MouseHisDataGrid); // Window class do not have AdornerLayer
            mAdornerLayer.Add(adorner);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPFComponent.Behaviors
{
    public class DataGridRowBehavior
    {




        public static bool GetShowIndex(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowIndexProperty);
        }

        public static void SetShowIndex(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowIndexProperty, value);
        }

        /// <summary>
        /// 展示索引列
        /// </summary>
        public static readonly DependencyProperty ShowIndexProperty =
            DependencyProperty.RegisterAttached("ShowIndex", typeof(bool), typeof(DataGridRowBehavior), new PropertyMetadata(false,OnShowIndexPropertyChanged));

        private static void OnShowIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGrid;
            if (dataGrid == null) return;
            dataGrid.LoadingRow += dataGrid_LoadingRow;
            if (dataGrid.IsLoaded)
            {
                dataGrid.InvalidateArrange();
            }
        }

        static void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null) return;
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using WPFComponent.Common;

namespace WPFComponent.Converters
{
    /// <summary>
    /// 指定类型转换成Icon图片源
    /// </summary>
    public class String2ImageSourceConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var strValue = value.ToString();
            return DrawingHelper.Convert2Image(new[] {strValue + "Path"});
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

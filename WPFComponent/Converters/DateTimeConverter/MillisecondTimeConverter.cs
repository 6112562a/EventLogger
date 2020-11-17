using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WPFComponent.Converters.DateTimeConverter
{
    /// <summary>
    /// 毫秒级时间格式转换器
    /// </summary>
    public class MillisecondTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //yyyy-MM-dd HH:mm:ss fff
                //格式字符串转化为时间格式
                if (targetType != typeof(DateTime))
                    return value;
                var strSource = value.ToString();
                var splitStrArr = strSource.Split(' ');
                var dateTimeResult = DateTime.Parse(string.Format("{0} {1}", splitStrArr[0], splitStrArr[1]));
                dateTimeResult = dateTimeResult.AddMilliseconds(Double.Parse(splitStrArr[2]));
                return dateTimeResult;
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}

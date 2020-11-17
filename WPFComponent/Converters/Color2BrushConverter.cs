using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFComponent.Converters
{
    public class Color2BrushConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as System.Windows.Media.Color?;
            if (color == null) return null;
            var brush = new SolidColorBrush(color.Value);
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

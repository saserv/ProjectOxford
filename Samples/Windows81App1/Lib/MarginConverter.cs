using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Windows81App1.Lib
{
    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string leftTop = value.ToString();
            return new Thickness(0, System.Convert.ToDouble(value), 0, 0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}

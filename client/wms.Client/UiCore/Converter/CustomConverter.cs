using System;
using System.Globalization;
using System.Windows.Data;

namespace wms.Client.UiCore.Converter
{
    public class CustomConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && int.TryParse(value.ToString(), out int result))
            {
                if (result.Equals(0))
                    return "";
                else
                    return "+" + result;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && bool.TryParse(value.ToString(), out bool result))
            {
                if (result)
                    return 1;
                else
                    return 0;
            }
            return false;
        }

    }
}

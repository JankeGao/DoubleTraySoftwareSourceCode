using System;
using System.Globalization;
using System.Windows.Data;

namespace wms.Client.Common.Converters
{
    /// <summary>
    /// 菜单宽度转换器
    /// </summary>
    public class MenuWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && bool.TryParse(value.ToString(), out bool result))
            {
                if (result)
                    return 0;
            }
            return 220;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

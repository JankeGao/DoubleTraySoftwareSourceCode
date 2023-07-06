using System;
using System.Globalization;
using System.Windows.Data;

namespace wms.Client.Common.Converters
{
    /// <summary>
    /// 01 /true false 类型转换器
    /// </summary>
    internal class IBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && int.TryParse(value.ToString(), out int result))
            {
                if (result == 0)
                    return false;
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value!=null&&bool.TryParse(value.ToString(),out bool result))
            {
                if (result)
                    return 1;
                else
                    return 0;
            }
            return 0;
        }
    }
}

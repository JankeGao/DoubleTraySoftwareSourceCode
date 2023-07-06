using System;
using System.Globalization;
using System.Windows.Data;

namespace wms.Client.UiCore.Converter
{
    /// <summary>
    /// 0/1 是否转换器
    /// </summary>
    public class FlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && int.TryParse(value.ToString(), out int result))
            {
                if (result.Equals(1))
                    return "是";
                else
                    return "否";
            }
            return "否";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

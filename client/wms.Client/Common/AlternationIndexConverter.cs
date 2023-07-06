using HP.Utility.Files;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using wms.Client.LogicCore.Configuration;

namespace wms.Client.Common
{
    public class AlternationIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cfgINI = AppDomain.CurrentDomain.BaseDirectory + SerivceFiguration.INI_CFG;
            string IsDoubleTray = string.Empty;
            if (File.Exists(cfgINI))
            {
                IniFile ini = new IniFile(cfgINI);
                IsDoubleTray = ini.IniReadValue("ClientInfo", "IsDoubleTray");
            }
            if(IsDoubleTray == "true")
            {
                int index = (int)value;
                return index >= 1 && index <= 10 ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CustomCommandBarCreator.Converters
{
    public class BoolToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = 0;
            if(value is bool)
                if((bool)value)
                    result = 46;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = false;
            if(value is int)
            {
                if ((int)value == 46)
                    result = true;
            }
            return result;
        }
    }
}

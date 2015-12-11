using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Data;

namespace SmartScale.ValueConverters
{
    public class FloatToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //culture = new System.Globalization.CultureInfo("en-GB");
            // Maksymalnie dwa znaki po przecinku
            return (string.Format("{0:0.0}", value)).Replace(",",".");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}

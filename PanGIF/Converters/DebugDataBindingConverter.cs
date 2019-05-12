using Avalonia.Data.Converters;
using System;
using System.Diagnostics;
using System.Globalization;

namespace PanGIF.Converters
{
    class DebugDataBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}
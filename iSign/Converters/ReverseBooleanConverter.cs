using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace iSign.Converters
{
    public class ReverseBooleanConverter : IMvxValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException ();
        }
    }
}

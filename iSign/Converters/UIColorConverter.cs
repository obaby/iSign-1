using System;
using System.Globalization;
using iSign.Extensions;
using MvvmCross.Platform.Converters;

namespace iSign.Converters
{
    public class UIColorConverter : IMvxValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as string;
            return color?.ToUIColor ();
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException ();
        }
    }
}

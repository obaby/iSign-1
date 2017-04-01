using System;
using System.Globalization;
using MvvmCross.Platform.Converters;
using UIKit;

namespace iSign
{
    public class UIColorConverter : IMvxValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as string;
            if (color == null) return null;
            return color.ToUIColor ();
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException ();
        }
    }
}

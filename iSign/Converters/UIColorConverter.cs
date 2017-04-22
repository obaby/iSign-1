using System;
using System.Globalization;
using iSign.Core;
using iSign.Extensions;
using MvvmCross.Platform.Converters;
using UIKit;

namespace iSign.Converters
{
    public class UIColorConverter : IMvxValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            var texture = value as Texture;
            if (texture == null) return null;
            return texture.IsColor ? texture.Color.Value.ToUIColor() : UIColor.FromPatternImage (new UIImage (texture.Path));
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException ();
        }
    }
}

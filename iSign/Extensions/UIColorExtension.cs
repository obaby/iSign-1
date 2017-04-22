using System;
using System.Collections.Generic;
using iSign.Services;
using UIKit;

namespace iSign.Extensions
{
    public static class UIColorExtension
    {
        private static Dictionary<Color, string> Colors = new Dictionary<Color, string> {
            { Color.Red, ColorDefinition.Red},
            { Color.Blue, ColorDefinition.Blue},
            { Color.Black, ColorDefinition.Black},
            { Color.Yellow, ColorDefinition.Yellow},
            { Color.Orange, ColorDefinition.Orange},
            { Color.White, ColorDefinition.White},
            { Color.Purple, ColorDefinition.Purple},
            { Color.Green, ColorDefinition.Green},
        };
        public static UIColor ToUIColor (this Color color)
        {
            var hexa = Colors [color];
            var colorString = hexa.Replace ("#", "");
            float red, green, blue;
            UIColor result;

            switch (colorString.Length) {
            case 3: {
                    red = Convert.ToInt32 (string.Format ("{0}{0}", colorString.Substring (0, 1)), 16) / 255f;
                    green = Convert.ToInt32 (string.Format ("{0}{0}", colorString.Substring (1, 1)), 16) / 255f;
                    blue = Convert.ToInt32 (string.Format ("{0}{0}", colorString.Substring (2, 1)), 16) / 255f;
                    result = UIColor.FromRGBA (red, green, blue, 1f);
                    break;
                }
            case 6: {
                    red = Convert.ToInt32 (colorString.Substring (0, 2), 16) / 255f;
                    green = Convert.ToInt32 (colorString.Substring (2, 2), 16) / 255f;
                    blue = Convert.ToInt32 (colorString.Substring (4, 2), 16) / 255f;
                    result = UIColor.FromRGBA (red, green, blue, 1f);
                    break;
                }

            default:
                return null;
            }

            return result;
        }

        // ReSharper disable once InconsistentNaming
        public static string ToPCLColor (this UIColor color)
        {
            var red = (int)(color.CGColor.Components [0] * 255);
            var green = (int)(color.CGColor.Components [1] * 255);
            var blue = (int)(color.CGColor.Components [2] * 255);

            return $"#{red:X2}{green:X2}{blue:X2}";
        }
    }
}

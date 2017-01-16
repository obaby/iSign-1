using System;
using UIKit;

namespace iSign
{
    public static class UIColorExtension
    {
        public static UIColor ToUIColor (this string color)
        {
            var colorString = color.Replace ("#", "");
            float red, green, blue;
            UIColor result = null;

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
    }
}

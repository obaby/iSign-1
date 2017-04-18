using System;
using CoreGraphics;
namespace iSign
{
    public static class CGRectExtensions
    {
        public static CGRect Ceiling (this CGRect self)
        {
            var width = Math.Ceiling (self.Width);
            var height = Math.Ceiling (self.Height);
            return new CGRect (self.Location, new CGSize (width, height));
        }

        public static bool IsSmallerThan (this CGRect self, CGRect other)
        {
            return self.Size.IsSmallerThan (other.Size);
        }

        public static bool IsSmallerThan (this CGSize self, CGSize other)    
        {
            return self.Width < other.Width || self.Height < other.Height;
        }
    }
}

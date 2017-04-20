using System;
using CoreGraphics;
using UIKit;

namespace iSign
{
    public interface IImageView
    {
        CGRect Frame { get; set; }
        CGSize MinimumSize { get; }
        Action OkAction { get; set; }
        Action CancelAction { get; set; }
        object DataContext { get; set; }
        ImageText GetImage ();
        void StartWith (ImageText imageText);
        void CloseView ();
   }

    public class ImageText
    {
        public UIImage Image { get; set; }
        public string Text { get; set; }
    }
}
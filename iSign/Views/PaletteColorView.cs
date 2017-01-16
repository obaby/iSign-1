using System;
using CoreGraphics;
using iSign.Core;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace iSign
{
    public class PaletteColorView : MvxView
    {
        public PaletteColorView ()
        {
            var tap = new UITapGestureRecognizer (Tapped);
            AddGestureRecognizer (tap);
        }
        private PaletteColorViewModel Context => DataContext as PaletteColorViewModel;
        public override void WillMoveToSuperview (UIView newsuper)
        {
            base.WillMoveToSuperview (newsuper);
            BackgroundColor = Context.Color.ToUIColor();
            Context.PropertyChanged -= Context_PropertyChanged;
            Context.PropertyChanged += Context_PropertyChanged;
            var size = (nfloat) Math.Min (Frame.Height, Frame.Width);
            Frame = new CGRect (Frame.Location, new CGSize (size, size));
            Layer.CornerRadius = size / 2;
        }

        private void Tapped ()
        {
            Context.IsSelected = true;
        }

        void Context_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof (Context.IsSelected)) {
                if (Context.IsSelected) {
                    Layer.BorderWidth = 2;
                    Layer.BorderColor = UIColor.White.CGColor;
                    return;
                }
                Layer.BorderWidth = 0;
            }
        }
    }
}

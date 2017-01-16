using System;
using System.Collections.Generic;
using CoreGraphics;
using iSign.Core;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace iSign
{
    public class PaletteView : MvxView
    {
        private const int Margin = 10;
        private const int XMargin = 40;
        private List<PaletteColorView> Colors { get; }
        public PaletteView ()
        {
            BackgroundColor = UIColor.Gray;
        }

        public void Layout ()
        {
            var context = DataContext as PaletteViewModel;
            var x = (nfloat)XMargin;
            var withMargin = true;
            var size = Frame.Height - 2 * Margin;
            if (size <= 0) {
                size = Frame.Height;
                withMargin = false;
            }
            var y = withMargin ? Margin : 0;
            foreach (var colorVm in context.PaletteColors) {
                var colorView = new PaletteColorView ();
                colorView.DataContext = colorVm;
                colorView.Frame = new CGRect (x, y, size, size);
                Add (colorView);
                x += size + XMargin;
                if (x >= Frame.Width - size - XMargin) break;
            }
        }
    }
}

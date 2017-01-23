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
        private UIButton _undoButton { get; }
        public PaletteView ()
        {
            BackgroundColor = UIColor.Gray;
            _undoButton = new UIButton ();
            _undoButton.SetTitle ("Undo", UIControlState.Normal);
            _undoButton.TouchUpInside += (sender, e) => Context.Undo ();
        }
        PaletteViewModel Context => DataContext as PaletteViewModel;
        
        public void Layout ()
        {
            var x = (nfloat)XMargin;
            var withMargin = true;
            var size = Frame.Height - 2 * Margin;
            if (size <= 0) {
                size = Frame.Height;
                withMargin = false;
            }
            var y = withMargin ? Margin : 0;

            _undoButton.Frame = new CGRect (x, y, 10, 10);
           
            Add (_undoButton);
            _undoButton.SizeToFit ();
            x += _undoButton.Frame.Width + XMargin;

            foreach (var colorVm in Context.PaletteColors) {
                var colorView = new PaletteColorView ();
                colorView.DataContext = colorVm;
                colorView.Frame = new CGRect (x, y, size, size);
                Add (colorView);
                x += size + XMargin;
                if (x >= Frame.Width - size - XMargin) break;
            }
        }

        internal void UpdateUndo (bool canUndo)
        {
            //_undoButton.Enabled = canUndo;
        }
   }
}

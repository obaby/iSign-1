using System;
using System.Collections.Generic;
using CoreGraphics;
using iSign.Core;
using MvvmCross.Binding.BindingContext;
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
        private UISlider _thicknessSlider { get; }
        public PaletteView ()
        {
            BackgroundColor = UIColor.Gray;
            _undoButton = new UIButton ();
            _thicknessSlider = new UISlider ();
            this.DelayBind (() => {
                var set = this.CreateBindingSet<PaletteView, PaletteViewModel> ();
                set.Bind (_undoButton)
                   .For ("Title")
                   .To (vm => vm.UndoText);
                set.Bind (_undoButton)
                   .To (vm => vm.UndoCommand);

                set.Bind (_thicknessSlider)
                   .For (v => v.MinValue)
                   .To (vm => vm.MinThickness);

                set.Bind (_thicknessSlider)
                   .For (v => v.MaxValue)
                   .To (vm => vm.MaxThickness);

                set.Bind (_thicknessSlider)
                   .For (v => v.Value)
                   .To (vm => vm.PointThickness);

                set.Apply ();
            });
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
            var i = 0;
            var currentColor = Context.SelectedColor;
            foreach (var colorVm in Context.PaletteColors) {
                var colorView = new PaletteColorView ();
                colorView.DataContext = colorVm;
                if (colorVm == currentColor || (i == 0 && currentColor == null)) {
                    colorVm.IsSelected = true;
                }
                colorView.Frame = new CGRect (x, y, size, size);
                Add (colorView);
                x += size + XMargin;
                if (x >= Frame.Width - size - XMargin) break;
                i++;
            }
            _thicknessSlider.Frame = new CGRect (x, y, Frame.Width - XMargin - x, Frame.Height/2);
            Add (_thicknessSlider);
        }

        internal void UpdateUndo (bool canUndo)
        {
            _undoButton.SetTitleColor (canUndo ? UIColor.White : UIColor.LightGray, UIControlState.Normal);
        }
   }
}

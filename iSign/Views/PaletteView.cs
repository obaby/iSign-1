using System;
using CoreGraphics;
using iSign.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace iSign.Views
{
    public sealed class PaletteView : MvxView
    {
        private int Margin => UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? 2 : 10;
        private int XMargin => UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? 15 : 40;
        private UIButton UndoButton { get; }
        private UISlider ThicknessSlider { get; }
        public PaletteView ()
        {
            BackgroundColor = UIColor.Gray;
            UndoButton = new UIButton ();
            ThicknessSlider = new UISlider ();
            this.DelayBind (() => {
                var set = this.CreateBindingSet<PaletteView, PaletteViewModel> ();
                set.Bind (UndoButton)
                   .For ("Title")
                   .To (vm => vm.UndoText);
                set.Bind (UndoButton)
                   .To (vm => vm.UndoCommand);

                set.Bind (ThicknessSlider)
                   .For (v => v.MinValue)
                   .To (vm => vm.MinThickness);

                set.Bind (ThicknessSlider)
                   .For (v => v.MaxValue)
                   .To (vm => vm.MaxThickness);

                set.Bind (ThicknessSlider)
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

            UndoButton.Frame = new CGRect (x, 0, 10, 10);
           
            Add (UndoButton);
            UndoButton.SizeToFit ();
            x += UndoButton.Frame.Width + XMargin;
            var i = 0;
            var currentColor = Context.SelectedColor;
            foreach (var colorVm in Context.PaletteColors) {
                var colorView = new PaletteColorView {DataContext = colorVm};
                if (colorVm == currentColor || (i == 0 && currentColor == null))
                {
                    if (colorVm != null) colorVm.IsSelected = true;
                }
                colorView.Frame = new CGRect (x, y, size, size);
                Add (colorView);
                x += size + XMargin;
                if (x >= Frame.Width - size - XMargin) break;
                i++;
            }
            ThicknessSlider.Frame = new CGRect (x, y, Frame.Width - XMargin - x, size);
            Add (ThicknessSlider);
        }

        internal void UpdateUndo (bool canUndo)
        {
            UndoButton.SetTitleColor (canUndo ? UIColor.White : UIColor.LightGray, UIControlState.Normal);
        }
   }
}

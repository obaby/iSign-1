using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace iSign
{
    public partial class TouchableScrollView : UIScrollView
    {
        private enum Modes
        {
            Editing,
            AddingLabel,
            Done
        }
        private Modes Mode { get; set; }
        private string Text { get; set; }
        private List<EditableView> AddedViews { get; }
        private PaletteView PaletteView { get; }
        public TouchableScrollView (IntPtr handle) : base (handle)
        {
            Mode = Modes.Done;
            SetupGestures ();
            AddedViews = new List<EditableView> ();
            PaletteView = new PaletteView ();
        }
        private object _paletteContext;
        public object PaletteContext {
            get {
                return _paletteContext;
            }
            set {
                _paletteContext = value;
                PaletteView.DataContext = value;
            }
        }

        public override void LayoutSubviews ()
        {
            base.LayoutSubviews ();
            PanGestureRecognizer.MinimumNumberOfTouches = 2;
        }

        public event EventHandler FinishedAddingView;
        private void OnFinishedAddingView ()
        {
            FinishedAddingView?.Invoke (this, EventArgs.Empty);
        }

        private void SetupGestures ()
        {
            var tapGesture = new UITapGestureRecognizer (ScrollViewTouched);
            AddGestureRecognizer (tapGesture);
        }

        private void ScrollViewTouched (UITapGestureRecognizer tapInfo)
        {
            if (Mode == Modes.Done) return;
            var location = tapInfo.LocationInView (this);
            var view = new EditableView (new CGRect (location.X, location.Y, 100, 100));
            if (Mode == Modes.AddingLabel) {
                var label = new UILabel (new CGRect (0, 0, 100, 10)) {
                    Text = Text
                };
                view.Add (label);
                label.SizeToFit ();
                view.Frame = new CGRect (view.Frame.Location, label.Frame.Size);
            }
            AddedViews.Add (view);
            Add (view);
            ShowPalette ();
            Mode = Modes.Done;
            OnFinishedAddingView ();
        }

        public void SetToEditMode ()
        {
            Mode = Modes.Editing;
        }

        public void EndEditMode ()
        {
            Mode = Modes.Done;
        }

        public void AddLabel (string text)
        {
            Mode = Modes.AddingLabel;
            Text = text;
        }

        internal void Clear ()
        {
            foreach (var view in AddedViews) {
                view.RemoveFromSuperview ();
            }
            AddedViews.Clear ();
        }

        public void EndUpdate ()
        {
            foreach (var view in AddedViews) {
                view.EndUpdate ();
            }
        }

        private void ShowPalette ()
        {
            PaletteView.Frame = new CGRect (0, Frame.Height - 50, Frame.Width, 50);
            PaletteView.Layout ();
            Animate (0.5, 0.2, UIViewAnimationOptions.CurveLinear, () =>
                     Add (PaletteView), null);
        }
    }
}
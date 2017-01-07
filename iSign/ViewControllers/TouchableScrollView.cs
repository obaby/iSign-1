using Foundation;
using System;
using UIKit;
using CoreGraphics;

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
        private string Text { get; set;}

        public TouchableScrollView (IntPtr handle) : base (handle)
        {
            Mode = Modes.Done;
            SetupGestures ();
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
            var   view = new EditableView (new CGRect (location.X, location.Y, 100, 100));
            if (Mode == Modes.AddingLabel) {
                var label = new UILabel (new CGRect (0, 0, 100, 10)) {
                    Text = Text
                };
                view.Add (label);
                label.SizeToFit ();
            }
           
            Add (view);
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
    }
}
using System;
using CoreGraphics;
using iSign.Touch;
using UIKit;

namespace iSign
{
    public class SigningView : UIView
    {
        private CanvasView CanvasView { get; }
        private UIButton OkButton { get; }
        private UIButton CancelButton { get; }
        private UILabel HelpMessage { get; }

        public SigningView (CGRect bounds) : base (bounds)
        {
            Frame = bounds;
            CanvasView = new CanvasView (new CGRect(0, 0, 500, 500));
            BackgroundColor = UIColor.FromRGB (0, 153, 255).ColorWithAlpha(0.3f);
            OkButton = new UIButton ();
            OkButton.SetTitle ("Add Signature", UIControlState.Normal);
            OkButton.TouchUpInside += OkButton_TouchUpInside;

            CancelButton = new UIButton ();
            CancelButton.SetTitle ("Cancel", UIControlState.Normal);
            CancelButton.TouchUpInside += CancelButton_TouchUpInside;
        }



        public override void LayoutSubviews ()
        {
            base.LayoutSubviews ();
            ShowViewWithAnimation ();
        }

        void ShowViewWithAnimation ()
        {

            var topLeft = this.GetCenter (CanvasView.Frame);
            var topRight = new CGPoint (topLeft.X + CanvasView.Frame.Width, topLeft.Y);
            CanvasView.Frame = new CGRect (topLeft, CanvasView.Frame.Size);
            CanvasView.Layer.BorderWidth = 5;
            CanvasView.Layer.CornerRadius = 10;
            CanvasView.Transform = CGAffineTransform.Scale (CGAffineTransform.MakeIdentity (), 0.001f, 0.001f);
            Add (CanvasView);

            AnimateAsync (0.2, () => {
                CanvasView.Transform = CGAffineTransform.Scale (CGAffineTransform.MakeIdentity (), 1, 1);
            });

            CancelButton.SizeToFit ();
            CancelButton.Frame = new CGRect (new CGPoint (topLeft.X + 10, topLeft.Y - 40), CancelButton.Frame.Size);
            Add (CancelButton);
            Add (OkButton);
            OkButton.SizeToFit ();
            OkButton.Frame = new CGRect (new CGPoint (topRight.X - OkButton.Frame.Width - 10, topRight.Y - 40), OkButton.Frame.Size);

        }

        public UIImage GetSignature ()
        {
            return CanvasView?.DrawingImage;
        }

        public Action OkAction { get; set;}
        public Action CancelAction { get; set;}
        private void CloseView ()
        {
            RemoveFromSuperview ();
        }

        void OkButton_TouchUpInside (object sender, EventArgs e)
        {
            if (OkAction != null) {
                OkAction ();
            }
            CloseView ();
        }

        void CancelButton_TouchUpInside (object sender, EventArgs e)
        {
            if (CancelAction != null) {
                CancelAction ();
            }
            CloseView ();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;
using UIKit;

namespace iSign
{
    public class EditableView : UIView
    {
        public int Id { get; set; }
        public UIImageView ImageView { get; private set; }
        private UIPanGestureRecognizer DragGesture { get; }
        private UITapGestureRecognizer DoubleTapTwoFigersGesture { get; }
        private UITapGestureRecognizer DoubleTapGesture { get; }
        private UIPinchGestureRecognizer PinchGesture { get; }
        private UILongPressGestureRecognizer LongPressGesture { get; }
        private IMvxMessenger Messenger { get; }

        public EditableView (CGRect rect) : base(rect)
        {
            Messenger = Mvx.Resolve<IMvxMessenger> ();
            DragGesture = new UIPanGestureRecognizer (ViewDragged) {
                MinimumNumberOfTouches = 1
            };

            LongPressGesture = new UILongPressGestureRecognizer (ViewLongPressed);

            DoubleTapTwoFigersGesture = new UITapGestureRecognizer (ViewDoubleTappedWith2Fingers) {
                NumberOfTapsRequired = 2,
                NumberOfTouchesRequired = 2
            };

            PinchGesture = new UIPinchGestureRecognizer (ViewResized);

            DoubleTapGesture = new UITapGestureRecognizer (ViewDoubleTapped)
            {
                NumberOfTapsRequired = 2
            };

            AddGestureRecognizer (DoubleTapGesture);
            AddGestureRecognizer (DragGesture);
            AddGestureRecognizer (DoubleTapTwoFigersGesture);
            AddGestureRecognizer (LongPressGesture);
            AddGestureRecognizer (PinchGesture);
            BackgroundColor = UIColor.Clear;
            ViewStateFlow = new List<ViewState> { ViewState.Done, ViewState.Editing };

            UpdateLayer ();
        }

        private CGPoint _viewCoordinate;

        private void ViewDragged (UIPanGestureRecognizer panInfo)
        {
            if (panInfo.State == UIGestureRecognizerState.Began) {
                _viewCoordinate = panInfo.LocationInView (panInfo.View);
            }
            var newCoord = panInfo.LocationInView (panInfo.View);
            var deltaWidthDrag = newCoord.X - _viewCoordinate.X;
            var deltaHeightDrag = newCoord.Y - _viewCoordinate.Y;


            double x = Frame.X;
            double y = Frame.Y;
            var width = Frame.Size.Width;
            var height = Frame.Size.Height;

            var xMax = Superview.Frame.Width;
            var yMax = Superview.Frame.Height;
            if (Superview is UIScrollView) {
                xMax = ((UIScrollView)Superview).ContentSize.Width;
                yMax = ((UIScrollView)Superview).ContentSize.Height;
            }

            x = Math.Max (0, Math.Min (xMax - panInfo.View.Frame.Width, panInfo.View.Frame.X + deltaWidthDrag));
            y = Math.Max (0, Math.Min (yMax - panInfo.View.Frame.Height, panInfo.View.Frame.Y + deltaHeightDrag));

            panInfo.View.Frame = new CGRect (x, y,
                width,
                height);
        }

        private nfloat _previousScale = 1;
        private void ViewResized (UIPinchGestureRecognizer pinchInfo)
        {
            if (pinchInfo.State == UIGestureRecognizerState.Ended) {
                this.UnantMarch ();
                this.AntMarch (UIColor.Blue);
                return;
            }
            var scale = 1 - _previousScale + pinchInfo.Scale;
            _previousScale = pinchInfo.Scale;
            var size = new CGSize (Frame.Size.Width * scale, Frame.Size.Height * scale);
            var diffX = (Frame.Size.Width - size.Width) / 2;
            var diffY = (Frame.Size.Height - size.Height) / 2;
            var location = new CGPoint (Frame.X + diffX, Frame.Y + diffY);
            Frame = new CGRect (location, size);
            UpdateImageAndLayer (size);
        }


        private void ViewDoubleTapped (UITapGestureRecognizer tapInfo)
        {
            if (OnDoubleTap != null) {
                OnDoubleTap ();
            }
        }

        public Action OnDoubleTap { get; set;}

        private void ViewDoubleTappedWith2Fingers (UITapGestureRecognizer tapInfo)
        {
            Remove ();
        }

        public void Remove ()
        {
            RemoveFromSuperview ();
            RemoveGestureRecognizer (DragGesture);
            RemoveGestureRecognizer (DoubleTapGesture);
            RemoveGestureRecognizer (DoubleTapTwoFigersGesture);
            RemoveGestureRecognizer (LongPressGesture);
            RemoveGestureRecognizer (PinchGesture);
            Dispose ();
        }

        private void ViewLongPressed (UILongPressGestureRecognizer tapInfo)
        {
            if (tapInfo.State == UIGestureRecognizerState.Ended) {
                State = NextState ();
                UpdateLayer ();
            }
        }

        private ViewState NextState ()
        {
            var index = ViewStateFlow.IndexOf (State);
            var newIndex = (index + 1) % ViewStateFlow.Count;
            return ViewStateFlow [newIndex];
        }

        private List<ViewState> ViewStateFlow { get; }


        private void UpdateLayer ()
        {
            switch (State) {
            case ViewState.Done:
                this.UnantMarch ();
                ChangeGestureEnablity (false);
                break;
            case ViewState.Editing:
                this.UnantMarch ();
                this.AntMarch (UIColor.Blue);
                ChangeGestureEnablity (true);
                break;
            }
        }

        private void ChangeGestureEnablity (bool enabled)
        {
            DragGesture.Enabled = enabled;
            PinchGesture.Enabled = enabled;
        }

        private ViewState State { get; set; }
        private enum ViewState
        {
            Editing,
            Done
        }

        public void SetImage (UIImage image)
        {
            ImageView = new UIImageView (new CGRect (CGPoint.Empty, image.Size));
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            Frame = new CGRect (Frame.Location, image.Size);
            ImageView.Image = image;
            Add (ImageView);
        }

        public void UpdateImageAndLayer (CGSize size)
        {
            if (ImageView != null)
                ImageView.Frame = new CGRect (ImageView.Frame.Location, size);
            this.UpdateLayersFrame ();
        }
    }
}

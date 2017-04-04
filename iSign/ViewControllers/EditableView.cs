using System;
using System.Collections.Generic;
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
        private UILongPressGestureRecognizer LongPressGesture { get; }
        private IMvxMessenger Messenger { get; }
        private int _border = 50;
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

            DoubleTapGesture = new UITapGestureRecognizer (ViewDoubleTapped)
            {
                NumberOfTapsRequired = 2
            };

            CurrentTouch = TypeOfTouch.Nothing;
            AddGestureRecognizer (DoubleTapGesture);
            AddGestureRecognizer (DragGesture);
            AddGestureRecognizer (DoubleTapTwoFigersGesture);
            AddGestureRecognizer (LongPressGesture);
            BackgroundColor = UIColor.Clear;
            ViewStateFlow = new List<ViewState> { ViewState.Done, ViewState.Resizing, ViewState.Moving };

            State = ViewState.Resizing;
            UpdateLayer ();
        }

        private CGPoint _viewCoordinate;
        private CGPoint _formerCoordinate;

        private void ViewDragged (UIPanGestureRecognizer panInfo)
        {
            if (panInfo.State == UIGestureRecognizerState.Began) {
                _viewCoordinate = panInfo.LocationInView (panInfo.View);
                _formerCoordinate = _viewCoordinate;
                var location = panInfo.LocationOfTouch (0, this);
                CurrentTouch = DetermineTypeOfTouch (location);
            }
            if (panInfo.State == UIGestureRecognizerState.Ended) {
                CurrentTouch = TypeOfTouch.Nothing;
            }
            var newCoord = panInfo.LocationInView (panInfo.View);
            var deltaWidthDrag = newCoord.X - _viewCoordinate.X;
            var deltaHeightDrag = newCoord.Y - _viewCoordinate.Y;

            var deltaWidth = newCoord.X - _formerCoordinate.X;
            var deltaHeight = newCoord.Y - _formerCoordinate.Y;

            double x = Frame.X;
            double y = Frame.Y;
            var width = Frame.Size.Width;
            var height = Frame.Size.Height;
            nfloat ratio = 1;
            switch (CurrentTouch) {
            case TypeOfTouch.Dragging:
                var xMax = Superview.Frame.Width;
                var yMax = Superview.Frame.Height;
                if (Superview is UIScrollView) {
                    xMax = ((UIScrollView)Superview).ContentSize.Width;
                    yMax = ((UIScrollView)Superview).ContentSize.Height;
                }
                x = Math.Max (0, Math.Min (xMax - panInfo.View.Frame.Width, panInfo.View.Frame.X + deltaWidthDrag));
                y = Math.Max (0, Math.Min (yMax - panInfo.View.Frame.Height, panInfo.View.Frame.Y + deltaHeightDrag));
                break;
            case TypeOfTouch.ResizingTopBorder:
                y = panInfo.View.Frame.Y + deltaHeightDrag;
                height = panInfo.View.Frame.Height - deltaHeightDrag;
                ratio = panInfo.View.Frame.Height / height;
                width = panInfo.View.Frame.Width / ratio;
                break;
            case TypeOfTouch.ResizingBottomBorder:
                height = panInfo.View.Frame.Height + deltaHeight;
                ratio = panInfo.View.Frame.Height / height;
                width = panInfo.View.Frame.Width / ratio;
                break;
            case TypeOfTouch.ResizingLeftBorder:
                x = panInfo.View.Frame.X + deltaWidthDrag;
                width = panInfo.View.Frame.Width - deltaWidthDrag;
                ratio = panInfo.View.Frame.Width / width;
                height = panInfo.View.Frame.Height / ratio;
                break;
            case TypeOfTouch.ResizingRightBorder:
                width = panInfo.View.Frame.Width + deltaWidth;
                ratio = panInfo.View.Frame.Width / width;
                height = panInfo.View.Frame.Height / ratio;
                break;
            default: return;
            }
            _formerCoordinate = newCoord;
            panInfo.View.Frame = new CGRect (x, y,
                width,
                height);
            UpdateImageAndLayer (new CGSize (width, height));
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

        public void EndUpdate ()
        {
            State = ViewState.Done;
            UpdateLayer ();
        }

        private void UpdateLayer ()
        {
            switch (State) {
            case ViewState.Done:
                this.UnantMarch ();
                this.Unblink ();
                DragGesture.Enabled = false;
                break;
            case ViewState.Moving:
                this.UnantMarch ();
                this.Unblink ();
                this.AntMarch (UIColor.Blue);
                DragGesture.Enabled = true;
                break;
            case ViewState.Resizing:
                this.UnantMarch ();
                this.Unblink ();
                this.Blink (UIColor.Red);
                DragGesture.Enabled = true;
                break;
            }
        }

        private TypeOfTouch CurrentTouch { get; set; }
        private enum TypeOfTouch
        {
            Nothing,
            Dragging,
            ResizingTopBorder,
            ResizingRightBorder,
            ResizingBottomBorder,
            ResizingLeftBorder,
        }

        private TypeOfTouch DetermineTypeOfTouch (CGPoint coordinate)
        {
            if (State == ViewState.Resizing) {
                if (coordinate.Y < _border) return TypeOfTouch.ResizingTopBorder;
                if (coordinate.Y > Frame.Height - _border) return TypeOfTouch.ResizingBottomBorder;
                if (coordinate.X < _border) return TypeOfTouch.ResizingLeftBorder;
                if (coordinate.X > Frame.Width - _border) return TypeOfTouch.ResizingRightBorder;
            }
            if (State == ViewState.Moving) {
                return TypeOfTouch.Dragging;
            }
            return TypeOfTouch.Nothing;
        }


        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
            if (State != ViewState.Resizing) return;
            base.TouchesMoved (touches, evt);
        }

        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            if (State != ViewState.Resizing) return;
            base.TouchesEnded (touches, evt);
            
        }

        public override void TouchesCancelled (NSSet touches, UIEvent evt)
        {
            if (State != ViewState.Resizing) return;
            base.TouchesCancelled (touches, evt);
        }
      
        private ViewState State { get; set; }
        private enum ViewState
        {
            Moving,
            Resizing,
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

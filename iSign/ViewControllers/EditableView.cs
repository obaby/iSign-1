using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using iSign.Touch;
using UIKit;
using static iSign.Helpers.CGRectHelpers;

namespace iSign
{
    public class EditableView : CanvasView
    {
        private UIPanGestureRecognizer DragGesture { get; }
        private UITapGestureRecognizer DoubleTapGesture { get; }
        private UILongPressGestureRecognizer LongPressGesture { get; }
        private int _border = 20;
        public EditableView (CGRect rect) : base(rect)
        {
            DragGesture = new UIPanGestureRecognizer (ViewDragged) {
                MinimumNumberOfTouches = 1
            };

            LongPressGesture = new UILongPressGestureRecognizer (ViewLongPressed);

            DoubleTapGesture = new UITapGestureRecognizer (ViewDoubleTapped) {
                NumberOfTapsRequired = 2,
                NumberOfTouchesRequired = 2
            };
            CurrentTouch = TypeOfTouch.Nothing;
            AddGestureRecognizer (DragGesture);
            AddGestureRecognizer (DoubleTapGesture);
            AddGestureRecognizer (LongPressGesture);
            Layer.BorderWidth = 2;
            BackgroundColor = UIColor.Clear;
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
                break;
            case TypeOfTouch.ResizingBottomBorder:
                height = panInfo.View.Frame.Height + deltaHeight;
                break;
            case TypeOfTouch.ResizingLeftBorder:
                x = panInfo.View.Frame.X + deltaWidthDrag;
                width = panInfo.View.Frame.Width - deltaWidthDrag;
                break;
            case TypeOfTouch.ResizingRightBorder:
                width = panInfo.View.Frame.Width + deltaWidth;
                break;
            default: return;
            }
            _formerCoordinate = newCoord;
            panInfo.View.Frame = new CGRect (x, y,
                width,
                height);
        }

        private void ViewDoubleTapped (UITapGestureRecognizer tapInfo)
        {
            tapInfo.View.RemoveFromSuperview ();
            RemoveGestureRecognizer (DragGesture);
            RemoveGestureRecognizer (DoubleTapGesture);
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
            switch (State) {
            case ViewState.Done:
                return ViewState.Moving;
            case ViewState.Moving:
                return ViewState.Editing;
            case ViewState.Editing:
                return ViewState.Done;
            default: 
                return ViewState.Done;
            }
        }

        public void EndUpdate ()
        {
            State = ViewState.Done;
            UpdateLayer ();
        }

        private void UpdateLayer ()
        {
            switch (State) {
            case ViewState.Done:
                Layer.BorderWidth = 0;
                DragGesture.Enabled = false;
                break;
            case ViewState.Editing:
                Layer.BorderWidth = 2;
                Layer.BorderColor = UIColor.Red.CGColor;
                DragGesture.Enabled = false;
                break;
            case ViewState.Moving:
                Layer.BorderWidth = 2;
                Layer.BorderColor = UIColor.Black.CGColor;
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
            if (coordinate.Y < _border) return TypeOfTouch.ResizingTopBorder;
            if (coordinate.Y > Frame.Height - _border) return TypeOfTouch.ResizingBottomBorder;
            if (coordinate.X < _border) return TypeOfTouch.ResizingLeftBorder;
            if (coordinate.X > Frame.Width - _border) return TypeOfTouch.ResizingRightBorder;
            return TypeOfTouch.Dragging;
        }


        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
            if (State != ViewState.Editing) return;
            base.TouchesMoved (touches, evt);
        }

        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            if (State != ViewState.Editing) return;
            base.TouchesEnded (touches, evt);
            
        }

        public override void TouchesCancelled (NSSet touches, UIEvent evt)
        {
            if (State != ViewState.Editing) return;
            base.TouchesCancelled (touches, evt);
        }
      
        private ViewState State { get; set; }
        private enum ViewState
        {
            Moving,
            Editing,
            Done
        }
    }
}

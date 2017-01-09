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
        private bool _isSigning;
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
            Layer.BorderWidth = 1;
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
                UpdateLayer ();
            }
        }

        public void EndUpdate ()
        {
            _isSigning = false;
            UpdateLayer ();
        }

        private void UpdateLayer ()
        {
            _isSigning = !_isSigning;
            if (_isSigning) {
                Layer.BorderWidth = 0;
                DragGesture.Enabled = false;
            } else {
                Layer.BorderWidth = 1;
                DragGesture.Enabled = true;
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
            if (coordinate.Y > this.Frame.Height - _border) return TypeOfTouch.ResizingBottomBorder;
            if (coordinate.X < _border) return TypeOfTouch.ResizingLeftBorder;
            if (coordinate.X > Frame.Width - _border) return TypeOfTouch.ResizingRightBorder;
            return TypeOfTouch.Dragging;
        }


        bool visualizeAzimuth;

        ReticleView reticleView;
        ReticleView ReticleView {
            get {
                reticleView = reticleView ?? new ReticleView (CGRectNull ()) {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Hidden = true
                };

                return reticleView;
            }
        }

        public override void TouchesBegan (NSSet touches, UIEvent evt)
        {
            if (!_isSigning) return;
            DrawTouches (touches, evt);

            if (visualizeAzimuth) {
                foreach (var touch in touches.Cast<UITouch> ()) {
                    if (touch.Type != UITouchType.Stylus)
                        continue;

                    ReticleView.Hidden = false;
                    UpdateReticleView (touch);
                }
            }
        }

        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
            if (!_isSigning) return;
            DrawTouches (touches, evt);

            if (visualizeAzimuth) {
                foreach (var touch in touches.Cast<UITouch> ()) {
                    if (touch.Type != UITouchType.Stylus)
                        continue;

                    UpdateReticleView (touch);

                    UITouch [] predictedTouches = evt?.GetPredictedTouches (touch);
                    UITouch predictedTouch = predictedTouches?.LastOrDefault ();

                    if (predictedTouch != null)
                        UpdateReticleView (predictedTouch, true);
                }
            }
        }

        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            if (!_isSigning) return;
            DrawTouches (touches, evt);
            EndTouches (touches, false);

            if (visualizeAzimuth) {
                foreach (var touch in touches.Cast<UITouch> ()) {
                    ReticleView.Hidden |= touch.Type == UITouchType.Stylus;
                }
            }
        }

        public override void TouchesCancelled (NSSet touches, UIEvent evt)
        {
            if (!_isSigning) return;
            if (touches == null)
                return;

            EndTouches (touches, true);
            if (visualizeAzimuth) {
                foreach (var touch in touches.Cast<UITouch> ())
                    ReticleView.Hidden |= touch.Type == UITouchType.Stylus;
            }
        }

        public override void TouchesEstimatedPropertiesUpdated (NSSet touches)
        {
            UpdateEstimatedPropertiesForTouches (touches);
        }

        public void ClearView ()
        {
            Clear ();
        }

        public void ToggleDebugDrawing (UIButton sender)
        {
            IsDebuggingEnabled = !IsDebuggingEnabled;
            visualizeAzimuth = !visualizeAzimuth;
            sender.Selected = IsDebuggingEnabled;
        }

        public void ToggleUsePreciseLocations (UIButton sender)
        {
            UsePreciseLocations = !UsePreciseLocations;
            sender.Selected = UsePreciseLocations;
        }

        void UpdateReticleView (UITouch touch, bool predicated = false)
        {
            if (!_isSigning) return;
            if (touch == null || touch.Type != UITouchType.Stylus)
                return;

            ReticleView.PredictedDotLayer.Hidden = !predicated;
            ReticleView.PredictedLineLayer.Hidden = !predicated;

            var azimuthAngle = touch.GetAzimuthAngle (this);
            var azimuthUnitVector = touch.GetAzimuthUnitVector (this);
            var altitudeAngle = touch.AltitudeAngle;

            if (predicated) {
                ReticleView.PredictedAzimuthAngle = azimuthAngle;
                ReticleView.PredictedAzimuthUnitVector = azimuthUnitVector;
                ReticleView.PredictedAltitudeAngle = altitudeAngle;
            } else {
                var location = touch.PreviousLocationInView (this);
                ReticleView.Center = location;
                ReticleView.ActualAzimuthAngle = azimuthAngle;
                ReticleView.ActualAzimuthUnitVector = azimuthUnitVector;
                ReticleView.ActualAltitudeAngle = altitudeAngle;
            }
        }
    }
}

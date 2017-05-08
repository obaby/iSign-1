using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using UIKit;

namespace iSign.Views
{
    public sealed class CanvasView : UIView
    {
        // ReSharper disable once InconsistentNaming
        private const double π = Math.PI;
        private const double DefaultLineWidth = 2;
        public double ForceSensitivity { get; set; } = 2;
        private const double TiltThreshold = π / 6; // 30°
        private const double MinLineWidth = 1;
        public double HandLineWidth { get; set; } = 2;
        private List<UIImage> PreviousImages { get; set; }
        public UIImage DrawingImage { get; private set; }
        UIColor drawColor;
        UIImage _image;

        private UIImage Image {
            get {
                return _image;
            }

            set {
                _image = value;
                ImageView.Image = _image;
            }
        }

        private UIImageView ImageView { get; set; }
        public UIColor DrawColor {
            get {
                return drawColor;
            }

            set {
                drawColor = value;
            }
        }

        public CanvasView (CGRect rect)
        {
            UserInteractionEnabled = true;
            BackgroundColor = UIColor.White;
            Frame = rect;
            PreviousImages = new List<UIImage> ();
            ImageView = new UIImageView ();
            ImageView.Frame = Frame;
            AddSubview (ImageView);
        }

        public override void TouchesMoved (Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesMoved (touches, evt);
            var touch = touches.First () as UITouch;
            if (touch == null) return;
            var bounds = Bounds.Ceiling ();
            UIGraphics.BeginImageContextWithOptions (bounds.Size, false, 0);
            var context = UIGraphics.GetCurrentContext ();
            DrawingImage?.Draw (bounds);

            var allTouches = new List<UITouch> ();
            var coalescedTouches = evt.GetCoalescedTouches (touch);
            if (coalescedTouches != null) {
                allTouches = coalescedTouches.ToList ();
            } else {
                allTouches.Append (touch);
            }

            foreach (var unused in touches) {
                DrawStroke (context, touch);
            }

            // 1
            DrawingImage = UIGraphics.GetImageFromCurrentImageContext ();
            var predictedTouches = evt.GetPredictedTouches (touch);
            foreach (var t in predictedTouches) {
                DrawStroke (context, t);
            }
            Image = UIGraphics.GetImageFromCurrentImageContext ();

            // Update image
            UIGraphics.EndImageContext ();
        }

        public bool CanUndo => PreviousImages.Count > 0;

        public void Undo ()
        {
            if (PreviousImages.Count <= 1) {
                Image = new UIImage ();
                DrawingImage = new UIImage ();
                PreviousImages = new List<UIImage> ();
                return;
            }
            PreviousImages.RemoveAt (PreviousImages.Count - 1);
            Image = PreviousImages [PreviousImages.Count - 1];
            DrawingImage = Image;
        }

        public override void TouchesEnded (Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);
            Image = DrawingImage;
            PreviousImages.Add (Image);
            LineAdded ();
        }

        public void StartWith (UIImage image)
        {
            Image = image;
            DrawingImage = image;
            PreviousImages.Add (image);
        }

        public event EventHandler OnLineAdded;
        private void LineAdded ()
        {
            OnLineAdded?.Invoke (this, EventArgs.Empty);
        }

        public override void TouchesCancelled (Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled (touches, evt);
            Image = DrawingImage;
        }

        private void DrawStroke (CGContext context, UITouch touch)
        {
            var previousLocation = touch.PreviousLocationInView (this);
            var location = touch.LocationInView (this);
            nfloat lineWidth;
            if (touch.Type == UITouchType.Stylus) {
                lineWidth = touch.AltitudeAngle < TiltThreshold ? LineWidthForShading (context, touch) : LineWidthForDrawing (touch);
                DrawColor.SetStroke ();
            } else {
                lineWidth = (nfloat)HandLineWidth;
                DrawColor.SetStroke ();
            }

            context.SetLineWidth (lineWidth);
            context.SetLineCap (CGLineCap.Round);

            context.MoveTo (previousLocation.X, previousLocation.Y);
            context.AddLineToPoint (location.X, location.Y);
            context.StrokePath ();
        }


        private nfloat LineWidthForShading (CGContext context, UITouch touch)
        {
            var previousLocation = touch.PreviousLocationInView (this);
            var location = touch.LocationInView (this);

            var vector1 = touch.GetAzimuthUnitVector (this);
            var vector2 = new CGPoint (location.X - previousLocation.X, location.Y - previousLocation.Y);

            var angle = Math.Abs (Math.Atan2 (vector2.Y, vector2.X) - Math.Atan2 (vector1.dy, vector1.dx));
            angle = angle > π ? 2 * π - angle : π - angle;

            var minAngle = 0;
            var maxAngle = π / 2;
            var normalizedAngle = (angle - minAngle) / (maxAngle - minAngle);

            var maxLineWidth = 60;
            var lineWidth = maxLineWidth * normalizedAngle;

            var minAltitudeAngle = 0.25;
            var maxAltitudeAngle = TiltThreshold;

            var altitudeAngle = Math.Min (minAltitudeAngle, touch.AltitudeAngle);

            var normalizedAltitude = 1 - ((altitudeAngle - minAltitudeAngle) / (maxAltitudeAngle - minAltitudeAngle));
            lineWidth = lineWidth * normalizedAltitude + MinLineWidth;

            var minForce = 0;
            var maxForce = 5;

            var normalizedAlpha = (touch.Force - minForce) / (maxForce - minForce);
            context.SetAlpha (normalizedAlpha);

            return (nfloat)lineWidth;
        }

        private nfloat LineWidthForDrawing (UITouch touch)
        {
            var lineWidth = DefaultLineWidth;
            if (touch.Force > 0) {
                lineWidth = touch.Force * ForceSensitivity;
            }

            return (nfloat)lineWidth;
        }

        public void ClearCanvas (bool animated)
        {
            if (animated) {
                Animate (0.5, () => Alpha = 0, () => {
                    Alpha = 1;
                    Image = null;
                    DrawingImage = null;
                }
                        );
            } else {
                Image = null;
                DrawingImage = null;
            }
        }

        public override void Draw (CGRect rect)
        {
            base.Draw (rect);
            using (var context = UIGraphics.GetCurrentContext ()) {

                //set up drawing attributes
                context.SetLineWidth (2);

                context.SetStrokeColor (UIColor.Black.CGColor);
                //create geometry
                var path = new CGPath ();

                path.AddLines (new CGPoint []{
                    new CGPoint (0, Frame.Height / 2 ),
                    new CGPoint (Frame.Width, Frame.Height / 2)});

                context.AddPath (path);
                context.StrokePath ();
            }
        }
    }
}
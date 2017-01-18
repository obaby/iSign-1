using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace iSign.Touch
{
    public class CanvasView : MvxImageView
    {

        private const double π = Math.PI;
        private const double DefaultLineWidth = 1;
        private const double ForceSensitivity = 1;
        private const double TiltThreshold = π / 6; // 30°
        private const double MinLineWidth = 1;
        private const double HandLineWidth = 1;
        private List<UIImage> PreviousImages { get; set; }
        private UIImage DrawingImage;
        public UIColor DrawColor { get; set;}
        private UIColor EraserColor => BackgroundColor ?? UIColor.White;

        public CanvasView (CGRect rect)
        {
            UserInteractionEnabled = true;
            Frame = rect;
            DrawColor = UIColor.Red;
            PreviousImages = new List<UIImage> ();
        }
       
        public override void TouchesMoved (Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesMoved (touches, evt);
            var touch = touches.First () as UITouch;
            if (touch == null) return;

            UIGraphics.BeginImageContextWithOptions (Bounds.Size, false, 0);
            var context = UIGraphics.GetCurrentContext ();
            if (DrawingImage != null) {
                DrawingImage.Draw (Bounds);
            }

            var allTouches = new List<UITouch> ();
            var coalescedTouches = evt.GetCoalescedTouches (touch);
            if (coalescedTouches != null) {
                allTouches = coalescedTouches.ToList ();
            } else {
                allTouches.Append (touch);
            }

            foreach (var t in touches) {
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
                return; }
            PreviousImages.RemoveAt (PreviousImages.Count - 1);
            Image = PreviousImages [PreviousImages.Count - 1];
            DrawingImage = Image;
        }

        public override void TouchesEnded (Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);
            Image = DrawingImage;
            PreviousImages.Add (Image);
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
            nfloat lineWidth = 0;
            if (touch.Type == UITouchType.Stylus) {
                lineWidth = touch.AltitudeAngle < TiltThreshold ? LineWidthForShading (context, touch) : LineWidthForDrawing (context, touch);
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

        private nfloat LineWidthForDrawing (CGContext context, UITouch touch)
        {
            var lineWidth = DefaultLineWidth;
            if (touch.Force > 0) {
                lineWidth = touch.Force * ForceSensitivity;
            }

            return (nfloat) lineWidth;
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
    }
}
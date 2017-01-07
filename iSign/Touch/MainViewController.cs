using System;
using System.Linq;

using UIKit;
using Foundation;

using CoreGraphics;
using static iSign.Helpers.CGRectHelpers;

namespace iSign.Touch {
	public partial class MainViewController : UIViewController {

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

		public CanvasView CanvasView {
			get {
				return (CanvasView)View;
			}
		}

		[Export ("initWithCoder:")]
		public MainViewController (NSCoder coder) : base (coder)
		{
		}

		public MainViewController (IntPtr handle) : base (handle)
		{
		}

        private void Setup ()
        {
            var image = ResizeImage (new UIImage ("Background/FastFlex.jpg"), View.Frame.Width, View.Frame.Height);

            UIGraphics.BeginImageContext (View.Frame.Size);
            image.DrawAsPatternInRect (View.Frame);
            var bg = UIGraphics.GetImageFromCurrentImageContext ();
            UIGraphics.EndImageContext ();
            View.BackgroundColor = UIColor.FromPatternImage (bg);
        }

        private UIImage ResizeImage (UIImage sourceImage, nfloat width, nfloat height)
        {
            UIGraphics.BeginImageContext (new CGSize (width, height));
            sourceImage.Draw (new CGRect (0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext ();
            UIGraphics.EndImageContext ();
            return resultImage;
        }
		public override void ViewDidLoad ()
		{
            Setup ();
			CanvasView.AddSubview (ReticleView);
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			CanvasView.DrawTouches (touches, evt);

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
			CanvasView.DrawTouches (touches, evt);

			if (visualizeAzimuth) {
				foreach (var touch in touches.Cast<UITouch> ()) {
					if (touch.Type != UITouchType.Stylus)
						continue;

					UpdateReticleView (touch);

					UITouch[] predictedTouches = evt?.GetPredictedTouches (touch);
					UITouch predictedTouch = predictedTouches?.LastOrDefault ();

					if (predictedTouch != null)
						UpdateReticleView (predictedTouch, true);
				}
			}
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			CanvasView.DrawTouches (touches, evt);
			CanvasView.EndTouches (touches, false);

			if (visualizeAzimuth) {
				foreach (var touch in touches.Cast<UITouch> ()) {
					ReticleView.Hidden |= touch.Type == UITouchType.Stylus;
				}
			}
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			if (touches == null)
				return;

			CanvasView.EndTouches (touches, true);
			if (visualizeAzimuth) {
				foreach (var touch in touches.Cast<UITouch> ())
					ReticleView.Hidden |= touch.Type == UITouchType.Stylus;
			}
		}

		public override void TouchesEstimatedPropertiesUpdated (NSSet touches)
		{
			CanvasView.UpdateEstimatedPropertiesForTouches (touches);
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.LandscapeRight | UIInterfaceOrientationMask.LandscapeLeft;
		}

		partial void ClearView (UIBarButtonItem sender)
		{
			CanvasView.Clear ();
		}

		partial void ToggleDebugDrawing (UIButton sender)
		{
			CanvasView.IsDebuggingEnabled = !CanvasView.IsDebuggingEnabled;
			visualizeAzimuth = !visualizeAzimuth;
			sender.Selected = CanvasView.IsDebuggingEnabled;
		}

		partial void ToggleUsePreciseLocations (UIButton sender)
		{
			CanvasView.UsePreciseLocations = !CanvasView.UsePreciseLocations;
			sender.Selected = CanvasView.UsePreciseLocations;
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		void UpdateReticleView (UITouch touch, bool predicated = false)
		{
			if (touch == null || touch.Type != UITouchType.Stylus)
				return;

			ReticleView.PredictedDotLayer.Hidden = !predicated;
			ReticleView.PredictedLineLayer.Hidden = !predicated;

			var azimuthAngle = touch.GetAzimuthAngle (View);
			var azimuthUnitVector = touch.GetAzimuthUnitVector (View);
			var altitudeAngle = touch.AltitudeAngle;

			if (predicated) {
				ReticleView.PredictedAzimuthAngle = azimuthAngle;
				ReticleView.PredictedAzimuthUnitVector = azimuthUnitVector;
				ReticleView.PredictedAltitudeAngle = altitudeAngle;
			} else {
				var location = touch.PreviousLocationInView (View);
				ReticleView.Center = location;
				ReticleView.ActualAzimuthAngle = azimuthAngle;
				ReticleView.ActualAzimuthUnitVector = azimuthUnitVector;
				ReticleView.ActualAltitudeAngle = altitudeAngle;
			}
		}

        partial void UIButton41_TouchUpInside (UIButton sender)
        {
            /*var pdfGenerator = new PDFGenerator (View);
            pdfGenerator.Generate ("fast_flex.pdf");
            var vc = new PDFViewController ();
            ShowViewController (vc, this);
            vc.LoadFile (pdfGenerator.FilePath);*/
        }
	}
}

using System;
using System.IO;
using System.Threading.Tasks;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace iSign
{
    public class PdfToImage
    {
        private UIButton AcceptBtn { get; }
        private UIButton CancelBtn { get; }
        public PdfToImage ()
        {
            AcceptBtn = new UIButton (new CGRect (10, -50, 100, 50));
            AcceptBtn.SetTitle ("Select", UIControlState.Normal);
            AcceptBtn.BackgroundColor = UIColor.FromHSBA (204, 100, 36, 0.5f);
            AcceptBtn.TouchUpInside += AcceptBtn_TouchUpInside;
            CancelBtn = new UIButton (new CGRect (10, -50, 100, 50));
            CancelBtn.SetTitle ("Cancel", UIControlState.Normal);
            CancelBtn.TouchUpInside += CancelBtn_TouchUpInside;
            CancelBtn.BackgroundColor = UIColor.FromHSBA (204, 100, 36, 0.8f);
        }

        private UIWebView Webview { get; set;}
        public event EventHandler<UIImage> ImageCreated;
        public void Load (string pdfFile)
        {
            var vc = ((UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController).VisibleViewController;
            Webview = new UIWebView (vc.View.Frame);
            var localDocUrl = Path.Combine (NSBundle.MainBundle.BundlePath, pdfFile);

            vc.Add (Webview);
            vc.Add (AcceptBtn);
            vc.Add (CancelBtn);
            UIView.Animate (1, 0.5, UIViewAnimationOptions.CurveEaseIn,
                () => {
                    AcceptBtn.Frame = new CGRect (13, 60, 100, 50);
                    CancelBtn.Frame = new CGRect (13, 120, 100, 50);
            },() => { }
            );
            Webview.Hidden = false;

            Webview.LoadRequest (new NSUrlRequest (new NSUrl (localDocUrl, false)));
        }

        public UIImage LoadImageFromPDFFile (string pdfFile, UIView view)
        {
            UIGraphics.BeginImageContext (new CGSize (1024, 1426));

            var context = UIGraphics.GetCurrentContext ();

            //var pdfURL = CFBundle.GetResourceUrl (NSBundle.MainBundle.BundleUrl, new CFString (pdfFile), null, null);
            var localDocUrl = Path.Combine (NSBundle.MainBundle.BundlePath, pdfFile);

            var pdf = new CGPDFDocument (new CGDataProvider (localDocUrl));
            var page = pdf.GetPage (1);
            var pageRect = page.GetBoxRect (CGPDFBox.Media);

            context.TranslateCTM (0.0f, view.Frame.Height + 49);
            var x = pageRect.Size.Width / view.Frame.Width;
            var y = pageRect.Size.Height / view.Frame.Height;
            context.ScaleCTM (x, -y);


            context.SaveState ();
            //var pdfTransform = page.GetDrawingTransform (CGPDFBox.Crop, new CGRect (0, 0, 1024, 1426), 0, true);
            //context.ConcatCTM (pdfTransform);
            context.DrawPDFPage (page);
            //context.RestoreState ();
            var resultImage = UIGraphics.GetImageFromCurrentImageContext ();
            UIGraphics.EndImageContext ();
            return resultImage;
        }

        public UIImage DrawPdrFromUrl (string pdfPath)
        {
            var localDocUrl = Path.Combine (NSBundle.MainBundle.BundlePath, pdfPath);

            var document = new CGPDFDocument (new CGDataProvider (localDocUrl));
            var page = document.GetPage (1);

            var pageRect = page.GetBoxRect (CGPDFBox.Media);
            var renderer = new UIGraphicsImageRenderer (pageRect.Size);
            var img = renderer.CreateImage (ctx => {
                UIColor.White.SetFill ();
                ctx.FillRect (pageRect);

                ctx.CGContext.TranslateCTM (0, pageRect.Size.Height);
                ctx.CGContext.ScaleCTM (1, -1);
                ctx.CGContext.DrawPDFPage (page);

            });
            return img;
        }

        private void OnImageCreated (UIImage image)
        {
            ImageCreated?.Invoke (this, image);
        }

        void AcceptBtn_TouchUpInside (object sender, EventArgs e)
        {
            var contentSize = Webview.SizeThatFits(CGSize.Empty);
            UIGraphics.BeginImageContext (contentSize);
            Webview.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
            var image = UIGraphics.GetImageFromCurrentImageContext ();
            UIGraphics.EndImageContext ();
            OnImageCreated (image);
            RemoveViews ();
        }

        void CancelBtn_TouchUpInside (object sender, EventArgs e)
        {
            RemoveViews ();
        }

        private void RemoveViews ()
        {
            Webview.RemoveFromSuperview ();
            AcceptBtn.RemoveFromSuperview ();
            CancelBtn.RemoveFromSuperview ();
        }
    }
}

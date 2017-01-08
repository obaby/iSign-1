using System;
using System.IO;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;

namespace iSign
{
    public class PdfToImage
    {
        private UIWebView Webview { get; set;}
        public event EventHandler<UIImage> ImageCreated;
        public void Load (string pdfFile)
        {
            var vc = ((UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController).VisibleViewController;
            Webview = new UIWebView (vc.View.Frame);
            var captureBtn = new UIButton (new CGRect (0, 50, 100, 50));
            captureBtn.SetTitle ("Capture", UIControlState.Normal);
            captureBtn.TouchUpInside+=CaptureBtn_TouchUpInside;
                                           
            Webview.LoadFinished += Webview_LoadFinished;
            var localDocUrl = Path.Combine (NSBundle.MainBundle.BundlePath, pdfFile);

            vc.Add (Webview);
            //vc.Add (captureBtn);
            Webview.Hidden = false;

            Webview.LoadRequest (new NSUrlRequest (new NSUrl (localDocUrl, false)));
        }

        async void Webview_LoadFinished (object sender, EventArgs e)
        {
            await Task.Delay (1000);
            var webView = sender as UIWebView;
            var tmpFrame = webView.Frame;

            // set new Frame
            var aFrame = webView.Frame;
            aFrame.Size = webView.SizeThatFits (UIScreen.MainScreen.Bounds.Size);
            webView.Frame = aFrame;

            UIGraphics.BeginImageContext (webView.Frame.Size);
            webView.Layer.RenderInContext (UIGraphics.GetCurrentContext());
            var image = UIGraphics.GetImageFromCurrentImageContext ();
            UIGraphics.EndImageContext ();
            OnImageCreated (image);
            webView.RemoveFromSuperview ();
        }

        private void OnImageCreated (UIImage image)
        {
            ImageCreated?.Invoke (this, image);
        }

        void CaptureBtn_TouchUpInside (object sender, EventArgs e)
        {
            var contentSize = Webview.Frame.Size;
            UIGraphics.BeginImageContext (contentSize);
            Webview.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
            var image = UIGraphics.GetImageFromCurrentImageContext ();
            UIGraphics.EndImageContext ();
            OnImageCreated (image);
            Webview.RemoveFromSuperview ();
            ((UIButton)sender).RemoveFromSuperview ();
        }
    }
}

using System;
using System.IO;
using CoreGraphics;
using Foundation;
using UIKit;

namespace iSign.Helpers
{
    public static class PDFToImage
    {
        public static UIImage Convert (string pdfPath)
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
    }

    public static class ViewToPDF
    {
        public static string Convert (UIScrollView view, string filename)
        {
            var pdfData = new NSMutableData ();
            UIGraphics.BeginPDFContext (pdfData, new CGRect (new CGPoint(0, 0), view.ContentSize), new NSDictionary ());
            UIGraphics.BeginPDFPage ();
            var context = UIGraphics.GetCurrentContext ();
            var originFrame = view.Frame;
            view.Frame = new CGRect (new CGPoint (0, 0), view.ContentSize);
            view.ContentScaleFactor = 2;
            foreach (var subview in view.Subviews) {
                subview.ContentScaleFactor = 2;}
            view.DrawViewHierarchy (view.Frame, true);
            //view.Layer.RenderInContext (context);
            view.Frame = originFrame;
            var filePath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), filename);
            UIGraphics.EndPDFContent ();
            pdfData.Save (filePath, true);
            return filePath;
        }
    }
}
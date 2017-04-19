using System;
using System.IO;
using CoreGraphics;
using Foundation;
using UIKit;

namespace iSign.Helpers
{
    public static class PDFToImage
    {
        public static UIImage Convert (string pdfPath, bool directLink)
        {
            var localDocUrl = directLink ? pdfPath : Path.Combine (NSBundle.MainBundle.BundlePath, pdfPath);

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

    public static class ViewConverter
    {
        public static string ToPDF (this UIScrollView view, string filename)
        {
            var image = view.ToUIImage ();
            var bounds = new CGRect (0, 0, view.ContentSize.Width, view.ContentSize.Height);
            var r = new UIGraphicsPdfRenderer (bounds, UIGraphicsPdfRendererFormat.DefaultFormat);
            NSData pdf = r.CreatePdf ((UIGraphicsPdfRendererContext ctx) => {
                ctx.BeginPage ();
                image.Draw (new CGPoint (0, 0));
            });
            var original = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "DL", filename);
            File.Delete (original);
            var filePath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), filename);
            pdf.Save (filePath, true);
            return filePath;
        }

        public static UIImage ToUIImage (this UIScrollView view)
        {
            UIGraphics.BeginImageContext (view.ContentSize);
            var context = UIGraphics.GetCurrentContext ();
            var originFrame = view.Frame;
            view.Frame = new CGRect (new CGPoint (0, 0), view.ContentSize);
            view.Layer.RenderInContext (context);
            view.Frame = originFrame;
            var image = UIGraphics.GetImageFromCurrentImageContext ();
            UIGraphics.EndImageContext ();
            return image;
        }

        public static UIImage ToUIImage (this UIView view)
        {
            UIGraphics.BeginImageContext (view.Frame.Size);
            var context = UIGraphics.GetCurrentContext ();
            var originFrame = view.Frame;
            view.Frame = new CGRect (new CGPoint (0, 0), view.Frame.Size);
            view.Layer.RenderInContext (context);
            view.Frame = originFrame;
            var image = UIGraphics.GetImageFromCurrentImageContext ();
            UIGraphics.EndImageContext ();
            return image;
        }
    }
}
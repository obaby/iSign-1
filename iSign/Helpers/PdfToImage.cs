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

    public  class ViewToPDF
    {
        private CGRect PrintableRect { get; set; }
        private CGRect PaperRect { get; set; }

        public string Convert (UIView view, string filename)
        {
            var pdfData = InitializePDF (view);
            DrawCurrentView (view);
            return FinalizePDF (pdfData, filename);
        }

        private NSMutableData InitializePDF (UIView view)
        {
            SetDocumentSizes (view);
            var pdfData = new NSMutableData ();
            UIGraphics.BeginPDFContext (pdfData, CGRect.Empty, new NSDictionary ());
            return pdfData;
        }

        private void SetDocumentSizes (UIView view)
        {
            const int height = 800;
            const int width = 1240;
            const int space = 0;

            var pageMargins = new UIEdgeInsets (space, space, space, space);

            view.ViewPrintFormatter.ContentInsets = pageMargins;

            var pageSize = new CGSize (width, height);
            PrintableRect = new CGRect (pageMargins.Left,
                pageMargins.Top,
                pageSize.Width - pageMargins.Left - pageMargins.Right,
                pageSize.Height - pageMargins.Top - pageMargins.Bottom);

            PaperRect = new CGRect (0, 0, pageSize.Width, pageSize.Height);
        }

        private void DrawCurrentView (UIView view)
        {
            /*var render = new UIPrintPageRenderer ();
            render.AddPrintFormatter (view.ViewPrintFormatter, 0);
            render.SetValueForKey (NSValue.FromCGRect (PaperRect), new NSString ("paperRect"));
            render.SetValueForKey (NSValue.FromCGRect (PrintableRect), new NSString ("printableRect"));

            for (var i = 0; i < render.NumberOfPages; i++) {
                UIGraphics.BeginPDFPage ();
                var bounds = UIGraphics.PDFContextBounds;
                render.DrawPage (i, bounds);
            }*/
            try {
                var context = UIGraphics.GetCurrentContext ();
                view.Layer.RenderInContext (context);
            } catch (Exception e) {
                var i = e.Message;
            }
        }

        private string FinalizePDF (NSMutableData pdfData, string filename)
        {
            var filePath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), filename);
            UIGraphics.EndPDFContent ();
            pdfData.Save (filePath, true);
            return filePath;
        }
    }
}

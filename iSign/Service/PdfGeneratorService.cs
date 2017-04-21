using iSign.Core;
using iSign.Helpers;
using iSign.Services.Attributes;
using iSign.ViewControllers;
using UIKit;

namespace iSign.Service
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        private SignDocumentViewController SignDocumentViewController => ((UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController).VisibleViewController as SignDocumentViewController;
        public void Generate (string filename)
        {
            SignDocumentViewController.ScrollView.ToPDF (filename);
            var vc = new PDFViewerViewController (filename);
            SignDocumentViewController.PresentViewController (vc, true, null);
        }
    }
}

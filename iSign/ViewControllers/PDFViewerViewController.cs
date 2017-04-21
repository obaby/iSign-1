using System;
using System.IO;
using Foundation;
using UIKit;

namespace iSign.ViewControllers
{
    public partial class PDFViewerViewController : UIViewController
    {
        private static string GetPath (string filename) => Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), filename);
        private string Filename { get; set; }
        private string Oldname { get; set; }
        private string Originalname { get; }
        private bool _filenameChanged;

        public PDFViewerViewController (string filename) : base ("PDFViewer", null)
        {
            Filename = filename;
            Oldname = Filename;
            Originalname = Filename;
        }

        partial void ShareBtn_TouchUpInside (UIButton sender)
        {
            Rename ();
            var fileData = NSUrl.FromFilename (GetPath (Filename));
            var activityController = new UIActivityViewController (new NSObject [] { fileData }, null);
            if (activityController?.PopoverPresentationController != null) {
                activityController.PopoverPresentationController.SourceView = ShareBtn;
            }
            var topViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (topViewController.PresentedViewController != null) {
                topViewController = topViewController.PresentedViewController;
            }
            topViewController.PresentModalViewController (activityController, true);
        }

        partial void CloseBtn_TouchUpInside (UIButton sender)
        {
            Rename ();
            DismissModalViewController (true);
            Dispose ();
        }

        public void Rename ()
        {
            if (!_filenameChanged) return;
            if (File.Exists (GetPath (Filename))) File.Delete (GetPath (Filename));
            File.Move (GetPath (Oldname), GetPath (Filename));
            Oldname = Filename;
            _filenameChanged = false;
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            FilenameTextfield.Text = Filename;
            var path = GetPath (Filename);
            var uriPath = new Uri (path);
            var nsUrl = new NSUrl (uriPath.AbsolutePath);
            var nsUrlRequest = new NSUrlRequest (nsUrl);
            Viewer.LoadRequest (nsUrlRequest);
            FilenameTextfield.EditingChanged += FilenameTextfield_ValueChanged;
        }

        void FilenameTextfield_ValueChanged (object sender, EventArgs e)
        {
            Filename = FilenameTextfield.Text;
            if (string.IsNullOrEmpty (Filename) || string.IsNullOrWhiteSpace (Filename)) Filename = Originalname;
            _filenameChanged = true;
        }
    }
}

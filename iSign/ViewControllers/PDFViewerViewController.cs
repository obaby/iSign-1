using System;
using System.IO;
using Foundation;
using UIKit;

namespace iSign
{
    public partial class PDFViewerViewController : UIViewController
    {
        private string GetPath(string filename) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
        private string Filename { get; set; }
        private string Oldname { get; set; }
        private string Originalname { get; set; }
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
            File.Move (GetPath (Oldname), GetPath (Filename));
            Oldname = Filename;
            _filenameChanged = false;
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            Viewer.LoadRequest (new NSUrlRequest (new NSUrl (GetPath (Filename))));
            FilenameTextfield.EditingChanged+=FilenameTextfield_ValueChanged;
        }

        void FilenameTextfield_ValueChanged (object sender, EventArgs e)
        {
            Filename = FilenameTextfield.Text;
            if (string.IsNullOrEmpty (Filename) || string.IsNullOrWhiteSpace (Filename)) Filename = Originalname;
            _filenameChanged = true;
        }
    }
}


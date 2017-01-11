// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace iSign
{
    [Register ("PDFViewerViewController")]
    partial class PDFViewerViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CloseBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField FilenameTextfield { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ShareBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIWebView Viewer { get; set; }

        [Action ("CloseBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseBtn_TouchUpInside (UIKit.UIButton sender);

        [Action ("ShareBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ShareBtn_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CloseBtn != null) {
                CloseBtn.Dispose ();
                CloseBtn = null;
            }

            if (FilenameTextfield != null) {
                FilenameTextfield.Dispose ();
                FilenameTextfield = null;
            }

            if (ShareBtn != null) {
                ShareBtn.Dispose ();
                ShareBtn = null;
            }

            if (Viewer != null) {
                Viewer.Dispose ();
                Viewer = null;
            }
        }
    }
}
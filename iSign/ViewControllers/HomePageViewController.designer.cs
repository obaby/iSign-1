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
    [Register ("HomePageViewController")]
    partial class HomePageViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GoToSignDocButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GoToSignDocButton != null) {
                GoToSignDocButton.Dispose ();
                GoToSignDocButton = null;
            }
        }
    }
}
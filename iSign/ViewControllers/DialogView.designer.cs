// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace iSign.ViewControllers
{
    [Register ("DialogView")]
    partial class DialogView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CancelBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField InputTxt { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton OkBtn { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CancelBtn != null) {
                CancelBtn.Dispose ();
                CancelBtn = null;
            }

            if (InputTxt != null) {
                InputTxt.Dispose ();
                InputTxt = null;
            }

            if (OkBtn != null) {
                OkBtn.Dispose ();
                OkBtn = null;
            }
        }
    }
}
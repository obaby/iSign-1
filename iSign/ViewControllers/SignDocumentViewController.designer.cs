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
    [Register ("SignDocumentViewController")]
    partial class SignDocumentViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        iSign.ViewControllers.TouchableScrollView ContainerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton EditBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton EndEditingBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GeneratePdfBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LabelBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LoadFileBtn { get; set; }

        [Action ("EditBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void EditBtn_TouchUpInside (UIKit.UIButton sender);

        [Action ("EndEditingBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void EndEditingBtn_TouchUpInside (UIKit.UIButton sender);

        [Action ("GeneratePdfBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void GeneratePdfBtn_TouchUpInside (UIKit.UIButton sender);

        [Action ("LoadFileBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LoadFileBtn_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ContainerView != null) {
                ContainerView.Dispose ();
                ContainerView = null;
            }

            if (EditBtn != null) {
                EditBtn.Dispose ();
                EditBtn = null;
            }

            if (EndEditingBtn != null) {
                EndEditingBtn.Dispose ();
                EndEditingBtn = null;
            }

            if (GeneratePdfBtn != null) {
                GeneratePdfBtn.Dispose ();
                GeneratePdfBtn = null;
            }

            if (LabelBtn != null) {
                LabelBtn.Dispose ();
                LabelBtn = null;
            }

            if (LoadFileBtn != null) {
                LoadFileBtn.Dispose ();
                LoadFileBtn = null;
            }
        }
    }
}
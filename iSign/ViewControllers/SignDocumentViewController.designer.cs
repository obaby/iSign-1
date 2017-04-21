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
        UIKit.UIButton AddTextBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        iSign.ViewControllers.TouchableScrollView ContainerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GeneratePdfBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint HandHeightConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint HandTopConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton HelpButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint HelpHeightConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint HelpTopConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint LabelHeightConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint LabelTopConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LoadFileBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint LoadHeightConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint LoadTopConstrain { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint PdfHeightConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint PdfTopConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RotateBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SignBtn { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AddTextBtn != null) {
                AddTextBtn.Dispose ();
                AddTextBtn = null;
            }

            if (ContainerView != null) {
                ContainerView.Dispose ();
                ContainerView = null;
            }

            if (GeneratePdfBtn != null) {
                GeneratePdfBtn.Dispose ();
                GeneratePdfBtn = null;
            }

            if (HandHeightConstraint != null) {
                HandHeightConstraint.Dispose ();
                HandHeightConstraint = null;
            }

            if (HandTopConstraint != null) {
                HandTopConstraint.Dispose ();
                HandTopConstraint = null;
            }

            if (HelpButton != null) {
                HelpButton.Dispose ();
                HelpButton = null;
            }

            if (HelpHeightConstraint != null) {
                HelpHeightConstraint.Dispose ();
                HelpHeightConstraint = null;
            }

            if (HelpTopConstraint != null) {
                HelpTopConstraint.Dispose ();
                HelpTopConstraint = null;
            }

            if (LabelHeightConstraint != null) {
                LabelHeightConstraint.Dispose ();
                LabelHeightConstraint = null;
            }

            if (LabelTopConstraint != null) {
                LabelTopConstraint.Dispose ();
                LabelTopConstraint = null;
            }

            if (LoadFileBtn != null) {
                LoadFileBtn.Dispose ();
                LoadFileBtn = null;
            }

            if (LoadHeightConstraint != null) {
                LoadHeightConstraint.Dispose ();
                LoadHeightConstraint = null;
            }

            if (LoadTopConstrain != null) {
                LoadTopConstrain.Dispose ();
                LoadTopConstrain = null;
            }

            if (PdfHeightConstraint != null) {
                PdfHeightConstraint.Dispose ();
                PdfHeightConstraint = null;
            }

            if (PdfTopConstraint != null) {
                PdfTopConstraint.Dispose ();
                PdfTopConstraint = null;
            }

            if (RotateBtn != null) {
                RotateBtn.Dispose ();
                RotateBtn = null;
            }

            if (SignBtn != null) {
                SignBtn.Dispose ();
                SignBtn = null;
            }
        }
    }
}
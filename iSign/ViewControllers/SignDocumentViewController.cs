using System;
using System.IO;
using System.Net;
using CoreGraphics;
using iSign.Core.ViewModels;
using iSign.Helpers;
using MobileCoreServices;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using UIKit;

namespace iSign.ViewControllers
{
    public partial class SignDocumentViewController : MvxViewController<SigningDocViewModel>, IMvxModalIosView
    {
        public bool EditMode { get; private set; }
        private UIImageView _imageView;
        public SignDocumentViewController () : base ("SignDocumentView", null)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            ChangeButtonsState (false);
        }

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
            UpdateConstraints ();
            NavigationController.NavigationBar.Hidden = true;
        }

        void UpdateConstraints ()
        {
            var iPad = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
            var topConstraint = iPad ? 50 : 25;
            var heightConstraint = iPad ? 55 : 35;

            HelpTopConstraint.Constant = topConstraint;
            HelpHeightConstraint.Constant = heightConstraint;
            HandTopConstraint.Constant = topConstraint;
            HandHeightConstraint.Constant = heightConstraint;
            LabelTopConstraint.Constant = topConstraint;
            LabelHeightConstraint.Constant = heightConstraint;
            LoadTopConstrain.Constant = topConstraint;
            LoadHeightConstraint.Constant = heightConstraint;
            PdfTopConstraint.Constant = topConstraint;
            PdfHeightConstraint.Constant = heightConstraint;
        }
       
        partial void EditBtn_TouchUpInside (UIButton sender)
        {
            ContainerView.ShowSigningView (Context.SigningViewModel);
        }

        void Converter_ImageCreated (object sender, UIImage e)
        {
            _imageView = new UIImageView (e);
            _imageView.RemoveFromSuperview ();
            ContainerView.Add (_imageView);
            ContainerView.ContentSize = _imageView.Frame.Size;
        }


        public int NbRotations { get; private set; } = 0;

        partial void EndEditingBtn_TouchUpInside (UIButton sender)
        {
            NbRotations++;
            var degrees = NbRotations * 90 * (nfloat)Math.PI / 180;
            var frame = _imageView.Frame;
            _imageView.RemoveFromSuperview ();
            _imageView.Transform = CGAffineTransform.MakeRotation (degrees);
            _imageView.Frame = new CGRect (frame.X, frame.Y, frame.Height, frame.Width);
            ContainerView.Add (_imageView);
            ContainerView.ContentSize = _imageView.Frame.Size;
        }
        SigningDocViewModel Context => DataContext as SigningDocViewModel;
        partial void GeneratePdfBtn_TouchUpInside (UIButton sender)
        {
            var filename = ContainerView.ToPDF (Context.Filename);
            var vc = new PDFViewerViewController (Context.Filename);
            PresentViewController (vc, true, null);
        }

        void FileDownloaded (string filename)
        {
            UIImage image = null;
            try {
                image = new UIImage (filename);
            } catch {
                image = PDFToImage.Convert (filename, true);
            }
            _imageView = new UIImageView (image);
            var width = image.Size.Width;
            var height = image.Size.Height;
            var ratio = width / height;
            if (width < ContainerView.Frame.Width) {
                width = ContainerView.Frame.Width;
                height = width / ratio;
            }
            if (height < ContainerView.Frame.Height) {
                height = ContainerView.Frame.Height;
                width = height * ratio;
            }
            _imageView.Frame = new CGRect (_imageView.Frame.X, _imageView.Frame.Y, width, height);
            _imageView.RemoveFromSuperview ();
            ContainerView.Add (_imageView);
            ContainerView.ContentSize = _imageView.Frame.Size;
            ChangeButtonsState (true);
        }

        private void ChangeButtonsState (bool fileLoaded)
        {
            EndEditingBtn.Hidden = !fileLoaded;
            EditBtn.Enabled = fileLoaded;
            LabelBtn.Enabled = fileLoaded;
            GeneratePdfBtn.Enabled = fileLoaded;
        }

        partial void LoadFileBtn_TouchUpInside (UIButton sender)
        {
            if (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR) {
                FileDownloaded ("Pdf/FastFlex.jpg");
                Context.Filename = "FastFlex.jpg";
                return;
            }
            var allowedUtis = new string [] {
                UTType.PDF,
                UTType.Image
            };

            var pickerMenu = new UIDocumentMenuViewController (allowedUtis, UIDocumentPickerMode.Import);
            pickerMenu.DidPickDocumentPicker += (s, args) => {

                args.DocumentPicker.DidPickDocument += (sndr, pArgs) => {

                    var securityEnabled = pArgs.Url.StartAccessingSecurityScopedResource ();
                    var filename = pArgs.Url.LastPathComponent;
                    var client = new WebClient ();
                    Context.Filename = filename;
                    Directory.CreateDirectory (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "DL"));
                    var localpath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "DL" ,filename);
                    client.DownloadFile (pArgs.Url, localpath);
                    FileDownloaded (localpath);

                    pArgs.Url.StopAccessingSecurityScopedResource ();
                };

                PresentViewController (args.DocumentPicker, true, null);
            };

            pickerMenu.ModalPresentationStyle = UIModalPresentationStyle.Popover;
            PresentViewController (pickerMenu, true, null);
            UIPopoverPresentationController presentationPopover = pickerMenu.PopoverPresentationController;
            if (presentationPopover != null) {
                presentationPopover.SourceView = View;
                presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Down;
                presentationPopover.SourceRect = LoadFileBtn.Frame;
            }
        }

        partial void LabelTouchUpInside (UIButton sender)
        {
            ContainerView.ShowTextView ();
        }
   }
}


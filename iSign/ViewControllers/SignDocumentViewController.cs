using System;
using System.IO;
using System.Net;
using CoreGraphics;
using iSign.Converters;
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
            this.DelayBind (SetBindings);
        }

        private void SetBindings ()
        {
            var set = this.CreateBindingSet<SignDocumentViewController, SigningDocViewModel> ();
            set.Bind (SignBtn)
               .To (vm => vm.AddImageCommand);
            set.Bind (SignBtn)
               .For (v => v.Enabled)
               .To (vm => vm.CanAddSignature);

            set.Bind (AddTextBtn)
               .To (vm => vm.AddTextCommand);
            set.Bind (AddTextBtn)
              .For (v => v.Enabled)
              .To (vm => vm.CanAddText);

            set.Bind (GeneratePdfBtn)
               .To (vm => vm.GeneratePdfCommand);
            set.Bind (GeneratePdfBtn)
               .For (v => v.Enabled)
               .To (vm => vm.CanGeneratePdf);
            
            set.Bind (RotateBtn)
               .For (v => v.Hidden)
               .To (vm => vm.CanRotate)
               .WithConversion (new ReverseBooleanConverter ());
            set.Bind (RotateBtn)
               .To (vm => vm.RotateCommand);

            set.Bind (LoadFileBtn)
               .To (vm => vm.LoadFileCommand);
            set.Bind (LoadFileBtn)
               .For (v => v.Enabled)
               .To (vm => vm.CanLoadFile);

            set.Apply ();
            Context.OnRotatedImage += ViewModel_OnRotatedImage;
            Context.OnLoadFile += Context_OnLoadFile;
        }

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
            MenuView.BackgroundColor = UIColor.FromRGB(42, 56, 93);
            UpdateConstraints ();
            NavigationController.NavigationBar.Hidden = true;
        }

        private void UpdateConstraints ()
        {
            var iPad = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
            var topConstraint = iPad ? 50 : 25;
            var heightConstraint = iPad ? 55 : 35;

            HelpTopConstraint.Constant = topConstraint;
            HelpHeightConstraint.Constant = heightConstraint;
            SignTopConstraint.Constant = topConstraint;
            SignHeightConstraint.Constant = heightConstraint;
            AddTextTopConstraint.Constant = topConstraint;
            AddTextHeightConstraint.Constant = heightConstraint;
            LoadTopConstraint.Constant = topConstraint;
            LoadHeightConstraint.Constant = heightConstraint;
            PdfTopConstraint.Constant = topConstraint;
            PdfHeightConstraint.Constant = heightConstraint;
        }
       
        private void Converter_ImageCreated (object sender, UIImage e)
        {
            _imageView = new UIImageView (e);
            _imageView.RemoveFromSuperview ();
            ContainerView.Add (_imageView);
            ContainerView.ContentSize = _imageView.Frame.Size;
        }

        public int NbRotations { get; private set; } = 0;

        private void ViewModel_OnRotatedImage (object sender, EventArgs e)
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

        private SigningDocViewModel Context => DataContext as SigningDocViewModel;
        private void FileDownloaded (string filename)
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
        }

        private void Context_OnLoadFile (object sender, Action action)
        {
            if (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR) {
                FileDownloaded ("Pdf/FastFlex.jpg");
                Context.Filename = "FastFlex.jpg";
                action ();
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
                    var localpath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "DL", filename);
                    client.DownloadFile (pArgs.Url, localpath);

                    FileDownloaded (localpath);
                    action ();
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

        public TouchableScrollView ScrollView => ContainerView;
   }
}


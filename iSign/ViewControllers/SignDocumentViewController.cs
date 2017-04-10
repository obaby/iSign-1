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
    public partial class SignDocumentViewController : MvxViewController<SigningDocViewModel>
    {
        public bool EditMode { get; private set; }
        private UIImageView _imageView;
        public SignDocumentViewController () : base ("SignDocumentView", null)
        {
            this.DelayBind (SetBindings);
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            EndEditingBtn.Hidden = false;
        }

        private void Context_InputSet (object sender, string e)
        {
            EditMode = true;
            //Todo : add label
        }

        partial void EditBtn_TouchUpInside (UIButton sender)
        {
            ContainerView.ShowSigningView (Context.SigningViewModel);
        }

        private void SetBindings ()
        {
            var set = this.CreateBindingSet<SignDocumentViewController, SigningDocViewModel> ();
            set.Bind (LabelBtn)
               .To (vm => vm.AddLabelCommand);

               set.Apply ();

            var context = DataContext as SigningDocViewModel;
            if (context == null) return;

            context.InputSet += Context_InputSet;

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
   }
}


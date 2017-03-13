using System;
using System.IO;
using System.Net;
using CoreGraphics;
using iSign.Core;
using iSign.Helpers;
using MobileCoreServices;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using UIKit;

namespace iSign
{
    public partial class SignDocumentViewController : MvxViewController<SigningDocViewModel>
    {
        private bool _editMode;
        private UIImageView _imageView;
        public SignDocumentViewController () : base ("SignDocumentView", null)
        {
            this.DelayBind (SetBindings);
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            EndEditingBtn.Hidden = false;
            ContainerView.FinishedAddingView += ContainerView_FinishedAddingView;
        }

        void Context_InputSet (object sender, string e)
        {
            _editMode = true;
            ContainerView.AddLabel (e);
        }

        partial void EditBtn_TouchUpInside (UIButton sender)
        {
            _editMode = !_editMode;
            if (!_editMode) { ContainerView.EndEditMode (); return; }
            ContainerView.SetToEditMode ();
        }


        void ContainerView_FinishedAddingView (object sender, EventArgs e)
        {
            _editMode = false;
        }

        private void SetBindings ()
        {
            var set = this.CreateBindingSet<SignDocumentViewController, SigningDocViewModel> ();
            set.Bind (LabelBtn)
               .To (vm => vm.AddLabelCommand);
            set.Bind (ContainerView)
               .For(v => v.PaletteContext)
               .To (vm => vm.PaletteContext);

               set.Apply ();

            var context = DataContext as SigningDocViewModel;
            if (context == null) return;

            context.InputSet += Context_InputSet;

        }

        void Converter_ImageCreated (object sender, UIImage e)
        {
            _imageView = new UIImageView (e);
            ContainerView.Clear ();
            _imageView.RemoveFromSuperview ();
            ContainerView.Add (_imageView);
            ContainerView.ContentSize = _imageView.Frame.Size;
        }


        int nbRotations = 0;
        partial void EndEditingBtn_TouchUpInside (UIButton sender)
        {
            nbRotations++;
            var degrees = nbRotations * 90 * (nfloat)Math.PI / 180;
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
            ContainerView.EndUpdate ();
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
            ContainerView.Clear ();
            _imageView.RemoveFromSuperview ();
            ContainerView.Add (_imageView);
            ContainerView.ContentSize = _imageView.Frame.Size;
        }

        partial void LoadFileBtn_TouchUpInside (UIButton sender)
        {
            if (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR) {
                FileDownloaded ("Pdf/FastFlex.jpg");
                return;
            }
            var allowedUTIs = new string [] {
                UTType.PDF,
                UTType.Image
            };

            // Display the picker
            //var picker = new UIDocumentPickerViewController (allowedUTIs, UIDocumentPickerMode.Open);
            var pickerMenu = new UIDocumentMenuViewController (allowedUTIs, UIDocumentPickerMode.Import);
            pickerMenu.DidPickDocumentPicker += (s, args) => {

                // Wireup Document Picker
                args.DocumentPicker.DidPickDocument += (sndr, pArgs) => {

                    // IMPORTANT! You must lock the security scope before you can
                    // access this file
                    var securityEnabled = pArgs.Url.StartAccessingSecurityScopedResource ();
                    var filename = pArgs.Url.LastPathComponent;
                    var client = new WebClient ();
                    Context.Filename = filename;
                    Directory.CreateDirectory (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "DL"));
                    var localpath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "DL" ,filename);
                    client.DownloadFile (pArgs.Url, localpath);
                    // Open the document
                    FileDownloaded (localpath);

                    // IMPORTANT! You must release the security lock established
                    // above.
                    pArgs.Url.StopAccessingSecurityScopedResource ();
                };

                // Display the document picker
                PresentViewController (args.DocumentPicker, true, null);
            };

            pickerMenu.ModalPresentationStyle = UIModalPresentationStyle.Popover;
            PresentViewController (pickerMenu, true, null);
            UIPopoverPresentationController presentationPopover = pickerMenu.PopoverPresentationController;
            if (presentationPopover != null) {
                presentationPopover.SourceView = this.View;
                presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Down;
                presentationPopover.SourceRect = LoadFileBtn.Frame;
            }
        }
   }
}


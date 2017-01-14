using System;
using System.IO;
using System.Net;
using CoreGraphics;
using Dropins.Chooser.iOS;
using iSign.Core;
using iSign.Helpers;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.Platform;
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
            EndEditingBtn.Hidden = true;
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
            set.Apply ();

            var context = DataContext as SigningDocViewModel;
            if (context == null) return;

            context.InputSet+= Context_InputSet;
            context.FileDownloaded += Context_FileDownloaded;
                
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
            var degrees = nbRotations * 90 * (nfloat) Math.PI / 180;
            var frame = _imageView.Frame;
            _imageView.RemoveFromSuperview ();
            _imageView.Transform = CGAffineTransform.MakeRotation (degrees);
            _imageView.Frame = new CGRect (frame.X, frame.Y, frame.Height, frame.Width);
            ContainerView.Add (_imageView);
            ContainerView.ContentSize = _imageView.Frame.Size;
        }

        partial void GeneratePdfBtn_TouchUpInside (UIButton sender)
        {
            ContainerView.EndUpdate ();
            var filename = ContainerView.ToPDF ("result.pdf");
            var vc = new PDFViewerViewController (filename);
            PresentViewController (vc, true, null);
        }

        void Context_FileDownloaded (object sender, string filename)
        {
            var image = PDFToImage.Convert (filename, true);
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

        private void LoadFromImage ()
        {
            var image = new UIImage ("Pdf/FastFlex.jpg");
            _imageView = new UIImageView (image);

            ContainerView.Clear ();
            _imageView.RemoveFromSuperview ();
            ContainerView.Add (_imageView);
            ContainerView.UserInteractionEnabled = true;
            ContainerView.ContentSize = _imageView.Frame.Size;
        }

        partial void LoadFileBtn_TouchUpInside (UIButton sender)
        {
            var context = DataContext as SigningDocViewModel;
            if (context == null) return;
            var factory = Mvx.Resolve<IFileStorageChooser> ();
            UIAlertController actionSheetAlert = UIAlertController.Create ("Select the source", "", UIAlertControllerStyle.ActionSheet);
            foreach (var fileStorage in factory.FileStorages) {
                actionSheetAlert.AddAction (UIAlertAction.Create (fileStorage.Name, UIAlertActionStyle.Default, (action) => context.LoadFile (fileStorage)));
            }

            UIPopoverPresentationController presentationPopover = actionSheetAlert.PopoverPresentationController;
            if (presentationPopover != null) {
                presentationPopover.SourceView = LoadFileBtn;
            }
            PresentViewController (actionSheetAlert, true, null);
        }
    }
}


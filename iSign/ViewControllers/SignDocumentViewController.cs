using System;
using CoreGraphics;
using iSign.Core;
using iSign.Helpers;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using UIKit;

namespace iSign
{
    public partial class SignDocumentViewController : MvxViewController<SigningDocViewModel>
    {
        private bool _editMode;

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

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
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
                
        }

        partial void LoadFileBtn_TouchUpInside (UIButton sender)
        {
            LoadFromPDFFile ();
            EndEditingBtn.Hidden = false;
        }

        void Converter_ImageCreated (object sender, UIImage e)
        {
            var imageview = new UIImageView (e);
            ContainerView.Clear ();
            ContainerView.Add (imageview);
            ContainerView.ContentSize = imageview.Frame.Size;
        }


        int nbRotations = 0;
        partial void EndEditingBtn_TouchUpInside (UIButton sender)
        {
            nbRotations++;
            var degrees = nbRotations * 90 * (nfloat) Math.PI / 180;
            ContainerView.Transform = CGAffineTransform.MakeRotation (degrees);
        }

        partial void GeneratePdfBtn_TouchUpInside (UIButton sender)
        {
            var filename = ViewToPDF.Convert (ContainerView, "result.pdf");
        }

        private void LoadFromPDFFile ()
        {
            var image = PDFToImage.Convert ("Pdf/Timesheet.pdf");
            var imageview = new UIImageView (image);
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
            imageview.Frame = new CGRect (imageview.Frame.X, imageview.Frame.Y, width, height);
            ContainerView.Clear ();
            ContainerView.Add (imageview);
            ContainerView.ContentSize = imageview.Frame.Size;
        }

        private void LoadFromImage ()
        {
            var image = new UIImage ("Pdf/FastFlex.jpg");
            var imageview = new UIImageView (image);

            ContainerView.Clear ();
            ContainerView.Add (imageview);
            ContainerView.UserInteractionEnabled = true;
            ContainerView.ContentSize = imageview.Frame.Size;
        }
    }
}


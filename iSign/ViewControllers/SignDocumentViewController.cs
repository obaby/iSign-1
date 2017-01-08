using System;
using CoreGraphics;
using iSign.Core;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using UIKit;

namespace iSign
{
    public partial class SignDocumentViewController : MvxViewController<SigningDocViewModel>
    {
        private bool _editMode;
        private UIImageView Imageview { get; set; }

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
            var image = new UIImage ("Pdf/FastFlex.jpg");
            Imageview = new UIImageView (image);

            ContainerView.Add (Imageview);
            ContainerView.UserInteractionEnabled = true;
            ContainerView.ContentSize = Imageview.Frame.Size;
        }

        partial void UIButton92_TouchUpInside (UIButton sender)
        {
        }


        void Converter_ImageCreated (object sender, UIImage e)
        {
            Imageview = new UIImageView (e);
            ContainerView.Add (Imageview);
            ContainerView.ContentSize = Imageview.Frame.Size;
        }

        partial void GeneratePdfBtn_TouchUpInside (UIButton sender)
        {
            var converter = new PdfToImage ();
            converter.ImageCreated += Converter_ImageCreated; ;
            converter.Load ("Pdf/Timesheet.pdf");
        }
    }
}


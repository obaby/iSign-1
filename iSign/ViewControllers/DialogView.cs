using System;
using CoreGraphics;
using iSign.Core.ViewModels;
using iSign.Helpers;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace iSign.ViewControllers
{
    public partial class DialogView : MvxView, IImageView
    {
        public DialogView (IntPtr handle) : base (handle)
        {
            this.DelayBind (SetBindings);
        }

        private void SetBindings ()
        {
            var set = this.CreateBindingSet<DialogView, DialogTextViewModel> ();
            set.Bind (OkBtn).To (vm => vm.OkCommand);
            set.Bind (OkBtn).For ("Title").To (vm => vm.OkTxt);
            set.Bind (CancelBtn).To (vm => vm.CancelCommand);
            set.Bind (CancelBtn).For ("Title").To (vm => vm.CancelTxt);
            set.Bind (InputTxt).To (vm => vm.Input);
            set.Bind (InputTxt)
               .For (v => v.Placeholder)
               .To (vm => vm.Placeholder);
            set.Apply ();

            if (ViewModel == null) return;
            ViewModel.OnOk += ViewModel_OnOk;
            ViewModel.OnCancel += ViewModel_OnCancel;
        }

        DialogTextViewModel ViewModel => DataContext as DialogTextViewModel;

        public override void LayoutSubviews ()
        {
            ContainerView.Layer.CornerRadius = 20;
            BorderView.Layer.CornerRadius = 20;
            BackgroundColor = UIColor.FromRGB (0, 153, 255).ColorWithAlpha (0.3f);
            InputTxt.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
            base.LayoutSubviews ();
        }

        public Action OkAction { get; set; }
        public Action CancelAction { get; set; }

        public CGSize MinimumSize => new CGSize (186, 30);

        public ImageText GetImage ()
        {
            var borderStyle = InputTxt.BorderStyle;
            InputTxt.BorderStyle = UITextBorderStyle.None;
            var clearButtonMode = InputTxt.ClearButtonMode;
            InputTxt.ClearButtonMode = UITextFieldViewMode.Never;
            InputTxt.ResignFirstResponder ();
            var image = InputTxt.ToUIImage ();
            InputTxt.BorderStyle = borderStyle;
            InputTxt.ClearButtonMode = clearButtonMode;
            return new ImageText { Image = image, Text = InputTxt.Text };
        }

        void ViewModel_OnOk (object sender, EventArgs e)
        {
            OkAction?.Invoke ();
            CloseView ();
        }

        void ViewModel_OnCancel (object sender, EventArgs e)
        {
            CancelAction?.Invoke ();
            CloseView ();
        }

        private void CloseView ()
        {
            RemoveFromSuperview ();
        }

        public void StartWith (ImageText imageText)
        {
            
        }
    }
}
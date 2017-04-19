using System;
using iSign.Core.ViewModels;
using iSign.Helpers;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace iSign.ViewControllers
{
    public partial class DialogView : MvxView
    {

        public DialogView (IntPtr handle) : base (handle)
        {
            //this.DelayBind (SetBindings);
        }

        private void SetBindings ()
        {
            var set = this.CreateBindingSet<DialogView, DialogViewModel> ();
            set.Bind (OkBtn).To (vm => vm.OkCommand);
            set.Bind (OkBtn).For ("Title").To (vm => vm.OkTxt);
            set.Bind (CancelBtn).To (vm => vm.CancelCommand);
            set.Bind (CancelBtn).For ("Title").To (vm => vm.CancelTxt);
            set.Bind (InputTxt).To (vm => vm.Input);
            set.Apply ();
        }

        private string Text { get; set; }

        internal void StartWith (string text)
        {
            Text = text;
        }

        public override void LayoutSubviews ()
        {
            ContainerView.Layer.CornerRadius = 20;
            BorderView.Layer.CornerRadius = 20;
            BackgroundColor = UIColor.FromRGB (0, 153, 255).ColorWithAlpha (0.3f);
            InputTxt.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
            OkBtn.SetTitle ("OK", UIControlState.Normal);
            OkBtn.TouchUpInside += OkBtn_TouchUpInside;
            CancelBtn.SetTitle ("Cancel", UIControlState.Normal);
            CancelBtn.TouchUpInside += CancelBtn_TouchUpInside;
            InputTxt.Placeholder = "Text";
            InputTxt.Text = Text;
            base.LayoutSubviews ();
        }

        public Action OkAction { get; set; }
        public Action CancelAction { get; set; }

        public ImageText GetImageOfText ()
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

        void OkBtn_TouchUpInside (object sender, EventArgs e)
        {
            OkAction?.Invoke ();
            CloseView ();
        }

        void CancelBtn_TouchUpInside (object sender, EventArgs e)
        {
            CancelAction?.Invoke ();
            CloseView ();
        }

        private void CloseView ()
        {
            CancelBtn.TouchUpInside -= CancelBtn_TouchUpInside;
            OkBtn.TouchUpInside -= OkBtn_TouchUpInside;
            RemoveFromSuperview ();
        }

        public class ImageText
        {
            public UIImage Image { get; set; }
            public string Text { get; set; }
        }

   }
}
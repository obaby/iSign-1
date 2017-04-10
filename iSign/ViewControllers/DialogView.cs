using System;
using iSign.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace iSign.ViewControllers
{
    public partial class DialogView : MvxView
    {
        public DialogView (IntPtr handle) : base (handle)
        {
            this.DelayBind (SetBindings);
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

        public override void LayoutSubviews ()
        {
            base.LayoutSubviews ();
            BackgroundColor = UIColor.Clear;
            InputTxt.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
        }
    }
}
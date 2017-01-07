using System;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;

namespace iSign
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


            set.Apply ();
        }
    }
}
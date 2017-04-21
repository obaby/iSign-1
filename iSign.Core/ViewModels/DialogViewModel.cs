using System;
using iSign.Core.Services;
using MvvmCross.Core.ViewModels;

namespace iSign.Core.ViewModels
{
    public abstract class DialogViewModel : BaseViewModel
    {
        protected DialogViewModel (IViewModelServices viewModelService) : base (viewModelService)
        {
            OkCommand = new MvxCommand (() => {
                OkAction (Input);
                InvokeOnOk ();
            });

            CancelCommand = new MvxCommand (() => {
                CancelAction ();
                InvokeOnCancel ();
            });

            OkTxt = "OK";
            CancelTxt = "Cancel";
            Placeholder = "Text";
        }

        public Action<string> OkAction { get; set; }
        public Action CancelAction { get; set; }

        public IMvxCommand OkCommand { get; }
        public IMvxCommand CancelCommand { get; }

        public event EventHandler OnOk;
        public event EventHandler OnCancel;

        private void InvokeOnOk ()
        {
            OnOk?.Invoke (this, EventArgs.Empty);
        }

        private void InvokeOnCancel ()
        {
            OnCancel?.Invoke (this, EventArgs.Empty);
        }

        public string OkTxt { get; protected set; }
        public string CancelTxt { get; protected set; }
        public string Input { get; set; }
        public string Placeholder { get; protected set; }
        public abstract void Reload ();
    }
}

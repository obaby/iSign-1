using System;
using MvvmCross.Core.ViewModels;

namespace iSign.Core.ViewModels
{
    public class DialogViewModel : MvxViewModel, IReloadableViewModel
    {
        public DialogViewModel (Action<string> okAction, Action cancelAction)
        {
            OkCommand = new MvxCommand (() => okAction(Input));
            CancelCommand = new MvxCommand (cancelAction);
        }

        public IMvxCommand OkCommand { get; }
        public IMvxCommand CancelCommand { get; }
        public string OkTxt => "OK";
        public string CancelTxt => "Cancel";
        public string Input { get; set; }

        public void Reload ()
        {
            throw new NotImplementedException ();
        }
    }
}

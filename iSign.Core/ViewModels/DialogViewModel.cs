using System;
using MvvmCross.Core.ViewModels;

namespace iSign.Core
{
    public class DialogViewModel : MvxViewModel
    {
        public DialogViewModel (Action<string> OkAction, Action cancelAction)
        {
            OkCommand = new MvxCommand (() => OkAction(Input));
            CancelCommand = new MvxCommand (cancelAction);
        }

        public IMvxCommand OkCommand { get; }
        public IMvxCommand CancelCommand { get; }
        public string OkTxt => "OK";
        public string CancelTxt => "Cancel";
        public string Input { get; set; }
    }
}

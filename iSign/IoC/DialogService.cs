using System;
using Foundation;
using iSign.Core.Services;
using iSign.Core.ViewModels;
using iSign.ViewControllers;
using ObjCRuntime;
using UIKit;

namespace iSign.IoC
{
    public class DialogService : IDialogService
    {
        private DialogView DialogView { get; }
        public DialogService ()
        {
            var arr = NSBundle.MainBundle.LoadNib ("DialogView", null, null);
            DialogView = Runtime.GetNSObject<DialogView> (arr.ValueAt (0));
        }

        public void ShowDialog (Action<string> okAction)
        {
            var vc = ((UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController).VisibleViewController;

            var vm = new DialogViewModel (s => { okAction (s); Hide ();}, Hide);
            DialogView.DataContext = vm;
            DialogView.Frame = vc.View.Frame;
            vc.Add (DialogView);
        }

        private void Hide ()
        {
            DialogView.RemoveFromSuperview ();
        }
    }
}

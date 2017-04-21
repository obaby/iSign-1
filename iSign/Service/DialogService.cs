using CoreGraphics;
using iSign.Core.Services;
using iSign.Core.ViewModels;
using iSign.Extensions;
using iSign.ViewControllers;
using iSign.Views;
using UIKit;

namespace iSign.Service
{
    public class DialogService : IDialogService
    {
        private IImageView OpenedDialogView { get; set; }
        private void HideDialog ()
        {
            _dialogViewIsShown = false;
        }

        public void ShowImageDialog (DialogViewModel context)
        {
            var dialogView = new SigningView (SignDocumentViewController.ScrollView.Frame);
            ShowDialog (dialogView, context);
        }

        public void ShowTextDialog (DialogViewModel context)
        {
            var dialogView = ViewFactory.Create<DialogView> ();
			ShowDialog (dialogView, context);
        }

        private bool _dialogViewIsShown;
       
        SignDocumentViewController SignDocumentViewController => ((UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController).VisibleViewController as SignDocumentViewController;

        private void ShowDialog (IImageView dialogView, DialogViewModel context)
        {
            if (_dialogViewIsShown) return;
            var scrollView = SignDocumentViewController.ScrollView;
            dialogView.Frame = scrollView.Frame;
            dialogView.DataContext = context;
            EditableView editableView = null;
            var reopened = false;

            dialogView.CancelAction = HideDialog;

            dialogView.OkAction = () => {
                var signature = dialogView.GetImage ();
                if (signature == null) return;
                var previousRect = CGRect.Empty;
                if (reopened) {
                    if (editableView != null) {
                        previousRect = editableView.Frame;
                        editableView.Remove ();
                    }
                    reopened = false;
                }
                var center = scrollView.GetCenter (signature.Image.Size, scrollView.ContentOffset);
                var position = previousRect == CGRect.Empty ? new CGRect (center, signature.Image.Size) : previousRect;
                editableView = new EditableView (position) {
                    LimitSize = dialogView.MinimumSize
                };
                editableView.SetImage (signature.Image);
                if (previousRect != CGRect.Empty) {
                    editableView.Frame = previousRect;
                    editableView.UpdateImageAndLayer (previousRect.Size);
                }

                scrollView.Add (editableView);

                editableView.OnDoubleTap = () => {
                    var vm = dialogView.DataContext as DialogViewModel;
                    vm?.Reload ();
                    reopened = true;
                    _dialogViewIsShown = true;
                    OpenedDialogView = dialogView;
                    dialogView.StartWith (signature);
                    SignDocumentViewController.Add ((UIView)dialogView);
                };
                HideDialog ();
            };

            SignDocumentViewController.Add ((UIView)dialogView);
            OpenedDialogView = dialogView;
            _dialogViewIsShown = true;
        }
    }
}
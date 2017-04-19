using System;
using CoreGraphics;
using iSign.Core;
using iSign.Core.ViewModels;
using iSign.Extensions;
using iSign.Views;
using MvvmCross.Core.ViewModels;
using UIKit;

namespace iSign.ViewControllers
{
    public partial class TouchableScrollView : UIScrollView
    {
        public TouchableScrollView (IntPtr handle) : base (handle)
        {
        }

        public override void LayoutSubviews ()
        {
            base.LayoutSubviews ();
            PanGestureRecognizer.MinimumNumberOfTouches = 2;
        }

        private bool _dialogViewIsShown;

        public void ShowSigningView (IReloadableViewModel context)
        {
            var signingView = new SigningView (Frame);
            ShowDialog (signingView, context);
        }

        public void ShowTextView ()
        {
            var dialogView = ViewFactory.Create<DialogView> ();
            ShowDialog (dialogView);

        }

        private void ShowDialog (IImageView dialogView, IReloadableViewModel context = null)
        {
            if (_dialogViewIsShown) return;
            var vc = ((UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController).VisibleViewController;
            dialogView.Frame = Frame;
            dialogView.DataContext = context;
            EditableView editableView = null;
            var reopened = false;

            dialogView.CancelAction = () => {
                _dialogViewIsShown = false;
            };

            dialogView.OkAction = () => {
                _dialogViewIsShown = false;
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
                var center = this.GetCenter (signature.Image.Size, ContentOffset);
                var position = previousRect == CGRect.Empty ? new CGRect (center, signature.Image.Size) : previousRect;
                editableView = new EditableView (position) {
                    LimitSize = dialogView.MinimumSize
                };
                editableView.SetImage (signature.Image);
                if (previousRect != CGRect.Empty) {
                    editableView.Frame = previousRect;
                    editableView.UpdateImageAndLayer (previousRect.Size);
                }

                Add (editableView);

                editableView.OnDoubleTap = () => {
                    var vm = dialogView.DataContext as IReloadableViewModel;
                    vm?.Reload ();
                    reopened = true;
                    _dialogViewIsShown = true;
                    dialogView.StartWith (signature);
                    vc.Add ((UIView)dialogView);
                };
            };

            vc.Add ((UIView)dialogView);
            _dialogViewIsShown = true;
        }
    }
}
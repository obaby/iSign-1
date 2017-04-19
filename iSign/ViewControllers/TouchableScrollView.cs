using System;
using CoreGraphics;
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

        private bool _signingViewIsShown;

        public void ShowSigningView (IMvxViewModel context)
        {
            if (_signingViewIsShown) return;
            var vc = ((UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController).VisibleViewController;
            var signingView = new SigningView (Frame);
            EditableView editableView = null;
            var doubleTapped = false;
            signingView.DataContext = context;
            signingView.CancelAction = () => {
                _signingViewIsShown = false;
            };

            signingView.OkAction = () => {
                _signingViewIsShown = false;
                var signature = signingView.GetSignature ();
                if (signature == null) return;
                var previousRect = CGRect.Empty;
                if (doubleTapped) {
                    if (editableView != null) {
                        previousRect = editableView.Frame;
                        editableView.Remove ();
                    }
                    doubleTapped = false;
                }
                var center = this.GetCenter (signature.Size, ContentOffset);
                var position = previousRect == CGRect.Empty ? new CGRect (center, signature.Size) : previousRect;
                editableView = new EditableView (position);
                editableView.SetImage (signature);
                if (previousRect != CGRect.Empty) {
                    editableView.Frame = previousRect;
                    editableView.UpdateImageAndLayer (previousRect.Size);
                }
                Add (editableView);

                editableView.OnDoubleTap = () => {
                    var vm = signingView.DataContext as SigningViewModel;
                    vm?.Reloaded ();
                    doubleTapped = true;
                    _signingViewIsShown = true;
                    signingView.StartWith (editableView.ImageView.Image);
                    vc.Add (signingView);
                };
            };

            vc.Add (signingView);
            _signingViewIsShown = true;
        }

        private bool _textViewIsShown;

        public void ShowTextView ()
        {
            if (_textViewIsShown) return;
            var vc = ((UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController).VisibleViewController;
            var dialogView = ViewFactory.Create<DialogView> ();
            dialogView.Frame = Frame;
            EditableView editableView = null;
            var reopened = false;

            dialogView.CancelAction = () => {
                _textViewIsShown = false;
            };

            dialogView.OkAction = () => {
                _textViewIsShown = false;
                var signature = dialogView.GetImageOfText ();
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
                editableView = new EditableView (position);
                editableView.SetImage (signature.Image);
                if (previousRect != CGRect.Empty) {
                    editableView.Frame = previousRect;
                    editableView.UpdateImageAndLayer (previousRect.Size);
                }
                Add (editableView);

                editableView.OnDoubleTap = () => {
                    var vm = dialogView.DataContext as SigningViewModel;
                    vm?.Reloaded ();
                    reopened = true;
                    _textViewIsShown = true;
                    dialogView.StartWith (signature.Text);
                    vc.Add (dialogView);
                };
            };

            vc.Add (dialogView);
            _textViewIsShown = true;
        }
    }
}
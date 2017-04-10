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
                var previousRect = CGRect.Empty;
                if (doubleTapped) {
                    if (editableView != null)
                    {
                        previousRect = editableView.Frame;
                        editableView.Remove ();
                    }
                    doubleTapped = false;
                }
                _signingViewIsShown = false;
                var signature = signingView.GetSignature ();
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
                    vm?.Reloaded();
                    doubleTapped = true;
                    _signingViewIsShown = true;
                    signingView.StartWith (editableView.ImageView.Image);
                    vc.Add (signingView);
                };
            };

            vc.Add (signingView);
            _signingViewIsShown = true;
        }
    }
}
using System;
using UIKit;
using CoreGraphics;
using iSign.Core;
using MvvmCross.Plugins.Messenger;
using MvvmCross.Platform;
using MvvmCross.Core.ViewModels;

namespace iSign
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
            var signingView = new SigningView (Frame);
            signingView.DataContext = context;
            signingView.CancelAction = () => _signingViewIsShown = false;
            signingView.OkAction = () => {
                _signingViewIsShown = false;
                var signature = signingView.GetSignature ();
                var center = this.GetCenter (signature.Image.Size, ContentOffset);
                var editableView = new EditableView (new CGRect (center, signature.Image.Size));
                editableView.SetImage (signature);
                Add (editableView);
            };
            var vc = ((UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController).VisibleViewController;
            vc.Add (signingView);
            _signingViewIsShown = true;
        }
    }
}
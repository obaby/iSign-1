using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using iSign.Core;
using MvvmCross.Plugins.Messenger;
using MvvmCross.Platform;
using System.Linq;

namespace iSign
{
    public partial class TouchableScrollView : UIScrollView
    {
        private enum Modes
        {
            Editing,
            AddingLabel,
            Done
        }
        private Modes Mode { get; set; }

        private int _incrementalIds;
        private IMvxMessenger Messenger { get; }
        private MvxSubscriptionToken ViewActivatedToken { get; }
        private MvxSubscriptionToken UndoToken { get; }
        public TouchableScrollView (IntPtr handle) : base (handle)
        {
            Mode = Modes.Done;
            SetupGestures ();
            Messenger = Mvx.Resolve <IMvxMessenger> ();
            ViewActivatedToken = Messenger.Subscribe<ViewActivatedMessage> (HandleAction);
        }

        public override void LayoutSubviews ()
        {
            base.LayoutSubviews ();
            PanGestureRecognizer.MinimumNumberOfTouches = 2;
        }

        public event EventHandler FinishedAddingView;
        private void OnFinishedAddingView ()
        {
            FinishedAddingView?.Invoke (this, EventArgs.Empty);
        }

        private void SetupGestures ()
        {
            var tapGesture = new UITapGestureRecognizer (ScrollViewTouched);
            AddGestureRecognizer (tapGesture);
        }

        private void ScrollViewTouched (UITapGestureRecognizer tapInfo)
        {
            if (Mode == Modes.Done) return;
            var location = tapInfo.LocationInView (this);


           // Todo : Show Window
            Mode = Modes.Done;
            OnFinishedAddingView ();
        }
        private bool _signingViewIsShown;
        public void ShowSigningView ()
        {
            if (_signingViewIsShown) return;
            var signingView = new SigningView (Frame);
            signingView.CancelAction = () => _signingViewIsShown = false;
            signingView.OkAction = () => {
                _signingViewIsShown = false;
                var signature = signingView.GetSignature ();
                var center = this.GetCenter (signature.Size, ContentOffset);
                var editableView = new EditableView (new CGRect (center, signature.Size));
                editableView.SetImage (signature);
                Add (editableView);
            };
            var vc = ((UINavigationController)UIApplication.SharedApplication.KeyWindow.RootViewController).VisibleViewController;
            vc.Add (signingView);
            _signingViewIsShown = true;
        }

        public void SetToEditMode ()
        {
            Mode = Modes.Editing;
        }

        public void EndEditMode ()
        {
            Mode = Modes.Done;
        }

        internal void Clear ()
        {
            //todo; clear views
        }

        public void EndUpdate (int excludingId = -1)
        {
            /*foreach (var view in AddedViews) {
                if (view.Id != excludingId)
                    view.EndUpdate ();
            }*/
        }

        void HandleAction (ViewActivatedMessage message)
        {
            EndUpdate ();
        }
    }
}
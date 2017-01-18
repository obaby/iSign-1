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
        private string Text { get; set; }
        private List<EditableView> AddedViews { get; }
        private PaletteView PaletteView { get; }
        private EditableView CurrentView { get; set;}
        private UIColor CurrentColor { get; set;}
        private int _incrementalIds;
        private IMvxMessenger Messenger { get; }
        private MvxSubscriptionToken ViewActivatedToken { get; }
        private MvxSubscriptionToken UndoToken { get; }
        public TouchableScrollView (IntPtr handle) : base (handle)
        {
            Mode = Modes.Done;
            SetupGestures ();
            AddedViews = new List<EditableView> ();
            PaletteView = new PaletteView ();
            Messenger = Mvx.Resolve <IMvxMessenger> ();
            ViewActivatedToken = Messenger.Subscribe<ViewActivatedMessage> (HandleAction);
            UndoToken = Messenger.Subscribe<UndoMessage>(UndoAction);
        }
        private PaletteViewModel _paletteContext;
        public PaletteViewModel PaletteContext {
            get {
                return _paletteContext;
            }
            set {
                _paletteContext = value;
                PaletteView.DataContext = value;
                _paletteContext.PropertyChanged -= _paletteContext_PropertyChanged;
                _paletteContext.PropertyChanged += _paletteContext_PropertyChanged;
            }
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
            CurrentView = new EditableView (new CGRect (location.X, location.Y, 100, 100)) {
                Id = _incrementalIds,
            };
            if (CurrentColor != null) CurrentView.DrawColor = CurrentColor;
            if (Mode == Modes.AddingLabel) {
                var label = new UILabel (new CGRect (0, 0, 100, 10)) {
                    Text = Text
                };
                CurrentView.Add (label);
                label.SizeToFit ();
                CurrentView.Frame = new CGRect (CurrentView.Frame.Location, label.Frame.Size);
            }
            _incrementalIds++;
            AddedViews.Add (CurrentView);
            Add (CurrentView);
            ShowPalette ();
            Mode = Modes.Done;
            OnFinishedAddingView ();
        }

        public void SetToEditMode ()
        {
            Mode = Modes.Editing;
        }

        public void EndEditMode ()
        {
            Mode = Modes.Done;
        }

        public void AddLabel (string text)
        {
            Mode = Modes.AddingLabel;
            Text = text;
        }

        internal void Clear ()
        {
            foreach (var view in AddedViews) {
                view.RemoveFromSuperview ();
            }
            AddedViews.Clear ();
        }

        public void EndUpdate (int excludingId = -1)
        {
            foreach (var view in AddedViews) {
                if (view.Id != excludingId)
                    view.EndUpdate ();
            }
        }

        private void ShowPalette ()
        {
            PaletteView.Frame = new CGRect (Frame.X, Frame.Height, Frame.Width, 50);
            PaletteView.Layout ();
            Animate (0.5, 0.2, UIViewAnimationOptions.CurveLinear, () =>
                     Superview.Add (PaletteView), null);
        }

        void _paletteContext_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof (PaletteContext.SelectedColor)) {
                CurrentColor = PaletteContext.SelectedColor.Color.ToUIColor ();
                if (CurrentView != null)
                    CurrentView.DrawColor = CurrentColor;
            }
        }

        void HandleAction (ViewActivatedMessage message)
        {
            EndUpdate ();
            CurrentView = AddedViews.First (x => x.Id == message.ViewId);
            PaletteContext.SetSelectedColor(CurrentView.DrawColor.ToPCLColor());
        }

        void UndoAction (UndoMessage message)
        {
            CurrentView.Undo ();
            PaletteView.UpdateUndo (CurrentView.CanUndo);
        }
    }
}
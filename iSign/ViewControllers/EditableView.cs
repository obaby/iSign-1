﻿using System;
using System.Collections.Generic;
using CoreGraphics;
using iSign.Extensions;
using iSign.Views;
using UIKit;

namespace iSign.ViewControllers
{
    public sealed class EditableView : UIView
    {
        public int Id { get; set; }
        public UIImageView ImageView { get; private set; }
        private UIPanGestureRecognizer DragGesture { get; }
        private UITapGestureRecognizer DoubleTapTwoFigersGesture { get; }
        private UITapGestureRecognizer DoubleTapGesture { get; }
        private UIPinchGestureRecognizer PinchGesture { get; }
        private UILongPressGestureRecognizer LongPressGesture { get; }
        private CircleButton DeleteButton { get; }
        private CircleButton EditSignatureButton { get; set; }
        private CircleButton OkButton { get; set; }

        private UIColor DeleteButtonColor => UIColor.FromRGB (195, 13, 13);
        private UIColor EditSignatureButtonColor => UIColor.FromRGB (42, 56, 93);
       // private UIColor OkButtonColor => UIColor.FromRGB (36, 212, 255);
        private UIColor OkButtonColor => UIColor.FromRGB (50, 122, 56);
        private UIColor BorderColor => UIColor.FromRGB (119, 119, 119);

        public EditableView (CGRect rect) : base (rect)
        {
            DeleteButton = new CircleButton {
                BackgroundColor = DeleteButtonColor
            };
            OkButton = new CircleButton {
                BackgroundColor = OkButtonColor
            };
            EditSignatureButton = new CircleButton {
                BackgroundColor = EditSignatureButtonColor
            };

            DeleteButton.TouchUpInside += DeleteButton_TouchUpInside;
            OkButton.TouchUpInside += OkButton_TouchUpInside;
            EditSignatureButton.TouchUpInside += EditSignatureButton_TouchUpInside;

            DragGesture = new UIPanGestureRecognizer (ViewDragged) {
                MinimumNumberOfTouches = 1
            };

            LongPressGesture = new UILongPressGestureRecognizer (ViewLongPressed);

            DoubleTapTwoFigersGesture = new UITapGestureRecognizer (ViewDoubleTappedWith2Fingers) {
                NumberOfTapsRequired = 2,
                NumberOfTouchesRequired = 2
            };

            PinchGesture = new UIPinchGestureRecognizer (ViewResized);

            DoubleTapGesture = new UITapGestureRecognizer (ViewDoubleTapped) {
                NumberOfTapsRequired = 2
            };

            AddGestureRecognizer (DoubleTapGesture);
            AddGestureRecognizer (DragGesture);
            AddGestureRecognizer (DoubleTapTwoFigersGesture);
            AddGestureRecognizer (LongPressGesture);
            AddGestureRecognizer (PinchGesture);
            BackgroundColor = UIColor.Clear;
            ViewStateFlow = new List<ViewState> { ViewState.Done, ViewState.Editing };

            UpdateLayer ();
        }

        public UIViewController ParentViewController { get; set; }
        public IImageView DialogView { get; set; }

        public override void LayoutSubviews ()
        {
            base.LayoutSubviews ();

            OkButton.NextToThisView (this, 5);
            if (OkButton.Superview == null)
                Superview.Add (OkButton);
            
            DeleteButton.NextToThisView (this, 5)
                        .BelowThisView (OkButton, 10);
            if (DeleteButton.Superview == null)
                Superview.Add (DeleteButton);

            EditSignatureButton.NextToThisView (this, 5)
                               .BelowThisView (DeleteButton, 10);
            if (EditSignatureButton.Superview == null)
                Superview.Add (EditSignatureButton);
        }

        private CGPoint _viewCoordinate;

        private void ViewDragged (UIPanGestureRecognizer panInfo)
        {
            if (panInfo.State == UIGestureRecognizerState.Began) {
                _viewCoordinate = panInfo.LocationInView (panInfo.View);
            }
            var newCoord = panInfo.LocationInView (panInfo.View);
            var deltaWidthDrag = newCoord.X - _viewCoordinate.X;
            var deltaHeightDrag = newCoord.Y - _viewCoordinate.Y;


            var width = Frame.Size.Width;
            var height = Frame.Size.Height;

            var xMax = Superview.Frame.Width;
            var yMax = Superview.Frame.Height;
            var view = Superview as UIScrollView;
            if (view != null) {
                xMax = view.ContentSize.Width;
                yMax = view.ContentSize.Height;
            }

            var x = Math.Max (0, Math.Min (xMax - panInfo.View.Frame.Width, panInfo.View.Frame.X + deltaWidthDrag));
            var y = Math.Max (0, Math.Min (yMax - panInfo.View.Frame.Height, panInfo.View.Frame.Y + deltaHeightDrag));

            panInfo.View.Frame = new CGRect (x, y,
                width,
                height);
            LayoutSubviews ();
        }
        CGAffineTransform _previousTransform;
        public CGSize LimitSize { get; set; } = new CGSize (100, 100);
        private void ViewResized (UIPinchGestureRecognizer pinchInfo)
        {
            var scale = pinchInfo.Scale;
            var transform = CGAffineTransform.MakeIdentity ();
            transform.Scale (scale, scale);
            ImageView.Transform = transform;
            if (ImageView.Frame.Size.IsSmallerThan (LimitSize)) {
                ImageView.Transform = _previousTransform;
                return;
            }
            _previousTransform = transform;
            var transformedBounds = CGAffineTransform.CGRectApplyAffineTransform (ImageView.Bounds, transform);
            var location = new CGPoint (Frame.X + ImageView.Frame.X, Frame.Y + ImageView.Frame.Y);
            Frame = new CGRect (location, transformedBounds.Size);
            this.UpdateLayersFrame ();
        }

        void EditSignatureButton_TouchUpInside (object sender, EventArgs e)
        {
            if (OnDoubleTap != null) {
                OnDoubleTap ();
            }
        }

        private void ViewDoubleTapped (UITapGestureRecognizer tapInfo)
        {
            if (OnDoubleTap != null) {
                OnDoubleTap ();
            }
        }

        public Action OnDoubleTap { get; set; }

        private void DeleteButton_TouchUpInside (object sender, EventArgs args)
        {
            Remove ();
        }

        private void ViewDoubleTappedWith2Fingers (UITapGestureRecognizer tapInfo)
        {
            Remove ();
        }

        public void Remove ()
        {
            RemoveFromSuperview ();
            RemoveGestureRecognizer (DragGesture);
            RemoveGestureRecognizer (DoubleTapGesture);
            RemoveGestureRecognizer (DoubleTapTwoFigersGesture);
            RemoveGestureRecognizer (LongPressGesture);
            RemoveGestureRecognizer (PinchGesture);

            DeleteButton.TouchUpInside -= DeleteButton_TouchUpInside;
            OkButton.TouchUpInside -= OkButton_TouchUpInside;
            EditSignatureButton.TouchUpInside -= EditSignatureButton_TouchUpInside;

            DeleteButton.RemoveFromSuperview ();
            OkButton.RemoveFromSuperview ();
            EditSignatureButton.RemoveFromSuperview ();

            Dispose ();
        }

        void OkButton_TouchUpInside (object sender, EventArgs e)
        {
            State = ViewState.Done;
            UpdateLayer ();
        }

        private void ViewLongPressed (UILongPressGestureRecognizer tapInfo)
        {
            if (tapInfo.State == UIGestureRecognizerState.Ended) {
                State = NextState ();
                UpdateLayer ();
            }
        }

        private ViewState NextState ()
        {
            var index = ViewStateFlow.IndexOf (State);
            var newIndex = (index + 1) % ViewStateFlow.Count;
            return ViewStateFlow [newIndex];
        }

        private List<ViewState> ViewStateFlow { get; }


        private void UpdateLayer ()
        {
            switch (State) {
            case ViewState.Done:
                this.UnantMarch ();
                ChangeGestureEnablity (false);
                ChangeButtonsVisibility (false);
                break;
            case ViewState.Editing:
                this.UnantMarch ();
                this.AntMarch (BorderColor);
                ChangeButtonsVisibility (true);
                ChangeGestureEnablity (true);
                break;
            }
        }

        private void ChangeButtonsVisibility (bool visible)
        {
            DeleteButton.Hidden = !visible;
            OkButton.Hidden = !visible;
            EditSignatureButton.Hidden = !visible;
        }

        private void ChangeGestureEnablity (bool enabled)
        {
            DragGesture.Enabled = enabled;
            PinchGesture.Enabled = enabled;
        }

        private ViewState State { get; set; }
        private enum ViewState
        {
            Editing,
            Done
        }

        public void SetImage (UIImage image)
        {
            ImageView = new UIImageView (new CGRect (CGPoint.Empty, image.Size));
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            Frame = new CGRect (Frame.Location, image.Size);
            ImageView.Image = image;
            Add (ImageView);
        }

        public void UpdateImageAndLayer (CGSize size)
        {
            if (ImageView != null)
                ImageView.Frame = new CGRect (ImageView.Frame.Location, size);
            this.UpdateLayersFrame ();
        }
    }
}

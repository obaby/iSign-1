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
    }
}
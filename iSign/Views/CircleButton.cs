using CoreGraphics;
using UIKit;

namespace iSign.Views
{
    public class CircleButton : UIButton
    {
        public CircleButton ()
        {
            Frame = new CGRect (0, 0, 20, 20);
        }

        public override void LayoutSubviews ()
        {
            base.LayoutSubviews ();
            //Layer.CornerRadius = Frame.Width / 2;
        }
    }
}

using CoreAnimation;
using Foundation;
using UIKit;

namespace iSign.Extensions
{
    public class AntmarchLayer : CAShapeLayer
    {
        private UIView View { get; }
        public AntmarchLayer (UIColor color, UIView view )
        {
            StrokeColor = color.CGColor;
            View = view;
        }

        public override void LayoutSublayers ()
        {
            Frame = View.Bounds;
            LineWidth = 2;
            FillColor = UIColor.Clear.CGColor;
            LineJoin = JoinRound;
            LineDashPattern = new [] { NSNumber.FromNInt (2), NSNumber.FromNInt (5), NSNumber.FromNInt (5) };

            var path = UIBezierPath.FromRoundedRect (Bounds, 15);
            Path = path.CGPath;

            if (AnimationForKey ("linePhase") != null) {
                RemoveAnimation ("linePhase");
            } else {
                var animation = CABasicAnimation.FromKeyPath ("lineDashPhase");
                animation.SetFrom (new NSNumber (300));
                animation.SetTo (new NSNumber (0));
                animation.Duration = 10f;
                animation.RepeatCount = 10000;
                AddAnimation (animation, "linePhase");
            }
            base.LayoutSublayers ();
        }
    }
}

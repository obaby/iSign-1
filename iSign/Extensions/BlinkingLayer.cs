using System;
using CoreAnimation;
using Foundation;
using UIKit;

namespace iSign.Extensions
{
    public class BlinkingLayer : CAShapeLayer
    {
        public override void LayoutSublayers ()
        {
            LineWidth = 2;
            FillColor = UIColor.Clear.CGColor;
            LineJoin = JoinRound;
            LineDashPattern = new [] { NSNumber.FromNInt (2), NSNumber.FromNInt (5), NSNumber.FromNInt (5) };

            var path = UIBezierPath.FromRoundedRect (Bounds, 15);
            Path = path.CGPath;

            var flash = CABasicAnimation.FromKeyPath("opacity");

            flash.SetFrom (NSNumber.FromFloat (0));
            flash.SetTo (NSNumber.FromFloat (1));
            flash.Duration = 0.75;        
            flash.RepeatCount = 10000;

            AddAnimation (flash, "flashAnimation");
            base.LayoutSublayers ();
        }
    }
}

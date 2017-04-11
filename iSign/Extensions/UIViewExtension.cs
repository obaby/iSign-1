using System;
using System.Linq;
using CoreGraphics;
using UIKit;

namespace iSign.Extensions
{
    public static class UIViewExtension
    {
        public static void AntMarch (this UIView self, UIColor color)
        {
            var antMarchayer = new AntmarchLayer (color, self);
            antMarchayer.Bounds = self.Bounds;
            self.Layer.AddSublayer (antMarchayer);
        }

        public static void UnantMarch (this UIView self)
        {
            if (self?.Layer?.Sublayers == null) return;
            foreach (var layer in self.Layer.Sublayers.Where (x => x is AntmarchLayer)) {
                layer.RemoveFromSuperLayer ();
            }
        }

        public static void Blink (this UIView self, UIColor color)
        {
            var blinkLayer = new BlinkingLayer (color, self);
            blinkLayer.Bounds = self.Bounds;
            self.Layer.AddSublayer (blinkLayer);
        }

        public static void Unblink (this UIView self)
        {
            if (self?.Layer?.Sublayers == null) return;
            foreach (var layer in self.Layer.Sublayers.Where (x => x is BlinkingLayer)) {
                layer.RemoveFromSuperLayer ();
            }
        }

        public static void UpdateLayersFrame (this UIView self)
        {
            foreach (var layer in self.Layer.Sublayers) {
                layer.Frame = self.Bounds;
            }
        }

        public static CGPoint GetCenter (this UIView self, CGRect rect)
        {
            var x = (self.Frame.Width - rect.Width) / 2;
            var y = (self.Frame.Height - rect.Height) / 2;
            return new CGPoint (x, y);
        }

        public static CGPoint GetCenter (this UIView self, CGSize size)
        {
            var x = (self.Frame.Width - size.Width) / 2;
            var y = (self.Frame.Height - size.Height) / 2;
            return new CGPoint (x, y);
        }

        public static CGPoint GetCenter (this UIView self, CGSize size, CGPoint offset)
        {
            var x = (self.Frame.Width - size.Width) / 2;
            var y = (self.Frame.Height - size.Height) / 2;
            return new CGPoint (x + offset.X, y + offset.Y);
        }

        public static UIView AboveThisView (this UIView self, UIView relativeView, nfloat delta)
        {
            self.Frame = new CGRect (new CGPoint (self.Frame.X, relativeView.Frame.Y - delta), self.Frame.Size);
            return self;
        }

        public static UIView AtTheBeginingThisView (this UIView self, UIView relativeView, nfloat delta)
        {
	         self.Frame = new CGRect (new CGPoint (relativeView.Frame.X + delta, relativeView.Frame.Y), self.Frame.Size);
            return self;
        }

        public static UIView AtTheEndThisView (this UIView self, UIView relativeView, nfloat delta)
        {
        	self.Frame = new CGRect (new CGPoint (relativeView.Frame.X + relativeView.Frame.Width - self.Frame.Width - delta, relativeView.Frame.Y), self.Frame.Size);
        	return self;
        }

        public static UIView NextToThisView (this UIView self, UIView relativeView, nfloat delta)
        {
        	self.Frame = new CGRect (new CGPoint (relativeView.Frame.X + relativeView.Frame.Width + delta, relativeView.Frame.Y), self.Frame.Size);
        	return self;
        }

        public static UIView BeforeThisView (this UIView self, UIView relativeView, nfloat delta)
        {
        	self.Frame = new CGRect (new CGPoint (relativeView.Frame.X - self.Frame.Width - delta, relativeView.Frame.Y), self.Frame.Size);
        	return self;
        }
    }
}
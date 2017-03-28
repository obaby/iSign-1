using System.Linq;
using CoreAnimation;
using Foundation;
using iSign.Extensions;
using UIKit;
namespace iSign
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
            foreach (var layer in self.Layer.Sublayers.Where (x => x is AntmarchLayer)) {
                layer.RemoveFromSuperLayer ();
            }
        }

        public static void Blink (this UIView self, UIColor color)
        {
            var blinkLayer = new BlinkingLayer ();
            blinkLayer.StrokeColor = color.CGColor;
            blinkLayer.Bounds = self.Bounds;
            self.Layer.AddSublayer (blinkLayer);
        }

        public static void Unblink (this UIView self)
        {
            foreach (var layer in self.Layer.Sublayers.Where (x => x is BlinkingLayer)) {
                layer.RemoveFromSuperLayer ();
            }
        }

        public static void UpdateLayersFrame (this UIView self)
        {
            foreach (var layer in self.Layer.Sublayers) {
                layer.Frame = self.Frame;
            }
        }
    }
}
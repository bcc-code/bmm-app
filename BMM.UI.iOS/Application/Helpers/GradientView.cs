using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace BMM.UI.iOS.Helpers
{
    /// <summary>
    /// Wrapping the gradient layer into an UIView allows us to resize the gradient with Auto Layout
    /// <see href="https://stackoverflow.com/questions/17555986/cagradientlayer-not-resizing-nicely-tearing-on-rotation"> More info </see>
    /// </summary>
    public class GradientView: UIView
    {
        [Export ("layerClass")]
        public static Class LayerClass () {
            return new Class (typeof (CAGradientLayer));
        }
    }

    public static class GradientViewExtensions
    {
        public static void SetGradientBackground(this UIView view, CGColor[] colors, NSNumber[] locations)
        {
            view.BackgroundColor = UIColor.Clear;
            var gradientView = new GradientView
            {
                Frame = view.Bounds,
                AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth
            };
            if (gradientView.Layer is CAGradientLayer layer)
            {
                layer.Colors = colors;
                layer.Locations = locations;
                layer.CornerRadius = view.Layer.CornerRadius;
            }

            view.InsertSubview(gradientView, new IntPtr(0));
        }
    }
}
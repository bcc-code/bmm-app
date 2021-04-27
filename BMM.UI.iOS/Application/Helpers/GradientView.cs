using CoreAnimation;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace BMM.UI.iOS.Helpers
{
    //https://stackoverflow.com/questions/17555986/cagradientlayer-not-resizing-nicely-tearing-on-rotation
    public class GradientView: UIView
    {
        [Export ("layerClass")]
        public static Class LayerClass () {
            return new Class (typeof (CAGradientLayer));
        }
    }
}
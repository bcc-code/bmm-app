using CoreGraphics;
using UIKit;

namespace BMM.UI.iOS.Utils
{
    public class ImageUtils
    {
        public static UIImage MakeCircle(CGSize size, UIColor backgroundColor)
        {
            UIGraphics.BeginImageContextWithOptions(size, false, 0.0f);
            var context = UIGraphics.GetCurrentContext();
            context?.SetFillColor(backgroundColor.CGColor);
            context?.SetStrokeColor(UIColor.Clear.CGColor);
            var bounds = new CGRect(CGPoint.Empty, size);
            context?.AddEllipseInRect(bounds);
            context?.DrawPath(CGPathDrawingMode.Fill);
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return image;
        }
    }
}
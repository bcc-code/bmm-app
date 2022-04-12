using CoreGraphics;
using UIKit;

namespace BMM.UI.iOS.Utils
{
    public class ImageUtils
    {
        public static UIImage MakePlayerThumbCircle(CGSize size, UIColor backgroundColor)
        {
            var sizeWithClickableArea = new CGSize(size.Width * 1.5, size.Height * 3);
            
            UIGraphics.BeginImageContextWithOptions(sizeWithClickableArea, false, 0.0f);
            var context = UIGraphics.GetCurrentContext();
            context?.SetFillColor(backgroundColor.CGColor);
            context?.SetStrokeColor(UIColor.Clear.CGColor);
            context?.StrokeRect(new CGRect(CGPoint.Empty, sizeWithClickableArea));
            context?.AddEllipseInRect(new CGRect((sizeWithClickableArea.Width - size.Width) / 2, (sizeWithClickableArea.Height - size.Height) / 2, size.Width, size.Height));
            context?.DrawPath(CGPathDrawingMode.Fill);
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return image;
        }
    }
}
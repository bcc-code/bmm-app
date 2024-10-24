namespace BMM.UI.iOS.Extensions;

public static class UIImageExtensions
{
    public static UIImage WithBadge(this UIImage image, UIColor badgeColor = null)
    {
        badgeColor = badgeColor ?? UIColor.Red;

        UIGraphics.BeginImageContextWithOptions(image.Size, false, image.CurrentScale);
        try
        {
            image.Draw(CGPoint.Empty);

            var badgeSize = new CGSize(8, 8);
            var badgeOrigin = new CGPoint(0, 0);
            var badgeRect = new CGRect(badgeOrigin, badgeSize);

            var context = UIGraphics.GetCurrentContext();
            context.SetFillColor(badgeColor.CGColor);
            context.FillEllipseInRect(badgeRect);

            var resultImage = UIGraphics.GetImageFromCurrentImageContext();

            return resultImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
        }
        finally
        {
            UIGraphics.EndImageContext();
        }
    }
}
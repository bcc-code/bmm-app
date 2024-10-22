namespace BMM.UI.iOS.Extensions;

public static class UIImageExtensions
{
    public static UIImage WithBadge(this UIImage image, UIColor badgeColor = null)
    {
        // Default badge color to red if none is provided
        badgeColor = badgeColor ?? UIColor.Red;

        // Begin an image context with the size of the original image
        UIGraphics.BeginImageContextWithOptions(image.Size, false, image.CurrentScale);
        try
        {
            // Draw the original image without altering its color
            image.Draw(CGPoint.Empty);

            // Define badge size and position (you can adjust badgeSize to your needs)
            var badgeSize = new CGSize(8, 8);  // Badge size, adjust as necessary
            var badgeOrigin = new CGPoint(0, 0);  // Top-right corner
            var badgeRect = new CGRect(badgeOrigin, badgeSize);

            // Draw the badge circle
            var context = UIGraphics.GetCurrentContext();
            context.SetFillColor(badgeColor.CGColor);
            context.FillEllipseInRect(badgeRect);

            // Get the image from the current context
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();

            return resultImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);  // Keep the original rendering mode
        }
        finally
        {
            // End the image context
            UIGraphics.EndImageContext();
        }
    }
}
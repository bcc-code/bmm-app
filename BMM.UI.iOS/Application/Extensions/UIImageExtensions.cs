using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS.Extensions;

public static class UIImageExtensions
{
    private const int BadgeSize = 8;
    
    public static UIImage WithBadge(this UIImage image, UIColor iconColor)
    {
        var size = image.Size;
        var renderer = new UIGraphicsImageRenderer(size);

        return renderer.CreateImage(_ =>
        {
            var tintedImage = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            iconColor.GetResolvedColorSafe().SetFill();
            tintedImage.Draw(CGPoint.Empty);
            
            var badgeSize = new CGSize(BadgeSize, BadgeSize);
            var badgeOrigin = new CGPoint(0, 0);
            var badgeRect = new CGRect(badgeOrigin, badgeSize);
            
            var badgePath = UIBezierPath.FromOval(badgeRect);
            AppColors.RadioColor.SetFill();
            badgePath.Fill();
        })
        .ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
    }
    
    public static UIImage WithPadding(this UIImage image, float padding)
    {
        if (image == null)
            return null;

        var newSize = new CGSize(image.Size.Width + padding * 2, image.Size.Height + padding * 2);

        var format = UIGraphicsImageRendererFormat.DefaultFormat;
        format.Scale = image.CurrentScale;
        format.Opaque = false;

        var renderer = new UIGraphicsImageRenderer(newSize, format);

        var paddedImage = renderer.CreateImage((context) =>
        {
            context.CGContext.ClearRect(new CGRect(0, 0, newSize.Width, newSize.Height));
            image.Draw(new CGRect(padding, padding, image.Size.Width, image.Size.Height));
        });

        return paddedImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
    }
}
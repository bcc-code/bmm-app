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
            iconColor.SetFill();
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
}
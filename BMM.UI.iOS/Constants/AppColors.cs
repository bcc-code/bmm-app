using CoreGraphics;
using UIKit;

namespace BMM.UI.iOS.Constants
{
    public static class AppColors
    {
        public static UIColor BackgroundPrimaryColor => UIColor.FromName(nameof(BackgroundPrimaryColor));
        public static UIColor BackgroundSecondaryColor => UIColor.FromName(nameof(BackgroundSecondaryColor));
        public static UIColor LabelPrimaryColor => UIColor.FromName(nameof(LabelPrimaryColor));
        public static UIColor LabelPrimaryColorReverted => UIColor.FromName(nameof(LabelPrimaryColorReverted));
        public static UIColor LabelSecondaryColor => UIColor.FromName(nameof(LabelSecondaryColor));
        public static UIColor LabelTertiaryColor => UIColor.FromName(nameof(LabelTertiaryColor));
        public static UIColor NotificationColor => UIColor.FromName(nameof(NotificationColor));
        public static UIColor OnTintOneColor => UIColor.FromName(nameof(OnTintOneColor));
        public static UIColor OnTintThreeColor => UIColor.FromName(nameof(OnTintThreeColor));
        public static UIColor OnTintTwoColor => UIColor.FromName(nameof(OnTintTwoColor));
        public static UIColor PlaceholderColor => UIColor.FromName(nameof(PlaceholderColor));
        public static UIColor RadioColor => UIColor.FromName(nameof(RadioColor));
        public static UIColor TintColor => UIColor.FromName(nameof(TintColor));
        public static UIColor HighlightColor => UIColor.FromName(nameof(HighlightColor));

        public static readonly UIColor RefreshControlTintColor = UIColor.FromRGB(194, 239, 102);

        // Track
        public static readonly UIColor NewestTrackBackgroundColor = UIColor.White.ColorWithAlpha(0.2f);

        // Queue
        public static readonly UIColor QueueBackgroundColor = UIColor.FromRGB(12, 15, 20);
        public static readonly UIColor QueueBackgroundSelectedColor = UIColor.FromRGB(28, 36, 43);

        public static readonly UIColor SeparatorColor = UIColor.FromRGB(185, 195, 204).ColorWithAlpha(0.5f);
    }
}
using CoreGraphics;
using UIKit;

namespace BMM.UI.iOS.Constants
{
    public static class AppColors
    {
        public static readonly UIColor Paragraph2Color = UIColor.FromRGB(0.361f, 0.439f, 0.51f);
        public static readonly UIColor ColorPrimary = UIColor.FromRGB(131, 231, 107);
        public static readonly UIColor RefreshControlTintColor = UIColor.FromRGB(194, 239, 102);
        public static readonly UIColor TabBarUnselectedColor = UIColor.FromRGB(143,160,175);

        // Track
        public static readonly UIColor TrackTitleColor = UIColor.FromRGB(13, 19, 26);
        public static readonly UIColor TrackSubtitleColor = UIColor.FromRGB(92, 112, 130);
        public static readonly UIColor TrackMetaColor = UIColor.FromRGB(143, 160, 175);
        public static readonly UIColor NewestTrackBackgroundColor = UIColor.White.ColorWithAlpha(0.2f);

        // Queue
        public static readonly UIColor QueueBackgroundColor = UIColor.FromRGB(12, 15, 20);
        public static readonly UIColor QueueBackgroundSelectedColor = UIColor.FromRGB(28, 36, 43);

        // Streak
        public static readonly UIColor StreakColorBubbleBorderColor = UIColor.FromRGB(0.725f, 0.765f, 0.8f);
        public static readonly UIColor StreakBackGroundColor = UIColor.FromRGB(0.925f, 0.941f, 0.953f);

        // Player
        public static readonly UIColor PlayerBackgroundColor = UIColor.FromRGB(13, 19, 26);
        public static readonly CGColor[] TrackCoverGradientColors = {
            UIColor.FromRGB(0, 0, 0).ColorWithAlpha(0).CGColor,
            UIColor.FromRGB(3, 4, 6).ColorWithAlpha(.09f).CGColor,
            UIColor.FromRGB(9, 14, 19).ColorWithAlpha(.28f).CGColor,
            UIColor.FromRGB(13, 19, 26).ColorWithAlpha(.5f).CGColor
        };
    }
}
using System;
using UIKit;

namespace BMM.UI.iOS.Constants
{
    public static class AppTheme
    {
        public static readonly Lazy<TextTheme> Heading1 = new Lazy<TextTheme>(() => new TextTheme
        {
            Font = Typography.Header1.Value,
            Color = AppColors.TrackTitleColor
        });

        public static readonly Lazy<TextTheme> Heading2 = new Lazy<TextTheme>(() => new TextTheme
        {
            Font = Typography.Header2.Value,
            Color = AppColors.TrackTitleColor
        });

        public static readonly Lazy<TextTheme> Paragraph2 = new Lazy<TextTheme>(() => new TextTheme
        {
            Font = Typography.Paragraph2.Value,
            Color = AppColors.Paragraph2Color
        });

        public static readonly Lazy<TextTheme> Subtitle3 = new Lazy<TextTheme>(() => new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.TrackMetaColor
        });

        public static readonly Lazy<ButtonTheme> ButtonPrimary = new Lazy<ButtonTheme>(() => new StandardButtonTheme
        {
            TextTheme = new TextTheme {Font = Typography.Title1.Value, Color = UIColor.White},
            ButtonColor = AppColors.TrackTitleColor
        });

        public static readonly Lazy<ButtonTheme> ButtonSecondary = new Lazy<ButtonTheme>(() => new StandardButtonTheme
        {
            TextTheme = new TextTheme {Font = Typography.Title1.Value, Color = AppColors.TrackTitleColor},
            ButtonColor = AppColors.StreakBackGroundColor
        });

        public static readonly Lazy<ButtonTheme> ButtonTertiary = new Lazy<ButtonTheme>(() => new ButtonTheme
        {
            TextTheme = new TextTheme {Font = Typography.Title1.Value, Color = UIColor.White},
            ButtonColor = AppColors.TrackTitleColor,
            ContentEdgeInsets = new UIEdgeInsets(0, 34, 0, 34),
            HasRoundedCorners = true
        });
    }
}
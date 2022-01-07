using System;
using UIKit;

namespace BMM.UI.iOS.Constants
{
    public static class AppTheme
    {
        public static TextTheme Heading2 = new TextTheme
        {
            Font = Typography.Header2.Value,
            Color = AppColors.LabelPrimaryColor
        };

        public static TextTheme Title1 = new TextTheme
        {
            Font = Typography.Title1.Value,
            Color = AppColors.LabelPrimaryColor
        };

        public static TextTheme Title1Label1Reverted = new TextTheme
        {
            Font = Typography.Title1.Value,
            Color = AppColors.LabelPrimaryColorReverted
        };

        public static TextTheme Title2 = new TextTheme
        {
            Font = Typography.Title2.Value,
            Color = AppColors.LabelPrimaryColor
        };
        
        public static TextTheme Title3 = new TextTheme
        {
            Font = Typography.Title3.Value,
            Color = AppColors.LabelPrimaryColor
        };

        public static TextTheme Heading3 = new TextTheme
        {
            Font = Typography.Header3.Value,
            Color = AppColors.LabelPrimaryColor,
            MinimumFontSize = 12
        };

        public static TextTheme Paragraph1Label1 = new TextTheme
        {
            Font = Typography.Paragraph1.Value,
            Color = AppColors.LabelPrimaryColor
        };

        public static TextTheme Paragraph1Label2 = new TextTheme
        {
            Font = Typography.Paragraph1.Value,
            Color = AppColors.LabelSecondaryColor
        };

        public static TextTheme Paragraph2 = new TextTheme
        {
            Font = Typography.Paragraph2.Value,
            Color = AppColors.LabelSecondaryColor
        };

        public static TextTheme Subtitle1Label2 = new TextTheme
        {
            Font = Typography.Subtitle1.Value,
            Color = AppColors.LabelSecondaryColor
        };
        
        public static TextTheme Subtitle1Label1 = new TextTheme
        {
            Font = Typography.Subtitle1.Value,
            Color = AppColors.LabelPrimaryColor
        };

        public static TextTheme Subtitle2Label1 = new TextTheme
        {
            Font = Typography.Subtitle2.Value,
            Color = AppColors.LabelPrimaryColor
        };

        public static TextTheme Subtitle2Label2 => new TextTheme
        {
            Font = Typography.Subtitle2.Value,
            Color = AppColors.LabelSecondaryColor
        };

        public static TextTheme Subtitle2Label3 = new TextTheme
        {
            Font = Typography.Subtitle2.Value,
            Color = AppColors.LabelTertiaryColor
        };

        public static TextTheme Subtitle3Label1 = new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.LabelPrimaryColor
        };

        public static TextTheme Subtitle3Label2 => new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.LabelSecondaryColor
        };

        public static TextTheme Subtitle3Label3 = new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.LabelTertiaryColor
        };

        public static readonly Lazy<ButtonTheme> ButtonPrimary = new Lazy<ButtonTheme>(() => new StandardButtonTheme
        {
            TextTheme = Title1Label1Reverted,
            ButtonColor = AppColors.LabelPrimaryColor,
            IconTint = AppColors.LabelPrimaryColorReverted
        });

        public static readonly ButtonTheme ButtonSecondaryMedium = new StandardButtonTheme
        {
            TextTheme = Title1,
            ButtonColor = AppColors.BackgroundSecondaryColor,
            IconTint = AppColors.LabelPrimaryColor
        };

        public static readonly ButtonTheme ButtonSecondarySmall = new StandardButtonTheme
        {
            TextTheme = Title2,
            ButtonColor = AppColors.BackgroundSecondaryColor,
            IconTint = AppColors.LabelPrimaryColor
        };
        
        public static readonly ButtonTheme ButtonTertiaryMedium = new StandardButtonTheme
        {
            TextTheme = Title1,
            ButtonColor = UIColor.Clear,
            HasBorder = true,
            BorderColor = AppColors.PlaceholderColor
        };
    }
}
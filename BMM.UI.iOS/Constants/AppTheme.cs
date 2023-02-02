using System;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.NewMediaPlayer;
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
        
        public static TextTheme Title1OnColor1 = new TextTheme
        {
            Font = Typography.Title1.Value,
            Color = AppColors.OnColorOneColor
        };

        public static TextTheme Title2 = new TextTheme
        {
            Font = Typography.Title2.Value,
            Color = AppColors.LabelPrimaryColor
        };
        
        public static TextTheme Title2AutoSize = new TextTheme
        {
            Font = Typography.Title2.Value,
            Color = AppColors.LabelPrimaryColor,
            MinimumFontSize = 10
        };
        
        public static TextTheme Title3 = new TextTheme
        {
            Font = Typography.Title3.Value,
            Color = AppColors.LabelPrimaryColor
        };

        public static TextTheme Heading3 = new TextTheme
        {
            Font = Typography.Header3.Value,
            Color = AppColors.LabelPrimaryColor
        };
        
        public static TextTheme Heading3AutoSize = new TextTheme
        {
            Font = Typography.Header3.Value,
            Color = AppColors.LabelPrimaryColor,
            MinimumFontSize = 10
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
        
        public static TextTheme Paragraph1OnColor2 = new TextTheme
        {
            Font = Typography.Paragraph1.Value,
            Color = AppColors.OnColorTwoColor
        };

        public static TextTheme Paragraph2 = new TextTheme
        {
            Font = Typography.Paragraph2.Value,
            Color = AppColors.LabelSecondaryColor
        };
        
        public static TextTheme Paragraph3 = new TextTheme
        {
            Font = Typography.Paragraph3.Value,
            Color = AppColors.LabelTertiaryColor
        };

        public static TextTheme Subtitle1Label1 = new TextTheme
        {
            Font = Typography.Subtitle1.Value,
            Color = AppColors.LabelPrimaryColor
        };
        
        public static TextTheme Subtitle1Label2 = new TextTheme
        {
            Font = Typography.Subtitle1.Value,
            Color = AppColors.LabelSecondaryColor
        };
        
        public static TextTheme Subtitle1Label3 = new TextTheme
        {
            Font = Typography.Subtitle1.Value,
            Color = AppColors.LabelTertiaryColor
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
        
        public static TextTheme Subtitle2Label3AutoSize = new TextTheme
        {
            Font = Typography.Subtitle2.Value,
            Color = AppColors.LabelTertiaryColor,
            MinimumFontSize = 10
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
        
        public static TextTheme Subtitle3Label4 = new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.PlaceholderColor
        };
        
        public static TextTheme Subtitle3OnColor1 = new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.OnColorOneColor
        };
        
        public static TextTheme Subtitle3OnColor2 = new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.OnColorTwoColor
        };

        public static readonly ButtonTheme ButtonPrimary = new StandardButtonTheme
        {
            TextTheme = Title1Label1Reverted,
            ButtonColor = AppColors.LabelPrimaryColor,
            IconTint = AppColors.LabelPrimaryColorReverted
        };

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
            BorderColor = AppColors.PlaceholderColor,
            ImageEdgeInsets = UIEdgeInsets.Zero,
            ContentEdgeInsets = UIEdgeInsets.Zero
        };
        
        public static readonly ButtonTheme ButtonTertiaryMediumOnColorFive = new StandardButtonTheme
        {
            TextTheme = Title1,
            ButtonColor = UIColor.Clear,
            HasBorder = true,
            BorderColor = AppColors.OnColorFiveColor,
            ImageEdgeInsets = UIEdgeInsets.Zero,
            ContentEdgeInsets = UIEdgeInsets.Zero
        };

        #region CustomButtonThemes
        
        public static readonly ButtonTheme YearInReviewButton = new StandardButtonTheme
        {
            TextTheme = Title1,
            ButtonColor = AppColors.BackgroundPrimaryColor,
            IconTint = AppColors.LabelPrimaryColor,
            ImageEdgeInsets = new UIEdgeInsets(0, 6, 0, 16)
        };

        public static readonly ButtonTheme ButtonPrimaryBlack = new StandardButtonTheme
        {
            TextTheme = new TextTheme
            {
                Font = Typography.Title1.Value,
                Color = AppColors.LabelPrimaryColorReverted.GetResolvedColorSafe(UIUserInterfaceStyle.Light),
            },
            ButtonColor = AppColors.LabelPrimaryColor.GetResolvedColorSafe(UIUserInterfaceStyle.Light),
            IconTint = AppColors.LabelPrimaryColor.GetResolvedColorSafe(UIUserInterfaceStyle.Light)
        };
        
        public static readonly ButtonTheme ButtonPrimaryBlackAutoSize = new StandardButtonTheme
        {
            TextTheme = new TextTheme
            {
                Font = Typography.Title1.Value,
                Color = AppColors.LabelPrimaryColorReverted.GetResolvedColorSafe(UIUserInterfaceStyle.Light),
                MinimumFontSize = 10
            },
            ButtonColor = AppColors.LabelPrimaryColor.GetResolvedColorSafe(UIUserInterfaceStyle.Light),
            IconTint = AppColors.LabelPrimaryColor.GetResolvedColorSafe(UIUserInterfaceStyle.Light)
        };
        
        public static readonly ButtonTheme CancelSearchButton = new StandardButtonTheme
        {
            TextTheme = Subtitle1Label1,
            ButtonColor = UIColor.Clear,
            HasBorder = false
        };
        
        #endregion
    }
}
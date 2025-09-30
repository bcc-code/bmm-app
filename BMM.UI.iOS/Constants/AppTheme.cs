using System;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.NewMediaPlayer;
using UIKit;

namespace BMM.UI.iOS.Constants
{
    public static class AppTheme
    {
        public static TextTheme Heading1 = new TextTheme
        {
            Font = Typography.Header1.Value,
            Color = AppColors.LabelOneColor
        };
        
        public static TextTheme Heading2 = new TextTheme
        {
            Font = Typography.Header2.Value,
            Color = AppColors.LabelOneColor
        };

        public static TextTheme Title1 = new TextTheme
        {
            Font = Typography.Title1.Value,
            Color = AppColors.LabelOneColor
        };

        public static TextTheme Title1Label1Reverted = new TextTheme
        {
            Font = Typography.Title1.Value,
            Color = AppColors.LabelOneColorReverted
        };
        
        public static TextTheme Title1GlobalBlack1 = new TextTheme
        {
            Font = Typography.Title1.Value,
            Color = AppColors.GlobalBlackOneColor
        };

        public static TextTheme Title2 = new TextTheme
        {
            Font = Typography.Title2.Value,
            Color = AppColors.LabelOneColor
        };
        
        public static TextTheme Title2AutoSize = new TextTheme
        {
            Font = Typography.Title2.Value,
            Color = AppColors.LabelOneColor,
            MinimumFontSize = 10
        };
        
        public static TextTheme Title3 = new TextTheme
        {
            Font = Typography.Title3.Value,
            Color = AppColors.LabelOneColor
        };
        
        public static TextTheme Title3Reverted = new TextTheme
        {
            Font = Typography.Title3.Value,
            Color = AppColors.LabelOneColorReverted
        };

        public static TextTheme Heading3 = new TextTheme
        {
            Font = Typography.Header3.Value,
            Color = AppColors.LabelOneColor
        };
        
        public static TextTheme Heading3AutoSize = new TextTheme
        {
            Font = Typography.Header3.Value,
            Color = AppColors.LabelOneColor,
            MinimumFontSize = 10
        };

        public static TextTheme Paragraph1Label1 = new TextTheme
        {
            Font = Typography.Paragraph1.Value,
            Color = AppColors.LabelOneColor
        };

        public static TextTheme Paragraph1Label2 = new TextTheme
        {
            Font = Typography.Paragraph1.Value,
            Color = AppColors.LabelTwoColor
        };
        
        public static TextTheme Paragraph1GlobalBlack2 = new TextTheme
        {
            Font = Typography.Paragraph1.Value,
            Color = AppColors.GlobalBlackTwoColor
        };

        public static TextTheme Paragraph2 = new TextTheme
        {
            Font = Typography.Paragraph2.Value,
            Color = AppColors.LabelTwoColor
        };
        
        public static TextTheme Paragraph2Label3 = new TextTheme
        {
            Font = Typography.Paragraph2.Value,
            Color = AppColors.LabelThreeColor
        };
        
        public static TextTheme Paragraph3Label1 = new TextTheme
        {
            Font = Typography.Paragraph3.Value,
            Color = AppColors.LabelOneColor
        };
        
        public static TextTheme Paragraph3 = new TextTheme
        {
            Font = Typography.Paragraph3.Value,
            Color = AppColors.LabelThreeColor
        };

        public static TextTheme Subtitle1Label1 = new TextTheme
        {
            Font = Typography.Subtitle1.Value,
            Color = AppColors.LabelOneColor
        };
        
        public static TextTheme Subtitle1Label2 = new TextTheme
        {
            Font = Typography.Subtitle1.Value,
            Color = AppColors.LabelTwoColor
        };
        
        public static TextTheme Subtitle1Label3 = new TextTheme
        {
            Font = Typography.Subtitle1.Value,
            Color = AppColors.LabelThreeColor
        };

        public static TextTheme Subtitle1GlobalBlack1 = new TextTheme
        {
            Font = Typography.Subtitle1.Value,
            Color = AppColors.GlobalBlackOneColor
        };

        public static TextTheme Subtitle2Label1 => new TextTheme
        {
            Font = Typography.Subtitle2.Value,
            Color = AppColors.LabelOneColor
        };

        public static TextTheme Subtitle2Label2 => new TextTheme
        {
            Font = Typography.Subtitle2.Value,
            Color = AppColors.LabelTwoColor
        };

        public static TextTheme Subtitle2Label3 = new TextTheme
        {
            Font = Typography.Subtitle2.Value,
            Color = AppColors.LabelThreeColor
        };
        
        public static TextTheme Subtitle2Label3AutoSize = new TextTheme
        {
            Font = Typography.Subtitle2.Value,
            Color = AppColors.LabelThreeColor,
            MinimumFontSize = 10
        };

        public static TextTheme Subtitle3Label1 = new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.LabelOneColor
        };

        public static TextTheme Subtitle3Label2 => new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.LabelTwoColor
        };

        public static TextTheme Subtitle3Label3 = new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.LabelThreeColor
        };
        
        public static TextTheme Subtitle3Label4 = new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.PlaceholderColor
        };
        
        public static TextTheme Subtitle3GlobalBlack1 = new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.GlobalBlackOneColor
        };
        
        public static TextTheme Subtitle3GlobalBlack2 = new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.GlobalBlackTwoColor
        };
        
        public static TextTheme Subtitle3GlobalBlack3 = new TextTheme
        {
            Font = Typography.Subtitle3.Value,
            Color = AppColors.GlobalBlackThreeColor
        };

        public static readonly ButtonTheme ButtonPrimary = new StandardButtonTheme
        {
            TextTheme = Title1Label1Reverted,
            ButtonColor = AppColors.LabelOneColor,
            IconTint = AppColors.LabelOneColorReverted
        }; 
        
        public static readonly ButtonTheme ButtonPrimarySmall = new StandardButtonTheme
        {
            TextTheme = Title3Reverted,
            ButtonColor = AppColors.LabelOneColor,
            IconTint = AppColors.LabelOneColorReverted
        };

        public static readonly ButtonTheme ButtonSecondaryMedium = new StandardButtonTheme
        {
            TextTheme = Title1,
            ButtonColor = AppColors.BackgroundTwoColor,
            IconTint = AppColors.LabelOneColor
        };

        public static readonly ButtonTheme ButtonSecondarySmall = new StandardButtonTheme
        {
            TextTheme = Title2,
            ButtonColor = AppColors.BackgroundTwoColor,
            IconTint = AppColors.LabelOneColor
        };
        
        public static readonly ButtonTheme ButtonSecondarySmallDarker = new StandardButtonTheme
        {
            TextTheme = Title2,
            ButtonColor = AppColors.SeparatorColor,
            IconTint = AppColors.LabelOneColor
        };
        
        public static readonly ButtonTheme ButtonTertiaryLarge = new StandardButtonTheme
        {
            TextTheme = Title1,
            ButtonColor = UIColor.Clear,
            ImageEdgeInsets = UIEdgeInsets.Zero,
            ContentEdgeInsets = UIEdgeInsets.Zero
        };
        
        public static readonly ButtonTheme ButtonTertiaryMedium = new StandardButtonTheme
        {
            TextTheme = Title1,
            ButtonColor = UIColor.Clear,
            HasBorder = true,
            BorderColor = AppColors.SeparatorColor,
            ImageEdgeInsets = UIEdgeInsets.Zero,
            ContentEdgeInsets = UIEdgeInsets.Zero
        };
        
        public static readonly ButtonTheme ButtonTertiarySmall = new StandardButtonTheme
        {
            TextTheme = Title3,
            ButtonColor = UIColor.Clear,
            HasBorder = true,
            BorderColor = AppColors.SeparatorColor,
            ImageEdgeInsets = UIEdgeInsets.Zero,
            ContentEdgeInsets = new UIEdgeInsets(0, 12, 0, 12)
        };
        
        public static readonly ButtonTheme IconButtonTertiarySmall = new StandardButtonTheme
        {
            TextTheme = Title3,
            ButtonColor = UIColor.Clear,
            HasBorder = true,
            BorderColor = AppColors.SeparatorColor,
            IconTint = AppColors.LabelOneColor
        };
        
        public static readonly ButtonTheme ButtonTertiaryMediumSeparatorColorFive = new StandardButtonTheme
        {
            TextTheme = Title1,
            ButtonColor = UIColor.Clear,
            HasBorder = true,
            BorderColor = AppColors.GlobalBlackSeparatorColor,
            ImageEdgeInsets = UIEdgeInsets.Zero,
            ContentEdgeInsets = UIEdgeInsets.Zero
        };

        #region CustomButtonThemes
        
        public static readonly ButtonTheme YearInReviewButton = new StandardButtonTheme
        {
            TextTheme = Title1,
            ButtonColor = AppColors.BackgroundOneColor,
            IconTint = AppColors.LabelOneColor,
            ImageEdgeInsets = new UIEdgeInsets(0, 6, 0, 16)
        };

        public static readonly ButtonTheme ButtonTertiaryMediumAutoSize = new StandardButtonTheme
        {
            TextTheme = new TextTheme
            {
                Font = Typography.Title1.Value,
                Color = AppColors.LabelOneColor,
                MinimumFontSize = 8
            },
            ButtonColor = UIColor.Clear,
            HasBorder = true,
            BorderColor = AppColors.SeparatorColor,
            ImageEdgeInsets = UIEdgeInsets.Zero,
            ContentEdgeInsets = new UIEdgeInsets(0, 12, 0, 12)
        };
        
        public static readonly ButtonTheme ButtonPrimaryBlackAutoSize = new StandardButtonTheme
        {
            TextTheme = new TextTheme
            {
                Font = Typography.Title1.Value,
                Color = AppColors.LabelOneColorReverted.GetResolvedColorSafe(UIUserInterfaceStyle.Light),
                MinimumFontSize = 10
            },
            ButtonColor = AppColors.LabelOneColor.GetResolvedColorSafe(UIUserInterfaceStyle.Light),
            IconTint = AppColors.LabelOneColor.GetResolvedColorSafe(UIUserInterfaceStyle.Light)
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
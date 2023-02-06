using UIKit;

namespace BMM.UI.iOS.Constants
{
    public static class ThemeExtensions
    {
        private static bool ButtonConfigurationSupported => UIDevice.CurrentDevice.CheckSystemVersion(15, 0);
        
        public static void ApplyTextTheme(this UILabel label, TextTheme theme)
        {
            label.Font = theme.Font;
            label.TextColor = theme.Color;

            if (!theme.MinimumFontSize.HasValue)
                return;
            
            label.MinimumFontSize = theme.MinimumFontSize.Value;
            label.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
            label.AdjustsFontSizeToFitWidth = true;
            label.AdjustsFontForContentSizeCategory = true;
        }

        public static void ApplyButtonStyle(this UIButton button, ButtonTheme theme)
        {
            if (ButtonConfigurationSupported)
                button.Configuration = null;
            
            button.BackgroundColor = theme.ButtonColor;
            button.TitleLabel.Font = theme.TextTheme.Font;
            button.SetTitleColor(theme.TextTheme.Color, UIControlState.Normal);
            button.SetTitleColor(theme.TextTheme.Color.ColorWithAlpha(0.75f), UIControlState.Highlighted);
            button.ImageEdgeInsets = theme.ImageEdgeInsets;
            button.ContentEdgeInsets = theme.ContentEdgeInsets;

            if (theme.IconTint != null)
                button.TintColor = theme.IconTint;

            if (theme.HasRoundedCorners)
                button.Layer.CornerRadius = button.Frame.Height * 0.5f;

            if (theme.HasBorder)
            {
                button.Layer.BorderWidth = 1.0f;
                button.Layer.BorderColor = theme.BorderColor.CGColor;
            }
            
            if (!theme.TextTheme.MinimumFontSize.HasValue)
                return;
            
            button.TitleLabel.MinimumFontSize = theme.TextTheme.MinimumFontSize.Value;
            button.TitleLabel.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
            button.TitleLabel.AdjustsFontSizeToFitWidth = true;
            button.TitleLabel.AdjustsFontForContentSizeCategory = true;
        }
    }
}
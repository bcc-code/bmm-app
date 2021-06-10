using UIKit;

namespace BMM.UI.iOS.Constants
{
    public static class ThemeExtensions
    {
        public static void ApplyTextTheme(this UILabel label, TextTheme theme)
        {
            label.Font = theme.Font;
            label.TextColor = theme.Color;
        }

        public static void ApplyButtonStyle(this UIButton button, ButtonTheme theme)
        {
            button.BackgroundColor = theme.ButtonColor;
            button.TitleLabel.Font = theme.TextTheme.Font;
            button.SetTitleColor(theme.TextTheme.Color, UIControlState.Normal);
            button.SetTitleColor(theme.TextTheme.Color.ColorWithAlpha(0.75f), UIControlState.Highlighted);
            button.Layer.CornerRadius = 20;
            button.ImageEdgeInsets = new UIEdgeInsets(0, -6, 0, 0);
            button.ContentEdgeInsets = new UIEdgeInsets(6, 16, 6, 16);
        }
    }
}
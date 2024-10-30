using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS.Extensions;

public static class AppThemeExtensions
{
    public static UIColor LightOnly(this UIColor color)
    {
        return color?.GetResolvedColorSafe(UIUserInterfaceStyle.Light);
    }
    
    public static TextTheme LightThemeOnly(this TextTheme textTheme)
    {
        textTheme.Color = textTheme.Color.LightOnly();
        return textTheme;
    }
    
    public static ButtonTheme LightThemeOnly(this ButtonTheme buttonTheme)
    {
        buttonTheme.BorderColor = buttonTheme.BorderColor.LightOnly();
        buttonTheme.TextTheme = buttonTheme.TextTheme.LightThemeOnly();
        buttonTheme.ButtonColor = buttonTheme.ButtonColor.LightOnly();
        return buttonTheme;
    }
}
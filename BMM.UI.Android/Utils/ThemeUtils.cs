using AndroidX.AppCompat.App;
using BMM.Core.Models.Themes;

namespace BMM.UI.Droid.Utils
{
    public static class ThemeUtils
    {
        public static int GetUIModeForTheme(Theme theme)
        {
            switch (theme)
            {
                case Theme.Dark:
                    return AppCompatDelegate.ModeNightYes;
                case Theme.Light:
                    return AppCompatDelegate.ModeNightNo;
                case Theme.System:
                    return AppCompatDelegate.ModeNightFollowSystem;
                default:
                    return default;
            }
        }
        
        public static int? GetStyleForTheme(ColorTheme theme, bool isDialog)
        {
            switch (theme)
            {
                case ColorTheme.DarkGreen:
                    return isDialog
                        ? Resource.Style.AppTheme_CustomThemeDarkGreen_NotFullscreen
                        : Resource.Style.AppTheme_CustomThemeDarkGreen;
                case ColorTheme.Orange:
                    return isDialog
                        ? Resource.Style.AppTheme_CustomThemeOrange_NotFullscreen
                        : Resource.Style.AppTheme_CustomThemeOrange;
                case ColorTheme.Violet:
                    return isDialog
                        ? Resource.Style.AppTheme_CustomThemeViolet_NotFullscreen
                        : Resource.Style.AppTheme_CustomThemeViolet;
                case ColorTheme.Red:
                    return isDialog
                        ? Resource.Style.AppTheme_CustomThemeRed_NotFullscreen
                        : Resource.Style.AppTheme_CustomThemeRed;
                case ColorTheme.Golden:
                    return isDialog
                        ? Resource.Style.AppTheme_CustomThemeGolden_NotFullscreen
                        : Resource.Style.AppTheme_CustomThemeGolden;
                default:
                    return isDialog
                        ? Resource.Style.AppTheme_NotFullscreen
                        : Resource.Style.AppTheme;
            }
        }
    }
}
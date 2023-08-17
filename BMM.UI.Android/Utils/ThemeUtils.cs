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
                case Theme.OrangeDark:
                case Theme.GoldenDark:
                    return AppCompatDelegate.ModeNightYes;
                case Theme.Light:
                case Theme.OrangeLight:
                case Theme.GoldenLight:
                    return AppCompatDelegate.ModeNightNo;
                case Theme.System:
                    return AppCompatDelegate.ModeNightFollowSystem;
                default:
                    return default;
            }
        }
        
        public static int? GetStyleForTheme(Theme theme, bool isDialog)
        {
            switch (theme)
            {
                case Theme.OrangeDark:
                case Theme.OrangeLight:
                    return isDialog
                        ? Resource.Style.AppTheme_CustomThemeOrange_NotFullscreen
                        : Resource.Style.AppTheme_CustomThemeOrange;
                case Theme.GoldenDark:
                case Theme.GoldenLight:
                    return isDialog
                        ? Resource.Style.AppTheme_CustomThemeGolden_NotFullscreen
                        : Resource.Style.AppTheme_CustomThemeGolden;
                default:
                    return Resource.Style.AppTheme_NotFullscreen;;
            }
        }
    }
}
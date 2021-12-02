using AndroidX.AppCompat.App;
using BMM.Core.Models.Themes;

namespace BMM.UI.Droid.Utils
{
    public static class ThemeUtils
    {
        public static int GetUIModeForTheme(Theme theme)
        {
            return theme switch
            {
                Theme.Dark => AppCompatDelegate.ModeNightYes,
                Theme.Light => AppCompatDelegate.ModeNightNo,
                Theme.System => AppCompatDelegate.ModeNightFollowSystem,
                _ => default
            };
        }
    }
}
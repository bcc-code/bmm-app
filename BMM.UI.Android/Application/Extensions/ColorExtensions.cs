using Android.Graphics;
using Android.Views;
using BMM.UI.Droid.Application.Helpers;

namespace BMM.UI.Droid.Application.Extensions
{
    public static class ColorExtensions
    {
        public static StatusBarVisibility GetStatusBArVisibilityBasedOnColor(this Color color, Window window)
        {
            return BitmapHelper.BackgroundColorRequiresDarkText(color)
                ? window.DecorView.SystemUiVisibility.AddFlag(SystemUiFlags.LightStatusBar)
                : window.DecorView.SystemUiVisibility.RemoveFlag(SystemUiFlags.LightStatusBar);
        }
    }
}
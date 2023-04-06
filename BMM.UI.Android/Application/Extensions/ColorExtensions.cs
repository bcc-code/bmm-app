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
        
        public static Color WithAlpha(this Color color, float factor)
        {
            double alpha = Math.Round(color.A * factor);
            int red = Color.GetRedComponent(color);
            int green = Color.GetGreenComponent(color);
            int blue = Color.GetBlueComponent(color);
            return Color.Argb((int)alpha, red, green, blue);
        }
    }
}
using Airbnb.Lottie;
using BMM.Core.Constants;
using BMM.Core.Models.Themes;
using BMM.UI.iOS.NewMediaPlayer;
using UIKit;

namespace BMM.UI.iOS.Utils
{
    public static class ThemeUtils
    {
        public static UIUserInterfaceStyle GetUIUserInterfaceStyleForTheme(Theme theme)
        {
            return theme switch
            {
                Theme.Light => UIUserInterfaceStyle.Light,
                Theme.Dark => UIUserInterfaceStyle.Dark,
                _ => UIUserInterfaceStyle.Unspecified
            };
        }
        
        public static bool IsUsingDarkMode => AppDelegate.MainWindow.TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark;
        
        public static LOTAnimationView GetLottieAnimationFor(string lightThemeAnim, string darkThemeAnim)
        {
            if (VersionHelper.SupportsDarkMode && IsUsingDarkMode)
                return LOTAnimationView.AnimationNamed(darkThemeAnim);
            
            return LOTAnimationView.AnimationNamed(lightThemeAnim);
        }
    }
}
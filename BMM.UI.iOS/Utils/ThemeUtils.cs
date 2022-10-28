using BMM.Core.Models.Themes;
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
    }
}
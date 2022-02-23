using UIKit;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public static class VersionHelper
    {
        public static bool SupportsDarkMode => MinimumIosVersion(13);

        public static bool SupportsBarAppearanceProxy => MinimumIosVersion(13);

        private static bool MinimumIosVersion(int major, int minor = 0)
        {
            return UIDevice.CurrentDevice.CheckSystemVersion(major, minor);
        }
    }
}
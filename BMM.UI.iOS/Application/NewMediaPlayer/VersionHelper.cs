using UIKit;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public static class VersionHelper
    {
        public static bool SupportsAirPlayTwo => MinimumIosVersion(11);
        public static bool SupportsAutomaticWaitPlayerProperty => MinimumIosVersion(10);

        public static bool SupportsSafeAreaLayoutGuide => MinimumIosVersion(11);

        /// <summary>
        /// Whether <see cref="UserNotifications.UNUserNotificationCenter"/> is supported.
        /// </summary>
        public static bool SupportsUserNotificationCenter => MinimumIosVersion(10);

        public static bool SupportsDarkMode => MinimumIosVersion(13);

        public static bool SupportsLargeTitles => MinimumIosVersion(11);

        public static bool SupportsBarAppearanceProxy => MinimumIosVersion(13);

        private static bool MinimumIosVersion(int major, int minor = 0)
        {
            return UIDevice.CurrentDevice.CheckSystemVersion(major, minor);
        }
    }
}
using Android.OS;

namespace BMM.UI.Droid.Application.Helpers
{
    public class SdkVersionHelper : ISdkVersionHelper
    {
        private readonly BuildVersionCodes _sdkVersion;

        public SdkVersionHelper(BuildVersionCodes sdkVersion)
        {
            _sdkVersion = sdkVersion;
        }

        public bool SupportsNavigationBarColors => _sdkVersion >= BuildVersionCodes.Lollipop;

        public bool SupportsNavigationBarDividerColor => _sdkVersion >= BuildVersionCodes.P;

        /// <summary>
        /// Older Android versions try to use TLS 1.0 for SSL connection. But often servers require TLS 1.2 or greater therefore causing problems with SSL handshakes.
        /// </summary>
        public bool HasProblemsWithSslHandshakes => _sdkVersion <= BuildVersionCodes.Lollipop;

        public bool SupportsBackgroundActivityRestriction => _sdkVersion >= BuildVersionCodes.P;
    }
}
using Foundation;

namespace BMM.UI.iOS.Extensions
{
    public static class NSUrlExtensions
    {
        public static NSUrl WithScheme(this NSUrl url, string scheme)
        {
            var components = new NSUrlComponents(url, false);
            components.Scheme = scheme;
            return components.Url;
        }
    }
}
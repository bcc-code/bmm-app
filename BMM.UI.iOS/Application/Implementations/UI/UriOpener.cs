using System;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.UI;
using UIKit;

namespace BMM.UI.iOS.UI
{
    public class UriOpener : BaseUriOpener
    {
        public UriOpener(IAnalytics analytics) : base(analytics)
        {
        }
        
        protected override void PlatformOpenUri(Uri uri)
        {
            UIApplication.SharedApplication.OpenUrl(uri, new NSDictionary(), null);
        }
    }
}
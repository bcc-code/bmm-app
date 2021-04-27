using System;
using BMM.Core.Implementations.UI;
using UIKit;

namespace BMM.UI.iOS.UI
{
    public class UriOpener: IUriOpener
    {
        public void OpenUri(Uri uri)
        {
            UIApplication.SharedApplication.OpenUrl(uri);
        }
    }
}
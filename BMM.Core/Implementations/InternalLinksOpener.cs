using System;
using BMM.Core.Helpers;
using BMM.Core.Implementations.UI;

namespace BMM.Core.Implementations
{
    public class InternalLinksOpener : IUriOpener
    {
        private readonly IUriOpener _uriOpener;
        private readonly IDeepLinkHandler _deepLinkHandler;

        public InternalLinksOpener(IUriOpener uriOpener, IDeepLinkHandler deepLinkHandler)
        {
            _uriOpener = uriOpener;
            _deepLinkHandler = deepLinkHandler;
        }

        public void OpenUri(Uri uri)
        {
            // Check first if we can handle the link in the BMM app
            if (_deepLinkHandler.OpenFromInsideOfApp(uri))
                return;

            _uriOpener.OpenUri(uri);
        }
    }
}
using System;

namespace BMM.Core.Helpers
{
    public interface IDeepLinkHandler
    {
        /// <summary>
        /// Open the specified uri.
        /// </summary>
        /// <returns>True if the handler was able to resolve the link, false otherwise.</returns>
        /// <param name="uri">URI.</param>
        bool Open(Uri uri);
    }
}

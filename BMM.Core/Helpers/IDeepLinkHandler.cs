using System;

namespace BMM.Core.Helpers
{
    public interface IDeepLinkHandler
    {
        /// <summary>
        /// Open the specified uri without logging analytics event.
        /// </summary>
        /// <returns>True if the handler was able to resolve the link, false otherwise.</returns>
        /// <param name="uri">URI.</param>
        /// <param name="origin">Place where the deep link has been opened from. Needed for Analytics.</param>
        bool OpenFromInsideOfApp(Uri uri, string origin = "");

        /// <summary>
        /// Open the specified uri with logging analytics event.
        /// </summary>
        /// <returns>True if the handler was able to resolve the link, false otherwise.</returns>
        /// <param name="uri">URI.</param>
        bool OpenFromOutsideOfApp(Uri uri);

        void SetDeepLinkWillStartPlayerIfNeeded(string deepLink);
    }
}

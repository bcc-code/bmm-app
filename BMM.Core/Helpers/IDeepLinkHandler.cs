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
        bool OpenFromInsideOfApp(Uri uri);

        /// <summary>
        /// Open the specified uri with logging analytics event.
        /// </summary>
        /// <returns>True if the handler was able to resolve the link, false otherwise.</returns>
        /// <param name="uri">URI.</param>
        bool OpenFromOutsideOfApp(Uri uri);

        bool DeepLinkStartsPlaying(string deepLink);
    }
}

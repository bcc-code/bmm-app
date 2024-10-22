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

        /// <summary>
        ///     Use this method to notify about 'ready to handle deep link' state, which means the app successfully loaded needed elements.
        ///     In most cases it should be called after navigating to main navigation container.
        ///     Also, if there is a pending deep link to handle, this method executes it.  
        /// </summary>
        void SetReadyToOpenDeepLinkAndHandlePending();

        void SetDeepLinkWillStartPlayerIfNeeded(string deepLink);

        bool IsBmmUrl(Uri uri);

        int? GetIdFromUriIfPossible(Uri uri, string regex);
    }
}

using System;
using System.Linq;
using BMM.Core.Implementations.Downloading.FileDownloader;
using Foundation;

namespace BMM.UI.iOS.Implementations.Download.Exceptions
{
    public class IosDownloadException : Exception, IConnectionAwareDownloadException
    {
        private const string UrlErrorDomain = "NSURLErrorDomain";

        /// <summary>
        /// NSURLError codes that mean "the network let us down", as opposed to something being wrong with
        /// this particular file. The download queue keeps its remaining items for these instead of
        /// skipping them, since every other download would fail the same way.
        /// </summary>
        private static readonly int[] ConnectionErrorCodes =
        {
            -1001, // TimedOut
            -1003, // CannotFindHost
            -1004, // CannotConnectToHost
            -1005, // NetworkConnectionLost
            -1009, // NotConnectedToInternet
            -1018, // InternationalRoamingOff
            -1019, // CallIsActive
            -1020, // DataNotAllowed
            -997   // Lost connection to the background transfer daemon
        };

        private readonly string _downloadRequestUrl;
        private readonly NSError _error;

        /// <summary>
        /// The error is allowed to be null: a failure detected by the app itself (a corrupt file, a move
        /// that did not happen) has no <see cref="NSError"/> behind it. Dereferencing it unconditionally
        /// used to throw a <see cref="NullReferenceException"/> from inside the download queue's own catch
        /// block, which aborted the entire queue rather than skipping one file.
        /// </summary>
        public override string Message =>
            $"Error while downloading file {_downloadRequestUrl}: {_error?.Description ?? "no error details"}";

        public bool WasConnectionFailure =>
            _error != null
            && _error.Domain == UrlErrorDomain
            && ConnectionErrorCodes.Contains((int)_error.Code);

        public IosDownloadException(string downloadRequestUrl, NSError error)
        {
            _downloadRequestUrl = downloadRequestUrl;
            _error = error;
        }
    }
}

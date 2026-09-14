using System;
using Foundation;

namespace BMM.UI.iOS.Implementations.Download.Exceptions
{
    public class IosDownloadException : Exception
    {
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

        public IosDownloadException(string downloadRequestUrl, NSError error)
        {
            _downloadRequestUrl = downloadRequestUrl;
            _error = error;
        }
    }
}
using System;
using Foundation;

namespace BMM.UI.iOS.Implementations.Download.Exceptions
{
    public class IosDownloadException : Exception
    {
        private readonly string _downloadRequestUrl;
        private readonly NSError _error;

        public override string Message => $"Error while downloading file {_downloadRequestUrl}: {_error.Description}";

        public IosDownloadException(string downloadRequestUrl, NSError error)
        {
            _downloadRequestUrl = downloadRequestUrl;
            _error = error;
        }
    }
}
using System;

namespace BMM.UI.iOS.Implementations.Download.Exceptions
{
    public class IosFailedDownloadRecoverException : Exception
    {
        private readonly string _downloadRequestUrl;

        public override string Message => $"Could not recover from failed download request: {_downloadRequestUrl}";

        public IosFailedDownloadRecoverException(string downloadRequestUrl)
        {
            _downloadRequestUrl = downloadRequestUrl;
        }
    }
}
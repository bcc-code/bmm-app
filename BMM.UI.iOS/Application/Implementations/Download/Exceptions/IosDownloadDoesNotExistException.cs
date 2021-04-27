using System;

namespace BMM.UI.iOS.Implementations.Download.Exceptions
{
    public class IosDownloadDoesNotExistException: Exception
    {
        private readonly string _downloadRequestUrl;

        public override string Message => $"Tried to process non existing download: {_downloadRequestUrl}";

        public IosDownloadDoesNotExistException(string downloadRequestUrl)
        {
            _downloadRequestUrl = downloadRequestUrl;
        }
    }
}
using System;

namespace BMM.UI.iOS.Implementations.Download.Exceptions
{
    public class UnexpectedIosDownloadException : Exception
    {
        private readonly string _downloadRequestUrl;
        private readonly Exception _innerException;

        public override string Message => $"Unexpected error while downloading file {_downloadRequestUrl}: {_innerException.Message}";

        public override string StackTrace => _innerException.StackTrace;

        public UnexpectedIosDownloadException(string downloadRequestUrl, Exception innerException)
        {
            _downloadRequestUrl = downloadRequestUrl;
            _innerException = innerException;
        }
    }
}
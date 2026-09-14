using System;
using System.Net;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    /// <summary>
    /// A download that came back with a non-success status code. Carries the code itself so the queue can
    /// tell "this file is gone" (404) apart from "come back later" (401, 5xx) instead of parsing a message.
    /// </summary>
    public class DownloadHttpStatusException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public DownloadHttpStatusException(HttpStatusCode statusCode)
            : base($"The request returned with error HTTP status code {statusCode}")
        {
            StatusCode = statusCode;
        }
    }
}

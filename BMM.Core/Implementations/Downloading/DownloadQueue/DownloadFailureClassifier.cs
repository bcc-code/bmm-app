using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using BMM.Api.Framework.Exceptions;
using BMM.Core.Implementations.Downloading.FileDownloader;

namespace BMM.Core.Implementations.Downloading.DownloadQueue
{
    /// <summary>
    /// Decides whether a failed download attempt was the network's fault.
    /// </summary>
    /// <remarks>
    /// Analytics showed roughly 62.000 "Connection failure" events against about 113 timeouts, the vast
    /// majority of them logged while the device reported no active connection at all. Those failures come
    /// back within milliseconds, so a queue that treats them as "this file is done" empties itself in a
    /// couple of seconds and then looks exactly like a completed download.
    /// </remarks>
    public static class DownloadFailureClassifier
    {
        public static DownloadOutcome Classify(Exception exception, bool cancellationWasRequested)
        {
            return exception switch
            {
                // A cancellation we asked for is not a failure. Anything else that surfaces as
                // cancellation is a timeout, which is a network problem.
                OperationCanceledException when cancellationWasRequested => DownloadOutcome.Cancelled,
                OperationCanceledException => DownloadOutcome.ConnectionFailure,

                InternetProblemsException => DownloadOutcome.ConnectionFailure,
                SocketException => DownloadOutcome.ConnectionFailure,
                WebException => DownloadOutcome.ConnectionFailure,

                // "Connection failure", "Software caused connection abort", "Socket closed" and
                // "Unable to resolve host ..." all arrive as HttpRequestException.
                HttpRequestException => DownloadOutcome.ConnectionFailure,

                // The response started arriving and then the connection died mid-body.
                IOException => DownloadOutcome.ConnectionFailure,

                DownloadHttpStatusException statusException => ClassifyStatusCode(statusException.StatusCode),

                _ => IsConnectionFailure(exception.InnerException, cancellationWasRequested)
                    ? DownloadOutcome.ConnectionFailure
                    : DownloadOutcome.PermanentFailure
            };
        }

        private static bool IsConnectionFailure(Exception inner, bool cancellationWasRequested)
        {
            return inner != null && Classify(inner, cancellationWasRequested) == DownloadOutcome.ConnectionFailure;
        }

        private static DownloadOutcome ClassifyStatusCode(HttpStatusCode statusCode)
        {
            // 401 means the access token needs refreshing, which is worth another attempt later.
            // 5xx and 429 are the server asking us to come back. Everything else is about this file.
            if (statusCode == HttpStatusCode.Unauthorized
                || statusCode == HttpStatusCode.RequestTimeout
                || statusCode == HttpStatusCode.TooManyRequests
                || (int)statusCode >= 500)
            {
                return DownloadOutcome.ConnectionFailure;
            }

            return DownloadOutcome.PermanentFailure;
        }
    }
}

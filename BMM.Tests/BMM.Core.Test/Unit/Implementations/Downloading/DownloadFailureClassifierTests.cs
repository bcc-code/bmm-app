using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using BMM.Api.Framework.Exceptions;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Downloading.FileDownloader;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.Downloading
{
    [TestFixture]
    public class DownloadFailureClassifierTests
    {
        private static DownloadOutcome Classify(Exception exception, bool cancellationWasRequested = false)
            => DownloadFailureClassifier.Classify(exception, cancellationWasRequested);

        [Test]
        public void Http_Request_Failures_Are_Connection_Failures()
        {
            // "Connection failure", "Software caused connection abort", "Socket closed" and
            // "Unable to resolve host ..." all reach us as HttpRequestException.
            Assert.AreEqual(DownloadOutcome.ConnectionFailure, Classify(new HttpRequestException("Connection failure")));
        }

        [Test]
        public void Socket_And_Web_And_Io_Failures_Are_Connection_Failures()
        {
            Assert.AreEqual(DownloadOutcome.ConnectionFailure, Classify(new SocketException()));
            Assert.AreEqual(DownloadOutcome.ConnectionFailure, Classify(new WebException()));
            Assert.AreEqual(DownloadOutcome.ConnectionFailure, Classify(new IOException()));
            Assert.AreEqual(DownloadOutcome.ConnectionFailure, Classify(new InternetProblemsException(new Exception())));
        }

        [Test]
        public void A_Timeout_Is_A_Connection_Failure()
        {
            Assert.AreEqual(DownloadOutcome.ConnectionFailure, Classify(new TaskCanceledException()));
        }

        [Test]
        public void A_Cancellation_We_Asked_For_Is_Not_A_Failure()
        {
            Assert.AreEqual(DownloadOutcome.Cancelled, Classify(new TaskCanceledException(), cancellationWasRequested: true));
        }

        [Test]
        public void A_Connection_Failure_Nested_In_Another_Exception_Is_Still_A_Connection_Failure()
        {
            var wrapped = new Exception("wrapped", new HttpRequestException("Connection failure"));

            Assert.AreEqual(DownloadOutcome.ConnectionFailure, Classify(wrapped));
        }

        [Test]
        public void A_Missing_File_Is_A_Permanent_Failure()
        {
            Assert.AreEqual(DownloadOutcome.PermanentFailure, Classify(new DownloadHttpStatusException(HttpStatusCode.NotFound)));
            Assert.AreEqual(DownloadOutcome.PermanentFailure, Classify(new DownloadHttpStatusException(HttpStatusCode.Forbidden)));
        }

        [Test]
        public void Statuses_Worth_Retrying_Are_Connection_Failures()
        {
            // 401 means the access token needs refreshing, 5xx and 429 are the server asking us to
            // come back. Skipping the file for those would lose it until the next full synchronization.
            Assert.AreEqual(DownloadOutcome.ConnectionFailure, Classify(new DownloadHttpStatusException(HttpStatusCode.Unauthorized)));
            Assert.AreEqual(DownloadOutcome.ConnectionFailure, Classify(new DownloadHttpStatusException(HttpStatusCode.TooManyRequests)));
            Assert.AreEqual(DownloadOutcome.ConnectionFailure, Classify(new DownloadHttpStatusException(HttpStatusCode.BadGateway)));
        }

        /// <summary>
        /// The iOS downloader fails with an NSError rather than a framework exception type, so it reports
        /// the verdict itself. Without this the queue would treat "not connected to the internet" on iOS
        /// as a broken file and skip every remaining track.
        /// </summary>
        [Test]
        public void A_Platform_Exception_Decides_For_Itself()
        {
            Assert.AreEqual(DownloadOutcome.ConnectionFailure, Classify(new FakePlatformDownloadException(wasConnectionFailure: true)));
            Assert.AreEqual(DownloadOutcome.PermanentFailure, Classify(new FakePlatformDownloadException(wasConnectionFailure: false)));
        }

        [Test]
        public void An_Unrecognised_Failure_Is_Permanent_So_The_Queue_Continues()
        {
            Assert.AreEqual(DownloadOutcome.PermanentFailure, Classify(new InvalidOperationException()));
        }

        private class FakePlatformDownloadException : Exception, IConnectionAwareDownloadException
        {
            public FakePlatformDownloadException(bool wasConnectionFailure)
            {
                WasConnectionFailure = wasConnectionFailure;
            }

            public bool WasConnectionFailure { get; }
        }
    }
}

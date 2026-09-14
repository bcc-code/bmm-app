using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.Exceptions;
using Moq;
using MvvmCross.Plugin.Messenger;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.Downloading
{
    [TestFixture]
    public class DownloadQueueTests
    {
        private readonly FakeTrackFactory _fakeTrackFactory = new FakeTrackFactory();
        private Mock<IFileDownloader> _fileDownloader;
        private Mock<IMvxMessenger> _mvxMessenger;
        private Mock<IExceptionHandler> _exceptionHandler;
        private Mock<IAnalytics> _analytics;
        private Mock<IConnection> _connection;
        private Mock<INetworkSettings> _networkSettings;
        private Mock<ILogger> _logger;
        private Task _queueRun;

        [SetUp]
        public void Init()
        {
            _fileDownloader = new Mock<IFileDownloader>();
            _mvxMessenger = new Mock<IMvxMessenger>();
            _exceptionHandler = new Mock<IExceptionHandler>();
            _analytics = new Mock<IAnalytics>();
            _connection = new Mock<IConnection>();
            _networkSettings = new Mock<INetworkSettings>();
            _logger = new Mock<ILogger>();
            _queueRun = Task.CompletedTask;

            _connection.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Online);
            _connection.Setup(x => x.IsUsingNetworkWithoutExtraCosts()).Returns(true);
            _networkSettings.Setup(x => x.GetMobileNetworkDownloadAllowed()).ReturnsAsync(true);

            // The queue hands its work to the exception handler, so capture it and run it inline.
            _exceptionHandler
                .Setup(x => x.FireAndForgetWithoutUserMessages(It.IsAny<Func<Task>>()))
                .Callback<Func<Task>>(action => _queueRun = action());
        }

        private TestableDownloadQueue CreateDownloadQueue()
        {
            return new TestableDownloadQueue(
                _fileDownloader.Object,
                _mvxMessenger.Object,
                _exceptionHandler.Object,
                _analytics.Object,
                _connection.Object,
                _networkSettings.Object,
                _logger.Object
            );
        }

        private async Task<TestableDownloadQueue> RunQueueWith(params Track[] tracks)
        {
            var downloadQueue = CreateDownloadQueue();
            downloadQueue.Enqueue(tracks);
            downloadQueue.StartDownloading();
            await _queueRun;
            return downloadQueue;
        }

        [Test]
        public void Downloadable_Gets_Queued()
        {
            var downloadQueue = CreateDownloadQueue();

            var track = _fakeTrackFactory.CreateTrackWithId(1);

            downloadQueue.Enqueue(track);

            Assert.IsTrue(downloadQueue.IsQueued(track));
        }

        [Test]
        public void DownloadQueue_Should_Not_Queue_Same_Item_Multiple_Times()
        {
            var downloadQueue = CreateDownloadQueue();

            var track = _fakeTrackFactory.CreateTrackWithId(1);
            downloadQueue.Enqueue(track);
            downloadQueue.Enqueue(track);

            Assert.AreEqual(1, downloadQueue.InitialDownloadCount);
        }

        [Test]
        public void DownloadQueue_Dequeue_All_Except_Passed()
        {
            var downloadQueue = CreateDownloadQueue();

            var tracks = Enumerable.Range(1, 5).Select(_fakeTrackFactory.CreateTrackWithId);
            downloadQueue.Enqueue(tracks);

            var tracksNotToRemove = Enumerable.Range(3, 2).Select(_fakeTrackFactory.CreateTrackWithId);

            downloadQueue.DequeueAllExcept(tracksNotToRemove);

            Assert.AreEqual(2, downloadQueue.InitialDownloadCount);

        }

        [Test]
        public void DownloadQueue_Should_Start_Downloading()
        {
            var downloadQueue = CreateDownloadQueue();

            var track = _fakeTrackFactory.CreateTrackWithId(1);

            downloadQueue.Enqueue(track);
            downloadQueue.StartDownloading();

           _exceptionHandler.Verify(x => x.FireAndForgetWithoutUserMessages(It.IsAny<Func<Task>>()), Times.Once);
        }

        [Test]
        public async Task Successful_Downloads_Empty_The_Queue()
        {
            var tracks = Enumerable.Range(1, 3).Select(_fakeTrackFactory.CreateTrackWithId).ToArray();

            var downloadQueue = await RunQueueWith(tracks);

            Assert.AreEqual(0, downloadQueue.RemainingDownloadsCount);
            Assert.IsFalse(downloadQueue.StoppedWithPendingDownloads);
            _mvxMessenger.Verify(x => x.Publish(It.Is<QueueFinishedMessage>(m => m.Succeeded)), Times.Once);
        }

        [Test]
        public async Task A_Connection_Failure_Stops_The_Queue_Instead_Of_Draining_It()
        {
            var tracks = Enumerable.Range(1, 5).Select(_fakeTrackFactory.CreateTrackWithId).ToArray();
            _fileDownloader
                .Setup(x => x.DownloadFile(It.IsAny<IDownloadable>()))
                .ThrowsAsync(new HttpRequestException("Connection failure"));

            var downloadQueue = await RunQueueWith(tracks);

            // The whole point: a dead connection must not turn five queued tracks into five instant
            // failures and an empty queue that looks like a finished download.
            Assert.AreEqual(5, downloadQueue.RemainingDownloadsCount);
            Assert.IsTrue(downloadQueue.StoppedWithPendingDownloads);
            _fileDownloader.Verify(x => x.DownloadFile(It.IsAny<IDownloadable>()), Times.Exactly(3));
        }

        [Test]
        public async Task A_Stopped_Queue_Does_Not_Report_Success()
        {
            _fileDownloader
                .Setup(x => x.DownloadFile(It.IsAny<IDownloadable>()))
                .ThrowsAsync(new HttpRequestException("Connection failure"));

            await RunQueueWith(_fakeTrackFactory.CreateTrackWithId(1));

            _mvxMessenger.Verify(x => x.Publish(It.Is<QueueFinishedMessage>(m => m.Succeeded)), Times.Never);
            _mvxMessenger.Verify(x => x.Publish(It.Is<QueueFinishedMessage>(m => !m.Succeeded)), Times.Once);
        }

        [Test]
        public async Task A_Stopped_Queue_Is_Not_Still_Running()
        {
            _fileDownloader
                .Setup(x => x.DownloadFile(It.IsAny<IDownloadable>()))
                .ThrowsAsync(new HttpRequestException("Connection failure"));

            var downloadQueue = await RunQueueWith(_fakeTrackFactory.CreateTrackWithId(1));

            // Items are still queued, but nothing is downloading them. Reporting otherwise leaves a
            // progress indicator on screen for a download that is not happening.
            Assert.IsFalse(downloadQueue.IsRunning);
            Assert.AreEqual(1, downloadQueue.RemainingDownloadsCount);
        }

        [Test]
        public async Task A_Failed_Download_Is_Not_Reported_As_Downloaded()
        {
            _fileDownloader
                .Setup(x => x.DownloadFile(It.IsAny<IDownloadable>()))
                .ThrowsAsync(new HttpRequestException("Connection failure"));

            await RunQueueWith(_fakeTrackFactory.CreateTrackWithId(1));

            _analytics.Verify(
                x => x.LogEvent(Event.TrackHasBeenDownloaded, It.IsAny<System.Collections.Generic.IDictionary<string, object>>()),
                Times.Never);
        }

        [Test]
        public async Task A_Missing_File_Is_Skipped_So_The_Rest_Of_The_Queue_Continues()
        {
            var missing = _fakeTrackFactory.CreateTrackWithId(1);
            var tracks = new[] { missing, _fakeTrackFactory.CreateTrackWithId(2), _fakeTrackFactory.CreateTrackWithId(3) };

            _fileDownloader
                .Setup(x => x.DownloadFile(It.Is<IDownloadable>(d => d.Id == missing.Id)))
                .ThrowsAsync(new DownloadHttpStatusException(HttpStatusCode.NotFound));

            var downloadQueue = await RunQueueWith(tracks);

            Assert.AreEqual(0, downloadQueue.RemainingDownloadsCount);
            _fileDownloader.Verify(x => x.DownloadFile(It.IsAny<IDownloadable>()), Times.Exactly(3));
        }

        [Test]
        public async Task Nothing_Is_Downloaded_While_The_Device_Is_Offline()
        {
            _connection.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Offline);
            var tracks = Enumerable.Range(1, 3).Select(_fakeTrackFactory.CreateTrackWithId).ToArray();

            var downloadQueue = await RunQueueWith(tracks);

            _fileDownloader.Verify(x => x.DownloadFile(It.IsAny<IDownloadable>()), Times.Never);
            Assert.AreEqual(3, downloadQueue.RemainingDownloadsCount);
            Assert.IsTrue(downloadQueue.StoppedWithPendingDownloads);
        }

        [Test]
        public async Task Downloading_Stops_When_The_User_Leaves_A_Free_Network()
        {
            _connection.Setup(x => x.IsUsingNetworkWithoutExtraCosts()).Returns(false);
            _networkSettings.Setup(x => x.GetMobileNetworkDownloadAllowed()).ReturnsAsync(false);

            var downloadQueue = await RunQueueWith(_fakeTrackFactory.CreateTrackWithId(1));

            _fileDownloader.Verify(x => x.DownloadFile(It.IsAny<IDownloadable>()), Times.Never);
            Assert.AreEqual(1, downloadQueue.RemainingDownloadsCount);
        }

        [Test]
        public async Task A_Transient_Failure_Is_Retried()
        {
            int attempts = 0;
            _fileDownloader
                .Setup(x => x.DownloadFile(It.IsAny<IDownloadable>()))
                .Returns(() =>
                {
                    attempts++;
                    return attempts == 1
                        ? Task.FromException(new HttpRequestException("Connection failure"))
                        : Task.CompletedTask;
                });

            var downloadQueue = await RunQueueWith(_fakeTrackFactory.CreateTrackWithId(1));

            Assert.AreEqual(2, attempts);
            Assert.AreEqual(0, downloadQueue.RemainingDownloadsCount);
            Assert.IsFalse(downloadQueue.StoppedWithPendingDownloads);
        }

        private class TestableDownloadQueue : DownloadQueue
        {
            public TestableDownloadQueue(
                IFileDownloader fileDownloader,
                IMvxMessenger messenger,
                IExceptionHandler exceptionHandler,
                IAnalytics analytics,
                IConnection connection,
                INetworkSettings networkSettings,
                ILogger logger)
                : base(fileDownloader, messenger, exceptionHandler, analytics, connection, networkSettings, logger)
            {
            }

            protected override Task WaitBeforeRetry(TimeSpan delay) => Task.CompletedTask;
        }
    }
}

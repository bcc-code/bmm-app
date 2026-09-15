using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FileStorage;
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
        private Mock<IStorageManager> _storageManager;
        private Mock<IFileStorage> _fileStorage;
        private Mock<IUnavailableTrackRegistry> _unavailableTracks;
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
            _storageManager = new Mock<IStorageManager>();
            _fileStorage = new Mock<IFileStorage>();
            _storageManager.Setup(x => x.SelectedStorage).Returns(_fileStorage.Object);
            _fileStorage.Setup(x => x.FreeSpace).Returns(10L * 1024 * 1024 * 1024);
            _unavailableTracks = new Mock<IUnavailableTrackRegistry>();
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
                _storageManager.Object,
                _unavailableTracks.Object,
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
        public async Task Abandoned_Files_Are_Reported_Even_When_The_Queue_Empties()
        {
            // A run that skips permanently broken files still empties the queue, so without this the only
            // trace would be the individual failures, and the collection would simply never be able to
            // show as downloaded with nothing explaining why.
            var missing = _fakeTrackFactory.CreateTrackWithId(1);
            _fileDownloader
                .Setup(x => x.DownloadFile(It.Is<IDownloadable>(d => d.Id == missing.Id)))
                .ThrowsAsync(new DownloadHttpStatusException(HttpStatusCode.NotFound));

            var downloadQueue = await RunQueueWith(missing, _fakeTrackFactory.CreateTrackWithId(2));

            Assert.AreEqual(0, downloadQueue.RemainingDownloadsCount);
            _logger.Verify(x => x.Error(nameof(DownloadQueue), It.Is<string>(m => m.Contains("abandoned"))), Times.Once);
            _analytics.Verify(
                x => x.LogEvent("Download queue stopped early", It.IsAny<System.Collections.Generic.IDictionary<string, object>>()),
                Times.Once);
        }

        [Test]
        public async Task An_Unobtainable_Track_Is_Remembered_So_Its_Collection_Can_Still_Complete()
        {
            var missing = _fakeTrackFactory.CreateTrackWithId(1);
            _fileDownloader
                .Setup(x => x.DownloadFile(It.Is<IDownloadable>(d => d.Id == missing.Id)))
                .ThrowsAsync(new DownloadHttpStatusException(HttpStatusCode.NotFound));

            await RunQueueWith(missing, _fakeTrackFactory.CreateTrackWithId(2));

            _unavailableTracks.Verify(x => x.MarkUnavailable(missing.Id), Times.Once);
            _unavailableTracks.Verify(x => x.MarkAvailable(2), Times.Once);
        }

        [Test]
        public async Task A_Track_That_Downloads_Is_No_Longer_Considered_Unobtainable()
        {
            await RunQueueWith(_fakeTrackFactory.CreateTrackWithId(1));

            _unavailableTracks.Verify(x => x.MarkAvailable(1), Times.Once);
            _unavailableTracks.Verify(x => x.MarkUnavailable(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task A_Connection_Failure_Does_Not_Mark_A_Track_Unobtainable()
        {
            // The file is probably fine, we just could not reach it. Marking it would let a collection
            // report itself complete while tracks are still missing.
            _fileDownloader
                .Setup(x => x.DownloadFile(It.IsAny<IDownloadable>()))
                .ThrowsAsync(new HttpRequestException("Connection failure"));

            await RunQueueWith(_fakeTrackFactory.CreateTrackWithId(1));

            _unavailableTracks.Verify(x => x.MarkUnavailable(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task A_Fully_Successful_Run_Is_Not_Reported_As_A_Problem()
        {
            await RunQueueWith(_fakeTrackFactory.CreateTrackWithId(1));

            _logger.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task An_Unexpected_Error_Does_Not_Leave_The_Queue_Marked_As_Running()
        {
            _mvxMessenger
                .Setup(x => x.Publish(It.IsAny<FileDownloadStartedMessage>()))
                .Throws(new InvalidOperationException("a subscriber blew up"));

            var downloadQueue = await RunQueueWith(_fakeTrackFactory.CreateTrackWithId(1));

            Assert.IsFalse(downloadQueue.IsRunning);
            _logger.Verify(
                x => x.Error(nameof(DownloadQueue), It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<bool>()),
                Times.AtLeastOnce);
        }

        [Test]
        public async Task A_Full_Disk_Stops_The_Queue_Without_Retrying()
        {
            // A full disk surfaces as IOException, exactly like a stream that died mid-download. Retrying
            // it would spin the queue against a disk that is not going to empty itself.
            _fileStorage.Setup(x => x.FreeSpace).Returns(1024);
            var tracks = Enumerable.Range(1, 3).Select(_fakeTrackFactory.CreateTrackWithId).ToArray();
            _fileDownloader
                .Setup(x => x.DownloadFile(It.IsAny<IDownloadable>()))
                .ThrowsAsync(new IOException("No space left on device"));

            var downloadQueue = await RunQueueWith(tracks);

            _fileDownloader.Verify(x => x.DownloadFile(It.IsAny<IDownloadable>()), Times.Once);
            Assert.AreEqual(3, downloadQueue.RemainingDownloadsCount);
            Assert.IsTrue(downloadQueue.StoppedWithPendingDownloads);
        }

        [Test]
        public async Task A_Full_Disk_Is_Reported_As_Such()
        {
            _fileStorage.Setup(x => x.FreeSpace).Returns(1024);
            _fileDownloader
                .Setup(x => x.DownloadFile(It.IsAny<IDownloadable>()))
                .ThrowsAsync(new IOException("No space left on device"));

            await RunQueueWith(_fakeTrackFactory.CreateTrackWithId(1));

            _mvxMessenger.Verify(
                x => x.Publish(It.Is<FileDownloadCanceledMessage>(m => m.Exception is StorageOutOfSpaceException)),
                Times.Once);
        }

        [Test]
        public async Task A_Dropped_Stream_On_A_Healthy_Disk_Is_Still_A_Connection_Failure()
        {
            // Same exception type as a full disk, but there is plenty of room, so it must keep the
            // retry behaviour rather than being written off as out of space.
            _fileDownloader
                .Setup(x => x.DownloadFile(It.IsAny<IDownloadable>()))
                .ThrowsAsync(new IOException("Connection reset by peer"));

            await RunQueueWith(_fakeTrackFactory.CreateTrackWithId(1));

            _fileDownloader.Verify(x => x.DownloadFile(It.IsAny<IDownloadable>()), Times.Exactly(3));
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
                IStorageManager storageManager,
                IUnavailableTrackRegistry unavailableTracks,
                ILogger logger)
                : base(fileDownloader, messenger, exceptionHandler, analytics, connection, networkSettings, storageManager, unavailableTracks, logger)
            {
            }

            protected override Task WaitBeforeRetry(TimeSpan delay) => Task.CompletedTask;
        }
    }
}

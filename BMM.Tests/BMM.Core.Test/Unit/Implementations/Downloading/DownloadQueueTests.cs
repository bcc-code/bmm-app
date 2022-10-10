using System;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Implementations.Analytics;
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

        [SetUp]
        public void Init()
        {
            _fileDownloader = new Mock<IFileDownloader>();
            _mvxMessenger = new Mock<IMvxMessenger>();
            _exceptionHandler = new Mock<IExceptionHandler>();
            _analytics = new Mock<IAnalytics>();
        }

        private DownloadQueue CreateDownloadQueue()
        {
            return new DownloadQueue(
                _fileDownloader.Object,
                _mvxMessenger.Object,
                _exceptionHandler.Object,
                _analytics.Object
            );
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
    }
}
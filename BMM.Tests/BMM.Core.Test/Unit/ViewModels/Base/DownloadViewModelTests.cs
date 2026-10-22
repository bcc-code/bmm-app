using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Test.Unit.Implementations.Downloading;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels.Base
{
    /// <summary>
    /// Covers what the header tells the user when a download does not finish. The states are only
    /// distinguishable from the queue's messages, so they are driven through the same handlers the
    /// messenger calls.
    /// </summary>
    [TestFixture]
    public class DownloadViewModelTests : BaseViewModelTests
    {
        private readonly FakeTrackFactory _fakeTrackFactory = new FakeTrackFactory();
        private Mock<IStorageManager> _storageManager;
        private Mock<IFileStorage> _fileStorage;
        private Mock<IDownloadQueue> _downloadQueue;
        private Mock<IUnavailableTrackRegistry> _unavailableTracks;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _storageManager = new Mock<IStorageManager>();
            _fileStorage = new Mock<IFileStorage>();
            _storageManager.Setup(x => x.SelectedStorage).Returns(_fileStorage.Object);
            _downloadQueue = new Mock<IDownloadQueue>();
            _unavailableTracks = new Mock<IUnavailableTrackRegistry>();
        }

        private TestDownloadViewModel CreateViewModel(params Track[] tracks)
        {
            var viewModel = new TestDownloadViewModel(
                _storageManager.Object,
                new Mock<IDocumentFilter>().Object,
                _downloadQueue.Object,
                new Mock<IConnection>().Object,
                new Mock<INetworkSettings>().Object,
                _unavailableTracks.Object,
                new Mock<ILogger>().Object,
                tracks);

            // Normally supplied by MvvmCross property injection.
            viewModel.TextSource = TextResource.Object;
            viewModel.SetOfflineAvailable(true);
            return viewModel;
        }

        [Test]
        public void Nothing_Is_Reported_While_All_Is_Well()
        {
            var track = _fakeTrackFactory.CreateTrackWithId(1);
            _fileStorage.Setup(x => x.IsDownloaded(It.IsAny<IDownloadable>())).Returns(true);
            var viewModel = CreateViewModel(track);

            viewModel.SimulateQueueFinished(succeeded: true);

            Assert.IsFalse(viewModel.HasDownloadProblem);
            Assert.IsNull(viewModel.DownloadProblemText);
        }

        [Test]
        public void Running_Out_Of_Space_Is_Reported()
        {
            var viewModel = CreateViewModel(_fakeTrackFactory.CreateTrackWithId(1));

            viewModel.SimulateDownloadCanceled(new StorageOutOfSpaceException(_fileStorage.Object));

            Assert.IsTrue(viewModel.HasDownloadProblem);
            Assert.AreEqual(Translations.TrackCollectionViewModel_NotEnoughtSpaceToDownload, viewModel.DownloadProblemText);
        }

        [Test]
        public void Running_Out_Of_Space_Keeps_Precedence_Over_The_Queue_Outcome()
        {
            // Both arrive for the same run, and "paused" would tell the user nothing about what to do.
            _downloadQueue.Setup(x => x.StoppedWithPendingDownloads).Returns(true);
            var viewModel = CreateViewModel(_fakeTrackFactory.CreateTrackWithId(1));

            viewModel.SimulateDownloadCanceled(new StorageOutOfSpaceException(_fileStorage.Object));
            viewModel.SimulateQueueFinished(succeeded: false);

            Assert.AreEqual(Translations.TrackCollectionViewModel_NotEnoughtSpaceToDownload, viewModel.DownloadProblemText);
        }

        [Test]
        public void A_Queue_That_Stopped_With_Work_Left_Is_Reported_As_Paused()
        {
            _downloadQueue.Setup(x => x.StoppedWithPendingDownloads).Returns(true);
            var viewModel = CreateViewModel(_fakeTrackFactory.CreateTrackWithId(1));

            viewModel.SimulateQueueFinished(succeeded: false);

            Assert.AreEqual(Translations.TrackCollectionViewModel_DownloadPausedNoConnection, viewModel.DownloadProblemText);
        }

        [Test]
        public void Unobtainable_Tracks_Are_Reported_After_A_Finished_Run()
        {
            var tracks = Enumerable.Range(1, 3).Select(_fakeTrackFactory.CreateTrackWithId).ToArray();
            _unavailableTracks.Setup(x => x.IsUnavailable(2)).Returns(true);
            // GetText comes from IMvxLanguageBinder as GetText(string, params object[]).
            TextResource
                .Setup(x => x.GetText(Translations.TrackCollectionViewModel_SomeTracksUnavailable, It.IsAny<object[]>()))
                .Returns<string, object[]>((key, args) => $"{key}:{args[0]}");
            var viewModel = CreateViewModel(tracks);

            viewModel.SimulateQueueFinished(succeeded: true);

            Assert.AreEqual($"{Translations.TrackCollectionViewModel_SomeTracksUnavailable}:1", viewModel.DownloadProblemText);
        }

        [Test]
        public void A_New_Attempt_Clears_The_Previous_Reason()
        {
            var viewModel = CreateViewModel(_fakeTrackFactory.CreateTrackWithId(1));
            viewModel.SimulateDownloadCanceled(new StorageOutOfSpaceException(_fileStorage.Object));

            viewModel.SimulateDownloadStarted();

            Assert.IsFalse(viewModel.HasDownloadProblem);
        }

        [Test]
        public void An_Item_The_User_Did_Not_Ask_For_Offline_Reports_Nothing()
        {
            // The queue's messages are global, so a failure elsewhere must not surface here.
            _downloadQueue.Setup(x => x.StoppedWithPendingDownloads).Returns(true);
            var viewModel = CreateViewModel(_fakeTrackFactory.CreateTrackWithId(1));
            viewModel.SetOfflineAvailable(false);

            viewModel.SimulateDownloadCanceled(new StorageOutOfSpaceException(_fileStorage.Object));
            viewModel.SimulateQueueFinished(succeeded: false);

            Assert.IsFalse(viewModel.HasDownloadProblem);
        }

        [Test]
        public void The_Subtitle_Shows_The_Reason_Instead_Of_The_Duration()
        {
            _downloadQueue.Setup(x => x.StoppedWithPendingDownloads).Returns(true);
            var viewModel = CreateViewModel(_fakeTrackFactory.CreateTrackWithId(1));
            viewModel.SetDurationLabel("17 min 9 sek");

            viewModel.SimulateQueueFinished(succeeded: false);

            // The iOS labels are single line with tail truncation, so the reason replaces the duration
            // instead of being appended to it.
            Assert.AreEqual(Translations.TrackCollectionViewModel_DownloadPausedNoConnection, viewModel.HeaderSubtitle);
        }

        [Test]
        public void The_Subtitle_Is_Just_The_Duration_When_There_Is_Nothing_To_Report()
        {
            var viewModel = CreateViewModel(_fakeTrackFactory.CreateTrackWithId(1));
            viewModel.SetDurationLabel("17 min 9 sek");

            Assert.AreEqual("17 min 9 sek", viewModel.HeaderSubtitle);
        }

        private class TestDownloadViewModel : DownloadViewModel
        {
            private readonly IList<Track> _tracks;

            public TestDownloadViewModel(
                IStorageManager storageManager,
                IDocumentFilter documentFilter,
                IDownloadQueue downloadQueue,
                IConnection connection,
                INetworkSettings networkSettings,
                IUnavailableTrackRegistry unavailableTracks,
                ILogger logger,
                IList<Track> tracks)
                : base(storageManager, documentFilter, downloadQueue, connection, networkSettings, unavailableTracks, logger)
            {
                _tracks = tracks;
            }

            public override string Title => "Test";

            public override string Image => null;

            protected override IEnumerable<IDownloadable> DownloadableTracks => _tracks;

            public void SetOfflineAvailable(bool value) => IsOfflineAvailable = value;

            public void SetDurationLabel(string value) => DurationLabel = value;

            public void SimulateDownloadStarted()
                => HandleFileDownloadStartedMessage(new FileDownloadStartedMessage(this, 1));

            public void SimulateDownloadCanceled(System.Exception exception)
                => HandleFileDownloadCanceledMessage(new FileDownloadCanceledMessage(this, exception));

            public void SimulateQueueFinished(bool succeeded)
                => HandleDownloadQueueFinishedMessage(new QueueFinishedMessage(this, succeeded));

            protected override Task DownloadAction() => Task.CompletedTask;

            protected override Task DeleteAction() => Task.CompletedTask;

            protected override Task<long> CalculateApproximateDownloadSize() => Task.FromResult(0L);

            public override Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
                => Task.FromResult(Enumerable.Empty<IDocumentPO>());
        }
    }
}

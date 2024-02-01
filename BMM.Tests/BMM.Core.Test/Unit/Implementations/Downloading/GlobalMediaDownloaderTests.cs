using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.Security;
using Moq;
using MvvmCross.Plugin.Messenger;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.Downloading
{
    [TestFixture]
    public class GlobalMediaDownloaderTests
    {
        private Mock<IDownloadQueue> _downloadQueue;
        private Mock<IUserStorage> _userStorage;
        private Mock<IStorageManager> _storageManager;
        private Mock<IExceptionHandler> _exceptionHandler;
        private Mock<IAnalytics> _analytics;
        private Mock<INetworkSettings> _networkSettings;
        private Mock<IConnection> _connection;
        private Mock<IMvxMessenger> _messenger;
        private Mock<IBMMClient> _client;
        private Mock<IAppContentLogger> _appContentLogger;
        private Mock<IGlobalTrackProvider> _globalTrackProvider;
        private readonly FakeTrackFactory _fakeTrackFactory = new FakeTrackFactory();

        private List<Track> TrackOfflineTracks => new List<Track>
        {
            _fakeTrackFactory.CreateTrackWithId(5),
            _fakeTrackFactory.CreateTrackWithId(6),
            _fakeTrackFactory.CreateTrackWithId(3),
        };

        [SetUp]
        public void Init()
        {
            _userStorage = new Mock<IUserStorage>();
            _storageManager = new Mock<IStorageManager>();
            _exceptionHandler = new Mock<IExceptionHandler>();
            _analytics = new Mock<IAnalytics>();
            _networkSettings = new Mock<INetworkSettings>();
            _connection = new Mock<IConnection>();
            _messenger = new Mock<IMvxMessenger>();
            _client = new Mock<IBMMClient>();
            _downloadQueue = new Mock<IDownloadQueue>();
            _appContentLogger = new Mock<IAppContentLogger>();
            _globalTrackProvider = new Mock<IGlobalTrackProvider>();

            var discoverClient = new Mock<IDiscoverClient>();
            discoverClient.Setup(x => x.GetDocuments(It.IsAny<int?>(), It.IsAny<AppTheme>(), It.IsAny<CachePolicy>()))
                .ReturnsAsync(new List<Document>());
            _client.Setup(x => x.Discover).Returns(discoverClient.Object);
            _connection.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Online);
            _networkSettings.Setup(x => x.GetMobileNetworkDownloadAllowed()).ReturnsAsync(true);
            _connection.Setup(x => x.IsUsingNetworkWithoutExtraCosts()).Returns(true);
            _userStorage.Setup(x => x.GetUser()).Returns(new User {Username = "ola.normann", PersonId = 42526});
            _storageManager.Setup(x => x.SelectedStorage.IdsOfDownloadedFiles()).Returns(new List<string> {"1_nb", "2_en", "3_nb"});
            _storageManager.Setup(x => x.SelectedStorage.PathsOfDownloadedFiles()).Returns(new List<string> {"1_nb", "2_en", "3_nb"});
        }

        public GlobalMediaDownloader CreateGlobalMediaDownloader()
        {
            var userStorage = new Mock<IUserStorage>();
            var config = new Mock<IFirebaseRemoteConfig>();
            config.Setup(x => x.SendAgeToDiscover).Returns(false);
            var deviceInfo = new Mock<IDeviceInfo>();
            return new GlobalMediaDownloader(_storageManager.Object,
                _exceptionHandler.Object,
                _analytics.Object,
                _networkSettings.Object,
                _connection.Object,
                _messenger.Object,
                _client.Object,
                _downloadQueue.Object,
                _appContentLogger.Object,
                _globalTrackProvider.Object,
                userStorage.Object,
                config.Object,
                deviceInfo.Object
            );
        }

        [Test]
        public async Task SynchronizeOfflineTracks_ShouldSynchronizeTracksAndDownloadThem()
        {
            //Arrange
            var trackOfflineTracks = new List<Track>
            {
                _fakeTrackFactory.CreateTrackWithId(5),
                _fakeTrackFactory.CreateTrackWithId(6),
                _fakeTrackFactory.CreateTrackWithId(3),
            };

            _globalTrackProvider
                .Setup(x => x.GetTracksSupposedToBeDownloaded())
                .ReturnsAsync(trackOfflineTracks);

            GlobalMediaDownloader globalMediaDownloader = CreateGlobalMediaDownloader();

            //Act
            await globalMediaDownloader.SynchronizeOfflineTracks();

            //Assert
            _downloadQueue.Verify(x => x.DequeueAllExcept(It.IsAny<IEnumerable<IDownloadable>>()), Times.Once);
            _downloadQueue.Verify(x => x.StartDownloading(), Times.Once);
            _downloadQueue.Verify(x => x.Enqueue(It.Is<IEnumerable<IDownloadable>>(y => y.Count() == 3)));
        }

        [Test]
        public void SynchronizeOfflineTracks_ShouldNotPerformSynchronizationWhenIsOffline()
        {
            //Arrange
            _globalTrackProvider
                .Setup(x => x.GetTracksSupposedToBeDownloaded())
                .ReturnsAsync(TrackOfflineTracks);
            _connection.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Offline);

            GlobalMediaDownloader globalMediaDownloader = CreateGlobalMediaDownloader();

            //Act
            var synchronizeTask = globalMediaDownloader.SynchronizeOfflineTracks();

            //Assert
            Assert.True(synchronizeTask.IsCompleted);
            _downloadQueue.Verify(x => x.DequeueAllExcept(It.IsAny<IEnumerable<IDownloadable>>()), Times.Never);
            _downloadQueue.Verify(x => x.Enqueue(It.IsAny<IEnumerable<IDownloadable>>()), Times.Never);
            _downloadQueue.Verify(x => x.StartDownloading(), Times.Never);
        }

        [Test]
        public void SynchronizeOfflineTracks_GetOfflineTracksShouldConcatPodcastAndOfflineTrackCollectionLists()
        {
            //Arrange
            _globalTrackProvider
                .Setup(x => x.GetTracksSupposedToBeDownloaded())
                .ReturnsAsync(TrackOfflineTracks);
            _connection.Setup(x => x.IsUsingNetworkWithoutExtraCosts()).Returns(false);

            GlobalMediaDownloader globalMediaDownloader = CreateGlobalMediaDownloader();

            //Act
            var synchronizeTask = globalMediaDownloader.SynchronizeOfflineTracks();

            //Assert
            Assert.True(synchronizeTask.IsCompleted);
            _downloadQueue.Verify(x => x.DequeueAllExcept(It.IsAny<IEnumerable<IDownloadable>>()), Times.Once);
            _downloadQueue.Verify(x => x.Enqueue(It.Is<IEnumerable<IDownloadable>>(y => y.Count() == 3)));
            _downloadQueue.Verify(x => x.StartDownloading(), Times.Once);
        }
    }
}
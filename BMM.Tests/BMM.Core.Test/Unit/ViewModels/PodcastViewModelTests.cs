using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Akavache;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Contributors.Interfaces;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.TrackListenedObservation;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels;
using Moq;
using MvvmCross.Base;
using MvvmCross.Localization;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.Tests;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class PodcastViewModelTests : BaseViewModelTests
    {
        private Mock<IBMMClient> _client;
        private Mock<IExceptionHandler> _exceptionHandler;
        private Mock<IPodcastOfflineManager> _podcastDownloaderMock;
        private Mock<IConnection> _connectionMock;
        private Mock<IGlobalMediaDownloader> _mediaDownloaderMock;
        private Mock<IUserDialogs> _userDialogsMock;
        private Mock<IToastDisplayer> _toastDisplayerMock;
        private Mock<INetworkSettings> _networkSettingsMock;
        private Mock<IBMMLanguageBinder> _languageBinder;
        private Mock<IViewModelAwareViewPresenter> _viewPresenter;
        private Mock<IDownloadedTracksOnlyFilter> _downloadedOnlyFilterMock;
        private Mock<IShufflePodcastAction> _shufflePodcastActionMock;
        private IBlobCache _inMemoryCache;
        private Mock<ITrackPOFactory> _trackPOFactoryMock;
        private Mock<IDocumentsPOFactory> _documentsPOFactoryMock;
        private Mock<ISettingsStorage> _settingsStorageMock;
        private Mock<IMediaPlayer> _mediaPlayerMock;

        public override void SetUp()
        {
            base.SetUp();
            _client = new Mock<IBMMClient>();
            _exceptionHandler = new Mock<IExceptionHandler>();
            _podcastDownloaderMock = new Mock<IPodcastOfflineManager>();
            _connectionMock = new Mock<IConnection>();
            _mediaDownloaderMock = new Mock<IGlobalMediaDownloader>();
            _userDialogsMock = new Mock<IUserDialogs>();
            _toastDisplayerMock = new Mock<IToastDisplayer>();
            _networkSettingsMock = new Mock<INetworkSettings>();
            _languageBinder = new Mock<IBMMLanguageBinder>();
            _inMemoryCache = new InMemoryBlobCache();
            _viewPresenter = new Mock<IViewModelAwareViewPresenter>();
            _downloadedOnlyFilterMock = new Mock<IDownloadedTracksOnlyFilter>();
            _shufflePodcastActionMock = new Mock<IShufflePodcastAction>();
            _trackPOFactoryMock = new Mock<ITrackPOFactory>();
            _documentsPOFactoryMock = new Mock<IDocumentsPOFactory>();
            _settingsStorageMock = new Mock<ISettingsStorage>();
            _mediaPlayerMock = new Mock<IMediaPlayer>();
            _client.Setup(x => x.Podcast.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>()))
                .ReturnsAsync(new Podcast() {Cover = "CoverStream", DocumentType = DocumentType.Podcast, Id = 1, Language = "en-US", Title = "Test"});
            _podcastDownloaderMock.Setup(x => x.FollowPodcast(It.IsAny<Podcast>()));
            _podcastDownloaderMock.Setup(x => x.UnfollowPodcast(It.IsAny<Podcast>()));
            _podcastDownloaderMock.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(false);
            _connectionMock.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Online);
            _languageBinder.Setup(x => x.GetText(It.IsAny<string>(), It.IsAny<object[]>())).Returns("test string");

            var mockMvxMessenger = new Mock<IMvxMessenger>();
            Ioc.RegisterSingleton(_client.Object);
            Ioc.RegisterSingleton(_exceptionHandler.Object);
            Ioc.RegisterSingleton(mockMvxMessenger.Object);
            Ioc.RegisterSingleton(new Mock<IMvxTextProvider>().Object);
            Ioc.RegisterSingleton(new Mock<INotificationCenter>(MockBehavior.Strict).Object);
            Ioc.RegisterSingleton(_inMemoryCache);
            Ioc.RegisterSingleton(_viewPresenter.Object);
            Ioc.RegisterSingleton(new Mock<IMediaQueue>().Object);
            Ioc.RegisterSingleton(new Mock<IMediaPlayer>().Object);
            Ioc.RegisterSingleton(new Mock<IMvxNavigationService>().Object);
            Ioc.RegisterSingleton(_connectionMock.Object);
        }

        public PodcastViewModel CreatePodcastViewModel()
        {
            var viewModel = new PodcastViewModel(
                _podcastDownloaderMock.Object,
                _connectionMock.Object,
                _mediaDownloaderMock.Object,
                _userDialogsMock.Object,
                _toastDisplayerMock.Object,
                _downloadedOnlyFilterMock.Object,
                _networkSettingsMock.Object,
                _shufflePodcastActionMock.Object,
                _trackPOFactoryMock.Object,
                _documentsPOFactoryMock.Object,
                _settingsStorageMock.Object,
                _mediaPlayerMock.Object,
                new Mock<IFirebaseRemoteConfig>().Object);

            viewModel.TextSource = _languageBinder.Object;
            return viewModel;
        }

        [Test]
        public void Constructor_ShouldSetupPropertiesCorrectly()
        {
            // Arrange
            _client.Setup(x => x.Podcast.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>())).ReturnsAsync(new Podcast() { Cover = null, DocumentType = DocumentType.Podcast, Id = 1, Language = "en-US", Title = "Test" });
            _podcastDownloaderMock.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(false);

            // Act
            var podcastViewModel = CreatePodcastViewModel();

            // Assert
            Assert.IsNotNull(podcastViewModel);
            Assert.AreEqual(podcastViewModel.IsDownloading, false);
            Assert.IsNotNull(podcastViewModel.ToggleFollowingCommand);
        }

        [Test]
        public void FollowButtonText_ShouldDisplayFollowingTextWithUppercase()
        {
            // Arrange
            var followButtonText = "text";
            _podcastDownloaderMock.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(true);
            _languageBinder.Setup(x => x[It.IsAny<string>()]).Returns(followButtonText);

            var podcastViewModel = CreatePodcastViewModel();

            // Act
            var resultString = podcastViewModel.FollowButtonText;

            // Assert
            Assert.AreEqual(resultString, followButtonText.ToUpper());
        }

        [Test]
        public async Task Init_ShouldCallAllRequiredMethods()
        {
            // Arrange
            _podcastDownloaderMock.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(true);

            var podcastViewModel = CreatePodcastViewModel();

            // Act
            await podcastViewModel.Initialize();

            // Assert
            _client.Verify(x => x.Podcast.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>()), Times.Once);
            _podcastDownloaderMock.Verify(x => x.IsFollowing(It.IsAny<Podcast>()), Times.Once);
            _podcastDownloaderMock.Verify(x => x.FollowPodcast(It.IsAny<Podcast>()), Times.Once);
            Assert.AreEqual(podcastViewModel.IsFollowing, true);
        }

        [Test]
        public async Task Init_ShouldNotFollowPodcastWhenWeAreNotFollowingPodcastOnStart()
        {
            // Arrange
            _client.Setup(x => x.Podcast.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>())).ReturnsAsync(new Podcast() { Cover = null, DocumentType = DocumentType.Podcast, Id = 1, Language = "en-US", Title = "Test" });

            var podcastViewModel = CreatePodcastViewModel();

            // Act
            await podcastViewModel.Initialize();

            // Assert
            _client.Verify(x => x.Podcast.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>()), Times.Once);
            _podcastDownloaderMock.Verify(x => x.IsFollowing(It.IsAny<Podcast>()), Times.Once);
            _podcastDownloaderMock.Verify(x => x.FollowPodcast(It.IsAny<Podcast>()), Times.Never);
            Assert.AreEqual(podcastViewModel.IsFollowing, false);
        }

        [Test]
        public async Task ToggleFollowingChange_ShouldDisplayErrorWhenThereIsNoConnection()
        {
            // Arrange
            _client.Setup(x => x.Podcast.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>())).ReturnsAsync(new Podcast() { Cover = null, DocumentType = DocumentType.Podcast, Id = 1, Language = "en-US", Title = "Test" });
            _connectionMock.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Offline);

            var podcastViewModel = CreatePodcastViewModel();
            await podcastViewModel.Initialize();

            // Act
            await podcastViewModel.ToggleFollowingCommand.ExecuteAsync();

            // Assert
            _userDialogsMock.Verify(x => x.AlertAsync(It.IsAny<string>(), null, null, null), Times.Once);
            Assert.AreEqual(podcastViewModel.IsFollowing, false);
        }

        [Test]
        public async Task ToggleFollowingChange_ShouldUnFollowPodcastWhenToggleRaised()
        {
            // Arrange
            _podcastDownloaderMock.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(true);
            _userDialogsMock.Setup(x => x.ConfirmAsync(It.IsAny<string>(), null, null, null, null)).ReturnsAsync(true);

            var podcastViewModel = CreatePodcastViewModel();
            await podcastViewModel.Initialize();

            // Act
            await podcastViewModel.ToggleFollowingCommand.ExecuteAsync();

            // Assert
            _podcastDownloaderMock.Verify(x => x.IsFollowing(It.IsAny<Podcast>()), Times.Once);
            _podcastDownloaderMock.Verify(x => x.UnfollowPodcast(It.IsAny<Podcast>()), Times.Once);
            Assert.AreEqual(podcastViewModel.IsFollowing, false);
        }

        [Test]
        public async Task ToggleFollowingChange_ShouldPreventFromUnFollowingWhenUserNotConfirm()
        {
            // Arrange
            _podcastDownloaderMock.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(true);
            _userDialogsMock.Setup(x => x.ConfirmAsync(It.IsAny<string>(), null, null, null, null)).ReturnsAsync(false);

            var podcastViewModel = CreatePodcastViewModel();
            await podcastViewModel.Initialize();

            // Act
            await podcastViewModel.ToggleFollowingCommand.ExecuteAsync();

            // Assert
            _podcastDownloaderMock.Verify(x => x.IsFollowing(It.IsAny<Podcast>()), Times.Once);
            _podcastDownloaderMock.Verify(x => x.UnfollowPodcast(It.IsAny<Podcast>()), Times.Never);
            Assert.AreEqual(podcastViewModel.IsFollowing, true);
        }

        [Test]
        public async Task ToggleFollowingChange_ShouldFollowUnFollowedPodcastWhenToggleRaised()
        {
            // Arrange
            _podcastDownloaderMock.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(false);
            _connectionMock.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Online);
            _userDialogsMock.Setup(x => x.ConfirmAsync(It.IsAny<string>(), null, null, null, null)).ReturnsAsync(true);

            var podcastViewModel = CreatePodcastViewModel();
            await podcastViewModel.Initialize();

            // Act
            await podcastViewModel.ToggleFollowingCommand.ExecuteAsync();

            // Assert
            _podcastDownloaderMock.Verify(x => x.IsFollowing(It.IsAny<Podcast>()), Times.Once);
            _podcastDownloaderMock.Verify(x => x.FollowPodcast(It.IsAny<Podcast>()), Times.Once);
            Assert.AreEqual(podcastViewModel.IsFollowing, true);
        }
    }
}

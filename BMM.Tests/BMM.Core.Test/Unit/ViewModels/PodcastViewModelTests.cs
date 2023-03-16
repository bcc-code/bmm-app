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
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.TrackListenedObservation;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer.Abstractions;
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
    public class PodcastViewModelTests : MvxIoCSupportingTest
    {
        private Mock<IBMMClient> _client;
        private Mock<IExceptionHandler> _exceptionHandler;
        private Mock<IPodcastOfflineManager> _podcastDownloader;
        private Mock<IConnection> _connection;
        private Mock<IGlobalMediaDownloader> _mediaDownloader;
        private Mock<IUserDialogs> _userDialogs;
        private Mock<IToastDisplayer> _toastDisplayer;
        private Mock<INetworkSettings> _networkSettings;
        private Mock<IBMMLanguageBinder> _languageBinder;
        private Mock<IViewModelAwareViewPresenter> _viewPresenter;
        private Mock<IDownloadedTracksOnlyFilter> _downloadedOnlyFilter;
        private Mock<IShufflePodcastAction> _shufflePodcastAction;
        private IBlobCache _inMemoryCache;
        private Mock<ITrackPOFactory> _trackPOFactory;
        private Mock<IDocumentsPOFactory> _documentsPOFactory;

        [SetUp]
        public void Init()
        {
            _client = new Mock<IBMMClient>();
            _exceptionHandler = new Mock<IExceptionHandler>();
            _podcastDownloader = new Mock<IPodcastOfflineManager>();
            _connection = new Mock<IConnection>();
            _mediaDownloader = new Mock<IGlobalMediaDownloader>();
            _userDialogs = new Mock<IUserDialogs>();
            _toastDisplayer = new Mock<IToastDisplayer>();
            _networkSettings = new Mock<INetworkSettings>();
            _languageBinder = new Mock<IBMMLanguageBinder>();
            _inMemoryCache = new InMemoryBlobCache();
            _viewPresenter = new Mock<IViewModelAwareViewPresenter>();
            _downloadedOnlyFilter = new Mock<IDownloadedTracksOnlyFilter>();
            _shufflePodcastAction = new Mock<IShufflePodcastAction>();
            _trackPOFactory = new Mock<ITrackPOFactory>();
            _documentsPOFactory = new Mock<IDocumentsPOFactory>();
            var _mainThreadDispatcher = new Mock<IMvxMainThreadDispatcher>();
            var _mainThreadAsyncDispatcher = new Mock<IMvxMainThreadAsyncDispatcher>();

            _mainThreadDispatcher.Setup(x => x.RequestMainThreadAction(It.IsAny<Action>(), It.IsAny<bool>())).Returns(true);
            _mainThreadAsyncDispatcher.Setup(x => x.ExecuteOnMainThreadAsync(It.IsAny<Action>(), It.IsAny<bool>()));
            _client.Setup(x => x.Podcast.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>()))
                .ReturnsAsync(new Podcast() {Cover = "CoverStream", DocumentType = DocumentType.Podcast, Id = 1, Language = "en-US", Title = "Test"});
            _podcastDownloader.Setup(x => x.FollowPodcast(It.IsAny<Podcast>()));
            _podcastDownloader.Setup(x => x.UnfollowPodcast(It.IsAny<Podcast>()));
            _podcastDownloader.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(false);
            _connection.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Online);
            _languageBinder.Setup(x => x.GetText(It.IsAny<string>(), It.IsAny<object[]>())).Returns("test string");

            Setup();
            base.AdditionalSetup();
            var mockMvxMessenger = new Mock<IMvxMessenger>();
            Ioc.RegisterSingleton(_client.Object);
            Ioc.RegisterSingleton(_exceptionHandler.Object);
            Ioc.RegisterSingleton(mockMvxMessenger.Object);
            Ioc.RegisterSingleton(new Mock<IMvxTextProvider>().Object);
            Ioc.RegisterSingleton(new Mock<INotificationCenter>(MockBehavior.Strict).Object);
            Ioc.RegisterSingleton(_inMemoryCache);
            Ioc.RegisterSingleton(_viewPresenter.Object);
            Ioc.RegisterSingleton(_mainThreadDispatcher.Object);
            Ioc.RegisterSingleton(_mainThreadAsyncDispatcher.Object);
            Ioc.RegisterSingleton(new Mock<IMediaQueue>().Object);
            Ioc.RegisterSingleton(new Mock<IMediaPlayer>().Object);
            Ioc.RegisterSingleton(new Mock<IMvxNavigationService>().Object);
            Ioc.RegisterSingleton(_connection.Object);
        }

        public PodcastViewModel CreatePodcastViewModel()
        {
            var viewModel = new PodcastViewModel(
                _podcastDownloader.Object,
                _connection.Object,
                _mediaDownloader.Object,
                _userDialogs.Object,
                _toastDisplayer.Object,
                _downloadedOnlyFilter.Object,
                _networkSettings.Object,
                _shufflePodcastAction.Object,
                _trackPOFactory.Object,
                _documentsPOFactory.Object);

            viewModel.TextSource = _languageBinder.Object;
            return viewModel;
        }

        [Test]
        public void Constructor_ShouldSetupPropertiesCorrectly()
        {
            // Arrange
            _client.Setup(x => x.Podcast.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>())).ReturnsAsync(new Podcast() { Cover = null, DocumentType = DocumentType.Podcast, Id = 1, Language = "en-US", Title = "Test" });
            _podcastDownloader.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(false);

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
            _podcastDownloader.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(true);
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
            _podcastDownloader.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(true);

            var podcastViewModel = CreatePodcastViewModel();

            // Act
            await podcastViewModel.Initialize();

            // Assert
            _client.Verify(x => x.Podcast.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>()), Times.Once);
            _podcastDownloader.Verify(x => x.IsFollowing(It.IsAny<Podcast>()), Times.Once);
            _podcastDownloader.Verify(x => x.FollowPodcast(It.IsAny<Podcast>()), Times.Once);
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
            _podcastDownloader.Verify(x => x.IsFollowing(It.IsAny<Podcast>()), Times.Once);
            _podcastDownloader.Verify(x => x.FollowPodcast(It.IsAny<Podcast>()), Times.Never);
            Assert.AreEqual(podcastViewModel.IsFollowing, false);
        }

        [Test]
        public async Task ToggleFollowingChange_ShouldDisplayErrorWhenThereIsNoConnection()
        {
            // Arrange
            _client.Setup(x => x.Podcast.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>())).ReturnsAsync(new Podcast() { Cover = null, DocumentType = DocumentType.Podcast, Id = 1, Language = "en-US", Title = "Test" });
            _connection.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Offline);

            var podcastViewModel = CreatePodcastViewModel();
            await podcastViewModel.Initialize();

            // Act
            await podcastViewModel.ToggleFollowingCommand.ExecuteAsync();

            // Assert
            _userDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), null, null, null), Times.Once);
            Assert.AreEqual(podcastViewModel.IsFollowing, false);
        }

        [Test]
        public async Task ToggleFollowingChange_ShouldUnFollowPodcastWhenToggleRaised()
        {
            // Arrange
            _podcastDownloader.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(true);
            _userDialogs.Setup(x => x.ConfirmAsync(It.IsAny<string>(), null, null, null, null)).ReturnsAsync(true);

            var podcastViewModel = CreatePodcastViewModel();
            await podcastViewModel.Initialize();

            // Act
            await podcastViewModel.ToggleFollowingCommand.ExecuteAsync();

            // Assert
            _podcastDownloader.Verify(x => x.IsFollowing(It.IsAny<Podcast>()), Times.Once);
            _podcastDownloader.Verify(x => x.UnfollowPodcast(It.IsAny<Podcast>()), Times.Once);
            Assert.AreEqual(podcastViewModel.IsFollowing, false);
        }

        [Test]
        public async Task ToggleFollowingChange_ShouldPreventFromUnFollowingWhenUserNotConfirm()
        {
            // Arrange
            _podcastDownloader.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(true);
            _userDialogs.Setup(x => x.ConfirmAsync(It.IsAny<string>(), null, null, null, null)).ReturnsAsync(false);

            var podcastViewModel = CreatePodcastViewModel();
            await podcastViewModel.Initialize();

            // Act
            await podcastViewModel.ToggleFollowingCommand.ExecuteAsync();

            // Assert
            _podcastDownloader.Verify(x => x.IsFollowing(It.IsAny<Podcast>()), Times.Once);
            _podcastDownloader.Verify(x => x.UnfollowPodcast(It.IsAny<Podcast>()), Times.Never);
            Assert.AreEqual(podcastViewModel.IsFollowing, true);
        }

        [Test]
        public async Task ToggleFollowingChange_ShouldFollowUnFollowedPodcastWhenToggleRaised()
        {
            // Arrange
            _podcastDownloader.Setup(x => x.IsFollowing(It.IsAny<Podcast>())).Returns(false);
            _connection.Setup(x => x.GetStatus()).Returns(ConnectionStatus.Online);
            _userDialogs.Setup(x => x.ConfirmAsync(It.IsAny<string>(), null, null, null, null)).ReturnsAsync(true);

            var podcastViewModel = CreatePodcastViewModel();
            await podcastViewModel.Initialize();

            // Act
            await podcastViewModel.ToggleFollowingCommand.ExecuteAsync();

            // Assert
            _podcastDownloader.Verify(x => x.IsFollowing(It.IsAny<Podcast>()), Times.Once);
            _podcastDownloader.Verify(x => x.FollowPodcast(It.IsAny<Podcast>()), Times.Once);
            Assert.AreEqual(podcastViewModel.IsFollowing, true);
        }
    }
}

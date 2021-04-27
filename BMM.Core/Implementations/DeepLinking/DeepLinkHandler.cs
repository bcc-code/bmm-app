using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Security;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels;
using MvvmCross;
using MvvmCross.Localization;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace BMM.Core.Implementations.DeepLinking
{
    public class DeepLinkHandler : IDeepLinkHandler
    {
        private const string PlaybackOriginName = "DeepLink";

        private readonly IBMMClient _client;
        private readonly IMvxNavigationService _navigationService;
        private readonly IViewModelAwareViewPresenter _viewPresenter;
        private readonly IAnalytics _analytics;
        private readonly IUserDialogs _userDialogs;
        private readonly ILogger _logger;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IUserAuthChecker _authChecker;

        private readonly IMvxLanguageBinder _textSource;
        private readonly IMvxLanguageBinder _globalTextSource;

        private readonly IList<IDeepLinkParser> _links;

        public DeepLinkHandler(
            IBMMClient client,
            IMvxNavigationService navigationService,
            IViewModelAwareViewPresenter viewPresenter,
            IAnalytics analytics,
            IUserDialogs userDialogs,
            ILogger logger,
            IExceptionHandler exceptionHandler,
            IUserAuthChecker authChecker
        )
        {
            _client = client;
            _navigationService = navigationService;
            _viewPresenter = viewPresenter;
            _analytics = analytics;
            _userDialogs = userDialogs;
            _logger = logger;
            _exceptionHandler = exceptionHandler;
            _authChecker = authChecker;
            _textSource = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "DeepLinkHandler");
            _globalTextSource = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "Global");

            _links = new List<IDeepLinkParser>
            {
                new RegexDeepLink("^/copyright$", NavigateTo<CopyrightViewModel>),
                new RegexDeepLink("^/archive$", () => _navigationService.Navigate<LibraryViewModel, LibraryViewModel.Tab>(LibraryViewModel.Tab.Archive)),
                new RegexDeepLink("^/daily-fra-kaare$", PlayFraKaare),
                new RegexDeepLink("^/playlist/latest$", NavigateTo<ExploreNewestViewModel>),
                new RegexDeepLink("^/speeches$", NavigateTo<ExploreRecentSpeechesViewModel>),
                new RegexDeepLink("^/music$", NavigateTo<ExploreRecentMusicViewModel>),
                new RegexDeepLink("^/contributors$", NavigateTo<ExploreContributorsViewModel>),
                new RegexDeepLink("^/featured$", NavigateTo<CuratedPlaylistsViewModel>),
                new RegexDeepLinkWithParameters("^/playlist/private/(?<id>[0-9]+)(/(?<name>.*))?$", OpenTrackCollection),
                new RegexDeepLinkWithParameters("^/playlist/contributor/(?<id>[0-9]+)(/(?<name>.*))?$", OpenContributor),
                new RegexDeepLinkWithParameters("^/playlist/podcast/(?<id>[0-9]+)(/(?<name>.*))?$", OpenPodcast),
                new RegexDeepLinkWithParameters("^/album/(?<id>[0-9]+)$", OpenAlbum),
                new TrackLinkParser("^/track/(?<id>[0-9]+)(/(?<language>.*))?$", (id, name, startTimeInMs) => PlayTrackById(id, startTimeInMs)),
                new RegexDeepLink("^/$", DoNothing)
            };
        }

        /// <summary>
        /// Navigates to the ViewModel if it is not already shown.
        /// ToDo: We can't detect if a user is already looking at the correct view if parameters are involved.
        /// </summary>
        private async Task NavigateTo<T>() where T : IMvxViewModel
        {
            if (!_viewPresenter.IsViewModelShown<T>())
            {
                await _navigationService.Navigate<T>();
            }
        }

        private Task OpenAlbum(int id, string unusedName)
        {
            return _navigationService.Navigate<AlbumViewModel, int>(id);
        }

        private Task OpenContributor(int id, string unusedName)
        {
            return _navigationService.Navigate<ContributorViewModel, int>(id);
        }

        private Task OpenPodcast(int id, string name)
        {
            return _navigationService.Navigate<PodcastViewModel, Podcast>(new Podcast {Id = id, Title = name});
        }

        private Task OpenTrackCollection(int id, string name)
        {
            return _navigationService.Navigate<TrackCollectionViewModel, TrackCollection>(new TrackCollection {Id = id, Name = name});
        }

        private Task DoNothing()
        {
            // This link is only supposed to open the app, which already happened. So we don't have to do anything.
            return Task.CompletedTask;
        }

        private async Task PlayFraKaare()
        {
            var items = await _client.Podcast.GetTracks(FraKaareTeaserViewModel.FraKårePodcastId, CachePolicy.UseCacheAndWaitForUpdates);
            var trackToPlay = items.Take(1).ToList();
            await PlayTracks(trackToPlay, PlaybackOriginName);
        }

        private async Task PlayTrackById(int id, long startTimeInMs = 0)
        {
            var requestedTrack = await _client.Tracks.GetById(id);
            await PlayTracks(new[] { requestedTrack }, PlaybackOriginName, startTimeInMs);
        }

        public bool Open(Uri uri)
        {
            if (!IsBmmUrl(uri))
                return false;

            _analytics.LogEvent("deep link opened", new Dictionary<string, object> {{"uri", uri.AbsolutePath}});

            foreach (var link in _links)
            {
                if (link.CanNavigateTo(uri, out var action))
                {
                    _exceptionHandler.FireAndForgetOnMainThread(async () =>
                    {
                        try
                        {
                            if (await _authChecker.IsUserAuthenticated())
                                await action();
                        }
                        catch (InternetProblemsException)
                        {
                            await ShowErrorMessage(uri, "InternetConnectionOffline");
                        }
                        catch (NotFoundException)
                        {
                            await ShowErrorMessage(uri, "ItemNotFound");
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Unexpected error when opening deep link", ex.Message, ex);
                            await ShowErrorMessage(uri);
                        }
                    });
                    return true;
                }
            }

            return false;
        }

        private bool IsBmmUrl(Uri uri)
        {
            return uri.AbsoluteUri.Contains(GlobalConstants.BmmUrl);
        }

        private Task ShowErrorMessage(Uri uri, string additionalGlobalTextSourceId = null)
        {
            var errorMessage = _textSource.GetText("ErrorMessage", uri.AbsoluteUri);
            var errorTitle = _textSource.GetText("ErrorTitle");
            if (additionalGlobalTextSourceId != null)
            {
                errorMessage = errorMessage + "\n" + _globalTextSource.GetText(additionalGlobalTextSourceId);
            }

            return _userDialogs.AlertAsync(errorMessage, errorTitle);
        }

        private Task PlayTracks(IList<Track> list, string playbackOrigin, long startTimeInMs = 0)
        {
            var mediaPlayer = Mvx.IoCProvider.Resolve<IMediaPlayer>();
            return mediaPlayer.Play(list.OfType<IMediaTrack>().ToList(), list.First(), playbackOrigin, startTimeInMs);
        }
    }
}

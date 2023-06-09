using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Constants;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.DeepLinking.Base.Interfaces;
using BMM.Core.Implementations.DeepLinking.Parameters;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.Security;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace BMM.Core.Implementations.DeepLinking
{
    public class DeepLinkHandler : IDeepLinkHandler
    {
        public static readonly string Music = "music";
        private readonly string _baseBrowsePath = "browse/";
        private readonly string[] _tileCollectionPaths = new[]
        {
            EndpointConstants.BrowsePodcast,
            EndpointConstants.BrowseMusic
        };

        private const string PlaybackOriginName = "DeepLink";

        private readonly IBMMClient _client;
        private readonly IMvxNavigationService _navigationService;
        private readonly IViewModelAwareViewPresenter _viewPresenter;
        private readonly IAnalytics _analytics;
        private readonly IUserDialogs _userDialogs;
        private readonly ILogger _logger;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IUserAuthChecker _authChecker;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;
        private readonly IRememberedQueueInfoService _rememberedQueueInfoService;

        private readonly IList<IDeepLinkParser> _links;
        private bool _readyToHandleDeepLink;
        private Uri _pendingDeepLink;

        public DeepLinkHandler(
            IBMMClient client,
            IMvxNavigationService navigationService,
            IViewModelAwareViewPresenter viewPresenter,
            IAnalytics analytics,
            IUserDialogs userDialogs,
            ILogger logger,
            IExceptionHandler exceptionHandler,
            IUserAuthChecker authChecker,
            IBMMLanguageBinder bmmLanguageBinder,
            IRememberedQueueInfoService rememberedQueueInfoService)
        {
            _client = client;
            _navigationService = navigationService;
            _viewPresenter = viewPresenter;
            _analytics = analytics;
            _userDialogs = userDialogs;
            _logger = logger;
            _exceptionHandler = exceptionHandler;
            _authChecker = authChecker;
            _bmmLanguageBinder = bmmLanguageBinder;
            _rememberedQueueInfoService = rememberedQueueInfoService;

            _links = new List<IDeepLinkParser>
            {
                new RegexDeepLink("^/copyright$", NavigateTo<CopyrightViewModel>),
                new RegexDeepLink("^/daily-fra-kaare$", PlayFraKaare),
                new RegexDeepLink("^/playlist/latest$", NavigateTo<ExploreNewestViewModel>),
                new RegexDeepLink("^/speeches$", NavigateTo<ExploreRecentSpeechesViewModel>),
                new RegexDeepLink($"^/{Music}$", NavigateTo<ExploreRecentMusicViewModel>),
                new RegexDeepLink("^/contributors$", NavigateTo<ExploreContributorsViewModel>),
                new RegexDeepLink("^/featured$", NavigateTo<CuratedPlaylistsViewModel>),
                new RegexDeepLink("^/archive$", NavigateTo<LibraryArchiveViewModel>),
                new RegexDeepLink("^/year-in-review$", NavigateTo<YearInReviewViewModel>),
                new RegexDeepLink<IdAndNameParameters>("^/playlist/curated/(?<id>[0-9]+)(/(?<name>.*))?$", OpenCuratedPlaylist),
                new RegexDeepLink<IdAndNameParameters>("^/playlist/private/(?<id>[0-9]+)(/(?<name>.*))?$", OpenTrackCollection),
                new RegexDeepLink<IdAndNameParameters>("^/playlist/podcast/(?<id>[0-9]+)(/(?<name>.*))?$", OpenPodcast),
                new RegexDeepLink<SharingSecretParameters>("^/playlist/shared/(?<sharingsecret>.*)$", OpenSharedTrackCollection ),
                new RegexDeepLink<IdDeepLinkParameters>("^/playlist/contributor/(?<id>[0-9]+)(/(?<name>.*))?$", OpenContributor),
                new RegexDeepLink<IdDeepLinkParameters>("^/album/(?<id>[0-9]+)$", OpenAlbum),
                new TrackLinkParser("^/track/(?<id>[0-9]+)(/(?<language>.*))?$", PlayTrackById),
                new RegexDeepLink<GenericDocumentsViewParameters>("^/browse(/(?<path>.*))?$", OpenGenericDocumentsView),
                new RegexDeepLink("^/$", DoNothing)
            };
        }

        private async Task OpenGenericDocumentsView(GenericDocumentsViewParameters genericDocumentsViewParameters)
        {
            string fullPath = $"{_baseBrowsePath}{genericDocumentsViewParameters.Path}";
            var parameter = new BrowseDetailsParameters(fullPath);

            if (_tileCollectionPaths.Any(p => p.Contains(fullPath)))
                await NavigateTo<BrowseDetailsTilesViewModel, IBrowseDetailsParameters>(parameter);
            else
                await NavigateTo<BrowseDetailsListViewModel, IBrowseDetailsParameters>(parameter);
        }

        /// <summary>
        /// Navigates to the ViewModel if it is not already shown.
        /// ToDo: We can't detect if a user is already looking at the correct view if parameters are involved.
        /// </summary>
        private async Task NavigateTo<T>() where T : IMvxViewModel
        {
            if (!_viewPresenter.IsViewModelShown<T>())
                await _navigationService.Navigate<T>();

            SendEventIfNeeded(typeof(T));
        }

        private void SendEventIfNeeded(Type type)
        {
            if (type == typeof(YearInReviewViewModel))
                _analytics.LogEvent(Event.YearInReviewOpened);
        }

        private async Task NavigateTo<TVm, TParam>(TParam param) where TVm : IMvxViewModel<TParam>
        {
            if (!_viewPresenter.IsViewModelShown<TVm>())
                await _navigationService.Navigate<TVm, TParam>(param);
        }

        private Task OpenSharedTrackCollection(SharingSecretParameters sharingSecretParameters)
        {
            return _navigationService.Navigate<SharedTrackCollectionViewModel, ISharedTrackCollectionParameter>(
                new SharedTrackCollectionParameter(sharingSecretParameters.SharingSecret));
        }

        private Task OpenAlbum(IdDeepLinkParameters idDeepLinksParameters)
        {
            return _navigationService.Navigate<AlbumViewModel, int>(idDeepLinksParameters.Id);
        }

        private Task OpenContributor(IdDeepLinkParameters idLinkParameters)
        {
            return _navigationService.Navigate<ContributorViewModel, int>(idLinkParameters.Id);
        }

        private Task OpenPodcast(IdAndNameParameters deepLinkParameters)
        {
            return _navigationService.Navigate<PodcastViewModel, Podcast>(new Podcast {Id = deepLinkParameters.Id, Title = deepLinkParameters.Name});
        }

        private Task OpenCuratedPlaylist(IdAndNameParameters deepLinkParameters)
        {
            return _navigationService.Navigate<CuratedPlaylistViewModel, Playlist>(new Playlist {Id = deepLinkParameters.Id, Title = deepLinkParameters.Name});
        }

        private Task OpenTrackCollection(IdAndNameParameters deepLinkParameters)
        {
            return _navigationService.Navigate<TrackCollectionViewModel, ITrackCollectionParameter>(new TrackCollectionParameter(deepLinkParameters.Id, deepLinkParameters.Name));
        }

        private Task DoNothing()
        {
            // This link is only supposed to open the app, which already happened. So we don't have to do anything.
            return Task.CompletedTask;
        }

        private async Task PlayFraKaare()
        {
            var items = await _client.Podcast.GetTracks(PodcastsConstants.FraKårePodcastId, CachePolicy.UseCacheAndWaitForUpdates);
            var trackToPlay = items.Take(1).ToList();
            await PlayTracks(trackToPlay, PlaybackOriginName);
        }

        private async Task PlayTrackById(TrackLinkParameters trackLinkParameters)
        {
            var requestedTrack = await _client.Tracks.GetById(trackLinkParameters.Id);
            await PlayTracks(new[] { requestedTrack }, PlaybackOriginName, trackLinkParameters.StartTimeInMs);
        }

        public bool OpenFromInsideOfApp(Uri uri, string origin) => Open(uri, "internal link opened", origin);

        public bool OpenFromOutsideOfApp(Uri uri)
        {
            if (_readyToHandleDeepLink)
                return Open(uri, "deep link opened");

            if (!_links.Any(l => l.PerformCanNavigateTo(uri, out _)))
                return false;
            
            _pendingDeepLink = uri;
            return true;
        }

        public void SetReadyToOpenDeepLinkAndHandlePending()
        {
            _readyToHandleDeepLink = true;
            
            if (_pendingDeepLink == null)
                return;

            OpenFromOutsideOfApp(_pendingDeepLink);
            _pendingDeepLink = null;
        }

        public void SetDeepLinkWillStartPlayerIfNeeded(string deepLink)
        {
            if (WillDeepLinkStartPlayer(new Uri(deepLink))) 
                _rememberedQueueInfoService.SetPlayerHasPendingOperation();
        }
        
        public bool IsBmmUrl(Uri uri)
        {
            return uri.AbsoluteUri.Contains(GlobalConstants.BmmUrl);
        }

        private bool WillDeepLinkStartPlayer(Uri uri)
        {
            return _links.First(l => l is TrackLinkParser).PerformCanNavigateTo(uri, out _);
        }

        private bool Open(Uri uri, string analyticsEventName, string origin = "")
        {
            if (!IsBmmUrl(uri))
                return false;

            SendAnalyticsEvent(uri, analyticsEventName, origin);

            foreach (var link in _links)
            {
                if (link.PerformCanNavigateTo(uri, out var action))
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
                            await ShowErrorMessage(uri, Translations.Global_InternetConnectionOffline);
                        }
                        catch (NotFoundException)
                        {
                            await ShowErrorMessage(uri, Translations.Global_ItemNotFound);
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

        private void SendAnalyticsEvent(Uri uri, string analyticsEventName, string origin)
        {
            var parameters = new Dictionary<string, object>
            {
                { "uri", uri.AbsolutePath }
            };
            
            if (!string.IsNullOrEmpty(origin))
                parameters.Add("origin", origin);

            _analytics.LogEvent(analyticsEventName, parameters);
        }

        private Task ShowErrorMessage(Uri uri, string additionalGlobalTextSourceId = null)
        {
            var errorMessage = _bmmLanguageBinder.GetText(Translations.DeepLinkHandler_ErrorMessage, uri.AbsoluteUri);
            var errorTitle = _bmmLanguageBinder[Translations.DeepLinkHandler_ErrorTitle];
            if (additionalGlobalTextSourceId != null)
            {
                errorMessage = errorMessage + "\n" + _bmmLanguageBinder[additionalGlobalTextSourceId];
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

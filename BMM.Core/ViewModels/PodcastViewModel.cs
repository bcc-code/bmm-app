using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.ChapterStrategy;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.DownloadManager;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.UI;
using BMM.Core.Messages;
using BMM.Core.Translation;
using BMM.Core.ViewModels.MyContent;
using BMM.Core.ViewModels.Parameters;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class PodcastViewModel : PodcastBaseViewModel, IMvxViewModel<Podcast>, IMvxViewModel<StartPlayingPodcast>, ITrackListViewModel
    {
        public IMvxAsyncCommand ToggleFollowingCommand { get; }
        private readonly IPodcastOfflineManager _podcastDownloader;
        private readonly IConnection _connection;
        private readonly IGlobalMediaDownloader _mediaDownloader;
        private readonly IUserDialogs _userDialogs;
        private readonly INetworkSettings _networkSettings;
        private readonly IToastDisplayer _toastDisplayer;
        private readonly WeekOfTheYearChapterStrategy _weekOfTheYearChapterStrategy = new WeekOfTheYearChapterStrategy();
        private readonly AslaksenTagChapterStrategy _aslaksenTagChapterStrategy = new AslaksenTagChapterStrategy();

        protected MvxSubscriptionToken DownloadedEpisodeRemovedToken;

        public int? StartPlayingTrackId { get; set; }

        public bool ShowDownloadButtons => false;

        public bool UseCircularImage => true;

        public bool ShowFollowButtons => true;
        public bool ShowShuffleButton => false;
        public bool ShowPlayButton => true;

        public bool ShowTrackCount => false;

        public bool ShowFollowSharedPlaylistButton => false;

        private bool _isFollowing;
        public bool IsFollowing
        {
            get => _isFollowing;
            private set
            {
                SetProperty(ref _isFollowing, value);
                RaisePropertyChanged(() => FollowButtonText);
            }
        }

        private IEnumerable<IDownloadFile> _downloadingFiles;
        private bool _forceRefreshFromServers;

        protected IEnumerable<IDownloadFile> DownloadingFiles
        {
            get => _downloadingFiles;
            private set => SetProperty(ref _downloadingFiles, value);
        }

        public bool IsDownloading => IsFollowing && DownloadingFiles.Any();

        [Obsolete]
        public string FollowButtonText
        {
            get
            {
                var text = IsFollowing ? TextSource[Translations.PodcastViewModel_Following] : TextSource[Translations.PodcastViewModel_Follow];
                return text.ToUpper();
            }
        }

        public override int CurrentLimit => Podcast?.Id == AslaksenTeaserViewModel.FraBegynnelsenPodcastId ? 60 : base.CurrentLimit;

        public bool ShowSharingInfo => false;

        public string Description => null;

        public bool ShowPlaylistIcon => false;

        public bool ShowImage => true;

        public bool IsDownloaded => IsFollowing && !DownloadingFiles.Any();

        public PodcastViewModel(
            IPodcastOfflineManager podcastDownloader,
            IConnection connection,
            IGlobalMediaDownloader mediaDownloader,
            IUserDialogs userDialogs,
            IToastDisplayer toastDisplayer,
            IDownloadedTracksOnlyFilter downloadedOnlyFilter,
            INetworkSettings networkSettings)
            : base(downloadedOnlyFilter)
        {
            _podcastDownloader = podcastDownloader;
            _connection = connection;
            _mediaDownloader = mediaDownloader;
            _userDialogs = userDialogs;
            _toastDisplayer = toastDisplayer;
            _networkSettings = networkSettings;
            ToggleFollowingCommand = new ExceptionHandlingCommand(async () => await ToggleFollowing());

            var viewPresenter = Mvx.IoCProvider.Resolve<IViewModelAwareViewPresenter>();
            var hideNotDownloadedTracks = viewPresenter.IsViewModelInStack<DownloadedContentViewModel>() ||
                                       viewPresenter.IsViewModelShown<DownloadedContentViewModel>();

            if (hideNotDownloadedTracks)
            {
                // Disable LoadMore when showing only Downloaded tracks
                // In most cases only the first few tracks are downloaded. Allowing LoadMore would feel weird, because no new tracks would appear in the list, since the new tracks are not downloaded
                // Also in most cases we only show 3 downloaded tracks, immediately triggering LoadMore over and over again until all 500+ From Kaare tracks are loaded.
                IsFullyLoaded = true;
            }

            PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case "IsOfflineAvailable":
                        RaisePropertyChanged(() => IsDownloading);
                        RaisePropertyChanged(() => IsDownloaded);
                        break;
                    case "DownloadingFiles":
                        RaisePropertyChanged(() => IsDownloading);
                        RaisePropertyChanged(() => IsDownloaded);

                        foreach (var file in DownloadingFiles)
                        {
                            file.PropertyChanged -= File_PropertyChanged;
                            file.PropertyChanged += File_PropertyChanged;
                        }
                        break;
                }
            };

            DownloadedEpisodeRemovedToken = Messenger.Subscribe<DownloadedEpisodeRemovedMessage>(async message =>
            {
                await RaisePropertyChanged(() => Documents);
            });
        }

        public override async Task Load()
        {
            await base.Load();
            await StartPlayingIfRequested();
        }

        public void Prepare(Podcast podcast)
        {
            Podcast = podcast;
        }

        public void Prepare(StartPlayingPodcast podcast)
        {
            _forceRefreshFromServers = true;
            Podcast = new Podcast {Id = podcast.Id, Title = podcast.Title};
            StartPlayingTrackId = podcast.TrackId;
        }

        public override async Task<IEnumerable<Document>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            if (_forceRefreshFromServers)
            {
                policy = CachePolicy.ForceGetAndUpdateCache;
                _forceRefreshFromServers = false;
            }

            var items = await Client.Podcast.GetTracks(Podcast.Id, policy, size, startIndex);
            var existingDocs = startIndex == 0 ? new List<Document>() as IEnumerable<Document> : Documents;

            IEnumerable<Document> itemsWithChapters;
            switch (Podcast.Id)
            {
                case FraKaareTeaserViewModel.FraKårePodcastId:
                    itemsWithChapters = _weekOfTheYearChapterStrategy.AddChapterHeaders(items, existingDocs);
                    break;

                case AslaksenTeaserViewModel.AslaksenPodcastId:
                    itemsWithChapters = _aslaksenTagChapterStrategy.AddChapterHeaders(items, existingDocs);
                    break;

                // BTV asked to change the order of tracks for the Hebrew podcast
                case AslaksenTeaserViewModel.HebrewPodcastId:
                case AslaksenTeaserViewModel.FraBegynnelsenPodcastId:
                    itemsWithChapters = items.OrderBy(x => x.Order).ToList();
                    break;

                default:
                    itemsWithChapters = items;
                    break;
            }

            return itemsWithChapters;
        }

        public override CacheKeys? CacheKey => CacheKeys.PodcastGetTracks;

        protected override async Task Initialization()
        {
            Podcast = await Client.Podcast.GetById(Podcast.Id, CachePolicy.UseCacheAndRefreshOutdated);

            await Task.WhenAll(
                base.Initialization(),
                CheckIfIsFollowingThisPodcast(),
                BackgroundInitialization(UpdateOfflineTracks)
            );
        }

        private async Task CheckIfIsFollowingThisPodcast()
        {
            IsFollowing = _podcastDownloader.IsFollowing(Podcast);
            if (IsFollowing)
            {
                await Follow();
            }
        }

        private async Task StartPlayingIfRequested()
        {
            if (!StartPlayingTrackId.HasValue)
                return;

            var track = Documents.FirstOrDefault(t => t.Id == StartPlayingTrackId.Value);
            if (track != null)
                await DocumentAction(track);

            StartPlayingTrackId = null;
        }

        private async Task ToggleFollowing()
        {
            var connectionStatus = _connection.GetStatus();
            if (connectionStatus == ConnectionStatus.Offline)
            {
                // Unfortunately the ToggleButton from Android ignores OneWay mode and changes the state of the button. Therefore we have to RaisePropertyChanged to get the UI back in sync
                await RaisePropertyChanged(() => IsFollowing);
                await RaisePropertyChanged(() => FollowButtonText);

                await _userDialogs.AlertAsync(TextSource[Translations.PodcastViewModel_ToggleFollowOfflineError]);
                return;
            }

            IsFollowing = !IsFollowing;

            if (IsFollowing)
            {
                if (IsLoading)
                {
                    IsFollowing = !IsFollowing;
                    return;
                }
                await MobileNetworkUsageWarning();

                await Follow();
                await RaisePropertyChanged(() => Documents);
            }
            else
            {
                string lang = TextSource.GetText(Translations.PodcastViewModel_UnfollowConfirm, new List<string> {Podcast.Title});
                var userConfirmed = await _userDialogs.ConfirmAsync(lang);
                if (!userConfirmed)
                {
                    IsFollowing = !IsFollowing;
                    return;
                }

                await Unfollow();
                await RaisePropertyChanged(() => Documents);
            }
        }

        private async Task Follow()
        {
            await _podcastDownloader.FollowPodcast(Podcast);
        }

        private async Task Unfollow()
        {
            await _podcastDownloader.UnfollowPodcast(Podcast);
        }

        private Task UpdateOfflineTracks()
        {
            return _mediaDownloader.SynchronizeOfflineTracks();
        }

        private void File_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
            {
                RaisePropertyChanged(() => Documents);
            }
        }

        private async Task MobileNetworkUsageWarning()
        {
            var mobileNetworkDownloadAllowed = await _networkSettings.GetMobileNetworkDownloadAllowed();

            var isUsingNetworkWithoutExtraCosts = _connection.IsUsingNetworkWithoutExtraCosts();

            if (!mobileNetworkDownloadAllowed && !isUsingNetworkWithoutExtraCosts)
            {
                await _toastDisplayer.WarnAsync(TextSource[Translations.PodcastViewModel_MobileDownloadDisabled]);
            }
        }
    }
}
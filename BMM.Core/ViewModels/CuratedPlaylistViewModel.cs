using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.PlaylistPersistence;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class CuratedPlaylistViewModel : DownloadViewModel, IMvxViewModel<Playlist>
    {
        private readonly IPlaylistManager _playlistManager;
        private readonly ITrackPOFactory _trackPOFactory;
        private readonly IMvxMainThreadAsyncDispatcher _mainThreadAsyncDispatcher;
        private readonly IOfflinePlaylistStorage _offlinePlaylistStorage;
        private Playlist _curatedPlaylist;

        public Playlist CuratedPlaylist
        {
            get => _curatedPlaylist;
            set
            {
                SetProperty(ref _curatedPlaylist, value);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => Description);
                RaisePropertyChanged(() => Image);
            }
        }

        public override string Title => CuratedPlaylist.Title;

        public override string Description => CuratedPlaylist.Description;

        public override IEnumerable<string> PlaybackOrigin()
        {
            return new[] { CuratedPlaylist.Id.ToString(), CuratedPlaylist.Title };
        }

        public override bool ShowImage => true;
        public override string Image => CuratedPlaylist.Cover;

        public CuratedPlaylistViewModel(
            IStorageManager storageManager,
            IDocumentFilter documentFilter,
            IDownloadQueue downloadQueue,
            IConnection connection,
            INetworkSettings networkSettings,
            IPlaylistManager playlistManager,
            ITrackPOFactory trackPOFactory,
            IShareLink shareLink,
            IMvxMainThreadAsyncDispatcher mainThreadAsyncDispatcher,
            IOfflinePlaylistStorage offlinePlaylistStorage,
            IFirebaseRemoteConfig firebaseRemoteConfig)
            : base(storageManager, documentFilter, downloadQueue, connection, networkSettings)
        {
            _playlistManager = playlistManager;
            _trackPOFactory = trackPOFactory;
            _mainThreadAsyncDispatcher = mainThreadAsyncDispatcher;
            _offlinePlaylistStorage = offlinePlaylistStorage;
            TrackInfoProvider = new AudiobookPodcastInfoProvider(TrackInfoProvider, firebaseRemoteConfig);
            ShareCommand = new ExceptionHandlingCommand(() => shareLink.Share(CuratedPlaylist));
            AddToTrackCollectionCommand = new ExceptionHandlingCommand(() => AddToTrackCollection(CuratedPlaylist.Id, DocumentType.Playlist));
        }
        
        public IMvxAsyncCommand ShareCommand { get; }
        
        public IMvxAsyncCommand AddToTrackCollectionCommand { get; }

        public void Prepare(Playlist curatedPlaylist)
        {
            CuratedPlaylist = curatedPlaylist;
            _mainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(async () =>
            {
                IsOfflineAvailable = await _offlinePlaylistStorage.IsOfflineAvailable(curatedPlaylist.Id);
            });
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var tracks = await Client.Playlist.GetTracks(CuratedPlaylist.Id, policy);
            DurationLabel = PrepareDurationLabel(tracks.SumTrackDuration());
            return tracks.Select(t => _trackPOFactory.Create(TrackInfoProvider, OptionCommand, t));
        }
        
        protected override async Task Initialization()
        {
            CuratedPlaylist = await Client.Playlist.GetById(CuratedPlaylist.Id, CachePolicy.UseCacheAndRefreshOutdated);
            await base.Initialization();
        }

        public override CacheKeys? CacheKey => CacheKeys.PlaylistGetById;

        protected override async Task DownloadAction()
        {
            await _playlistManager.DownloadPlaylist(CuratedPlaylist);
        }

        protected override async Task DeleteAction()
        {
            await _playlistManager.RemoveDownloadedPlaylist(CuratedPlaylist);
        }

        protected override Task<long> CalculateApproximateDownloadSize()
        {
            var sum = Documents.OfType<TrackPO>().Sum(x => x.Track.Media.Sum(t => t.Files.Sum(s => s.Size)));
            return Task.FromResult(sum);
        }
    }
}
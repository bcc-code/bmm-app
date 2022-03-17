using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.PlaylistPersistence;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class CuratedPlaylistViewModel : DownloadViewModel, IMvxViewModel<Playlist>
    {
        private readonly IPlaylistManager _playlistManager;
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
            IPlaylistManager playlistManager)
            : base(storageManager, documentFilter, downloadQueue, connection, networkSettings)
        {
            _playlistManager = playlistManager;
            TrackInfoProvider = new AudiobookPodcastInfoProvider(TrackInfoProvider);
        }

        public void Prepare(Playlist curatedPlaylist)
        {
            CuratedPlaylist = curatedPlaylist;
            Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>()
                .ExecuteOnMainThreadAsync(async () => { IsOfflineAvailable = await Mvx.IoCProvider.Resolve<IOfflinePlaylistStorage>().IsOfflineAvailable(curatedPlaylist.Id); });
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            return await Client.Playlist.GetTracks(CuratedPlaylist.Id, policy);
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
            var sum = Documents.OfType<Track>().Sum(x => x.Media.Sum(t => t.Files.Sum(s => s.Size)));
            return Task.FromResult(sum);
        }
    }
}
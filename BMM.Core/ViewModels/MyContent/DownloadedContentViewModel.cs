using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.PlaylistPersistence;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.MyContent
{
    public class DownloadedContentViewModel : ContentBaseViewModel
    {
        private readonly IConnection _connection;
        private readonly IOfflinePlaylistStorage _playlistOfflineStorage;
        private readonly IPlaylistPOFactory _playlistPOFactory;

        public bool IsEmpty { get; private set; }

        public DownloadedContentViewModel(
            IStorageManager storageManager,
            IConnection connection,
            ITrackCollectionPOFactory trackCollectionPOFactory,
            IOfflinePlaylistStorage playlistOfflineStorage,
            IPlaylistPOFactory playlistPOFactory)
            : base(
                storageManager,
                trackCollectionPOFactory)
        {
            _connection = connection;
            _playlistOfflineStorage = playlistOfflineStorage;
            _playlistPOFactory = playlistPOFactory;
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            Documents.CollectionChanged += DocumentsOnCollectionChanged;
            PropertyChanged += OnPropertyChanged;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            Documents.CollectionChanged -= DocumentsOnCollectionChanged;
            PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsLoading))
                RaisePropertyChanged(() => IsEmpty);
        }

        private void DocumentsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => IsEmpty);
        }

        private async Task<IList<TrackCollectionPO>> TrackCollectionContainingOfflineFiles(IEnumerable<TrackCollectionPO> allCollections, CachePolicy cachePolicy)
        {
            var offlineCollection = allCollections?.Where(tc => tc.IsAvailableOffline).ToList();
            var isOnline = _connection.GetStatus() == ConnectionStatus.Online;

            if (isOnline || offlineCollection == null)
                return offlineCollection;

            var returnList = new List<TrackCollectionPO>();
            foreach (var trackCollection in offlineCollection)
            {
                var collectionTracks = await Client.TrackCollection.GetById(trackCollection.Id, cachePolicy);
                
                if (collectionTracks.Tracks.Any(track => _storageManager.SelectedStorage.IsDownloaded(track)))
                    returnList.Add(trackCollection);
            }

            return returnList;
        }

        public override async Task Load()
        {
            await base.Load();
            await RaisePropertyChanged(() => IsEmpty);
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var allCollectionsExceptMyTracks = (await base.LoadItems(policy))?.OfType<TrackCollectionPO>();
            var offlineTrackCollections = await TrackCollectionContainingOfflineFiles(allCollectionsExceptMyTracks, policy);
            var offlinePlaylists = await BuildDownloadedCuratedPlaylists();

            var items = new List<DocumentPO>();

            items.AddRange(await BuildPinnedItems());
            items.AddRange(offlinePlaylists);
            if (offlineTrackCollections != null)
                items.AddRange(offlineTrackCollections);

            if (items.Count == 0)
                IsEmpty = true;

            return items;
        }

        private async Task<IEnumerable<DocumentPO>> BuildDownloadedCuratedPlaylists()
        {
            var documents = new List<DocumentPO>();
            IList<Playlist> playlists = await Client.Playlist.GetAll(CachePolicy.UseCache);
            var downloadedPlaylistIds = await _playlistOfflineStorage.GetPlaylistIds();
            foreach (var playlist in playlists)
            {
                if (downloadedPlaylistIds.Contains(playlist.Id))
                {
                    documents.Add(_playlistPOFactory.Create(playlist));
                }
            }

            return documents;
        }

        private async Task<IEnumerable<PinnedItemPO>> BuildPinnedItems()
        {
            var followedPodcastPinnedItem = new PinnedItem
            {
                Title = TextSource[Translations.DownloadedContentViewModel_FollowedPodcasts],
                Action = new MvxAsyncCommand<PinnedItem>(execute => NavigationService.Navigate<DownloadedFollowedPodcastsViewModel>()),
                Icon = "icon_podcast"
            };

            var pinnedItemsList = new List<PinnedItemPO>();

            var podcasts = await Client.Podcast.GetAll(CachePolicy.UseCacheAndRefreshOutdated);
            var followedPodcasts = podcasts?.Where(Mvx.IoCProvider.Resolve<IPodcastOfflineManager>().IsFollowing);
            if (followedPodcasts != null && followedPodcasts.Any())
                pinnedItemsList.Add(new PinnedItemPO(followedPodcastPinnedItem));

            return pinnedItemsList;
        }
    }
}
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

        public bool IsEmpty { get; private set; }

        public DownloadedContentViewModel(
            IStorageManager storageManager,
            IConnection connection,
            ITrackCollectionPOFactory trackCollectionPOFactory)
            : base(
                storageManager,
                trackCollectionPOFactory)
        {
            _connection = connection;
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

        private async Task<IList<TrackCollectionPO>> TrackCollectionContainOfflineFiles(IList<TrackCollectionPO> allCollections, CachePolicy cachePolicy)
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
            var allCollectionsExceptMyTracks = await base.LoadItems(policy) as IList<TrackCollectionPO>;

            var offlineTrackCollections = await TrackCollectionContainOfflineFiles(allCollectionsExceptMyTracks, policy);

            var offlineTrackCollectionsPlusPinnedItems = new List<DocumentPO>();

            offlineTrackCollectionsPlusPinnedItems.AddRange(await BuildPinnedItems());
            if (offlineTrackCollections != null)
                offlineTrackCollectionsPlusPinnedItems.AddRange(offlineTrackCollections);

            if (offlineTrackCollectionsPlusPinnedItems.Count == 0)
                IsEmpty = true;

            return offlineTrackCollectionsPlusPinnedItems;
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
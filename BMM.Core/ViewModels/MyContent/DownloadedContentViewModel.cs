using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Localization;

namespace BMM.Core.ViewModels.MyContent
{
    public class DownloadedContentViewModel : ContentBaseViewModel
    {
        private readonly IConnection _connection;

        public bool IsEmpty { get; private set; }

        // This TextSource is needed because the TrackCollectionsAddToViewModel reuses the fragment axml. Otherwise we would need to duplicate the translations.
        // ReSharper disable once UnusedMember.Global
        public IMvxLanguageBinder MyContentDownloadedContentTextSource =>
            new MvxLanguageBinder(GlobalConstants.GeneralNamespace, typeof(DownloadedContentViewModel).Name);

        public DownloadedContentViewModel(
            IOfflineTrackCollectionStorage downloader,
            IStorageManager storageManager,
            IConnection connection
            )
            : base(
                  downloader,
                  storageManager
                  )
        {
            _connection = connection;

            Documents.CollectionChanged += (sender, e) => RaisePropertyChanged(() => IsEmpty);

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsLoading")
                    RaisePropertyChanged(() => IsEmpty);
            };
        }

        private async Task<IList<TrackCollection>> TrackCollectionContainOfflineFiles(IList<TrackCollection> allCollections, CachePolicy cachePolicy)
        {
            var offlineCollection = allCollections?.Where(IsOfflineAvailable).ToList();
            var isOnline = _connection.GetStatus() == ConnectionStatus.Online;

            if (isOnline || offlineCollection == null)
            {
                return offlineCollection;
            }
            else
            {
                List<TrackCollection> returnList = new List<TrackCollection>();
                foreach (var trackCollection in offlineCollection)
                {
                    var collectionTracks = await Client.TrackCollection.GetById(trackCollection.Id, cachePolicy);
                    if (collectionTracks.Tracks.Any(track => _storageManager.SelectedStorage.IsDownloaded(track)))
                    {
                        returnList.Add(trackCollection);
                    }
                }

                return returnList;
            }
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var allCollectionsExceptMyTracks = await base.LoadItems(policy) as IList<TrackCollection>;

            var offlineTrackCollections = await TrackCollectionContainOfflineFiles(allCollectionsExceptMyTracks, policy);

            List<Document> offlineTrackCollectionsPlusPinnedItems = new List<Document>();

            offlineTrackCollectionsPlusPinnedItems.AddRange(await BuildPinnedItems());
            if (offlineTrackCollections != null)
                offlineTrackCollectionsPlusPinnedItems.AddRange(offlineTrackCollections);

            if (offlineTrackCollectionsPlusPinnedItems.Count == 0)
                IsEmpty = true;

            return offlineTrackCollectionsPlusPinnedItems;
        }

        private async Task<IEnumerable<PinnedItem>> BuildPinnedItems()
        {
            var followedPodcastPinnedItem = new PinnedItem
            {
                Title = MyContentDownloadedContentTextSource.GetText("FollowedPodcasts"),
                Action = new MvxAsyncCommand<PinnedItem>(execute => _navigationService.Navigate<DownloadedFollowedPodcastsViewModel>()),
                Icon = "icon_podcast"
            };

            var pinnedItemsList = new List<PinnedItem>();

            var podcasts = await Client.Podcast.GetAll(CachePolicy.UseCacheAndRefreshOutdated);
            var followedPodcasts = podcasts?.Where(Mvx.IoCProvider.Resolve<IPodcastOfflineManager>().IsFollowing);
            if (followedPodcasts != null && followedPodcasts.Any())
                pinnedItemsList.Add(followedPodcastPinnedItem);

            return pinnedItemsList;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.MyTracks;
using BMM.Core.Implementations.TrackCollections;
using MvvmCross;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Base
{
    public class ContentBaseViewModel : DocumentsViewModel
    {
        public IMvxAsyncCommand CreatePlaylistCommand { get; private set; }

        protected TrackCollection MyTracksCollection = new TrackCollection();

        protected readonly IOfflineTrackCollectionStorage _downloader;
        protected readonly IStorageManager _storageManager;

        public override CacheKeys? CacheKey => CacheKeys.TrackCollectionGetAll;

        public ContentBaseViewModel(IOfflineTrackCollectionStorage downloader, IStorageManager storageManager)
        {
            _downloader = downloader;
            _storageManager = storageManager;

            CreatePlaylistCommand = new ExceptionHandlingCommand(
                async () => await CreateTrackCollection()
            );
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            RefreshInBackground();
        }

        protected override async Task<bool> DeleteTrackCollection(TrackCollection item)
        {
            var success = await base.DeleteTrackCollection(item);

            if (success)
            {
                Documents.Remove(item);
            }

            return success;
        }

        protected override async Task<bool> CreateTrackCollection()
        {
            var success = await base.CreateTrackCollection();

            if (success)
                await Refresh();

            return success;
        }

        protected IList<TrackCollection> ExcludeMyTracksCollection(IList<TrackCollection> allCollections)
        {
            foreach (TrackCollection collection in allCollections)
            {
                if (collection.Name.Equals(MyTracksManager.MyTracksPlaylistName))
                {
                    MyTracksCollection = collection;
                    allCollections.Remove(collection);
                    break;
                }
            }

            return allCollections;
        }

        public bool IsOfflineAvailable(TrackCollection trackCollection)
        {
            return _downloader.IsOfflineAvailable(trackCollection);
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var allCollections = await Client.TrackCollection.GetAll(policy);

            if (allCollections == null)
                return null;

            allCollections = allCollections.OrderByDescending(c => c.Id).ToList();
            var allCollectionsExceptMyTracks = ExcludeMyTracksCollection(allCollections);

            return allCollectionsExceptMyTracks;
        }

        protected override async Task DocumentAction(Document item, IList<Track> list)
        {
            if (item is PinnedItem pinnedItem)
            {
                var action = pinnedItem.Action as MvxAsyncCommand<PinnedItem>;
                await action.ExecuteAsync(pinnedItem);
            }
            else
                await base.DocumentAction(item, list);
        }
    }
}
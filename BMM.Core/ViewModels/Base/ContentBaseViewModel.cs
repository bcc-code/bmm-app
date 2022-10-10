using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.TrackCollections;
using MvvmCross;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Base
{
    public class ContentBaseViewModel : DocumentsViewModel
    {
        public IMvxAsyncCommand CreatePlaylistCommand { get; private set; }
        
        protected readonly IStorageManager _storageManager;
        private readonly ITrackCollectionPOFactory _trackCollectionPOFactory;

        public override CacheKeys? CacheKey => CacheKeys.TrackCollectionGetAll;

        public ContentBaseViewModel(
            IStorageManager storageManager,
            ITrackCollectionPOFactory trackCollectionPOFactory)
        {
            _storageManager = storageManager;
            _trackCollectionPOFactory = trackCollectionPOFactory;

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
                Documents.Remove(Documents.First(t=>t.Id == item.Id));

            return success;
        }

        protected override async Task<bool> CreateTrackCollection()
        {
            var success = await base.CreateTrackCollection();

            if (success)
                await Refresh();

            return success;
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var allCollections = await Client.TrackCollection.GetAll(policy);

            if (allCollections == null)
                return null;

            return allCollections
                .OrderByDescending(c => c.Id)
                .Select(tc => _trackCollectionPOFactory.Create(tc));
        }

        protected override async Task DocumentAction(IDocumentPO item, IList<Track> list)
        {
            if (item is PinnedItemPO pinnedItemPO)
            {
                var action = pinnedItemPO.PinnedItem.Action as MvxAsyncCommand<PinnedItem>;
                await action.ExecuteAsync(pinnedItemPO.PinnedItem);
            }
            else
                await base.DocumentAction(item, list);
        }
    }
}
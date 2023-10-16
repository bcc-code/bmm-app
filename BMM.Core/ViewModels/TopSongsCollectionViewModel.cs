using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Tracklist.Interfaces;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class TopSongsCollectionViewModel 
        : TrackCollectionViewModel,
          ITopSongsCollectionViewModel
    {
        private readonly IPrepareTopSongsViewModelAction _prepareTopSongsViewModelAction;
        private readonly IAddTopSongsPlaylistToFavouritesAction _addTopSongsPlaylistToFavouritesAction;
        private string _name;
        private string _pageTitle;

        public TopSongsCollectionViewModel(
            IStorageManager storageManager,
            IDownloadedTracksOnlyFilter documentFilter,
            ITrackCollectionManager trackCollectionManager,
            IConnection connection,
            IDownloadQueue downloadQueue,
            INetworkSettings networkSettings,
            ITrackPOFactory trackPOFactory,
            IPrepareTopSongsViewModelAction prepareTopSongsViewModelAction,
            IAddTopSongsPlaylistToFavouritesAction addTopSongsPlaylistToFavouritesAction)
            : base(
            storageManager,
            documentFilter,
            trackCollectionManager,
            connection,
            downloadQueue,
            networkSettings,
            trackPOFactory)
        {
            _prepareTopSongsViewModelAction = prepareTopSongsViewModelAction;
            _addTopSongsPlaylistToFavouritesAction = addTopSongsPlaylistToFavouritesAction;
            _addTopSongsPlaylistToFavouritesAction.AttachDataContext(this);
            _prepareTopSongsViewModelAction.AttachDataContext(this);
        }

        public override string PlaybackOriginString(int? index = null) => PlaybackOrigins.TopSongs;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        public IMvxAsyncCommand AddToFavouritesCommand => _addTopSongsPlaylistToFavouritesAction.Command;

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            return await _prepareTopSongsViewModelAction.ExecuteGuarded();
        }
    }
}
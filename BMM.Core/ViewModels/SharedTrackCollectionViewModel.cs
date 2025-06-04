using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Tracklist.Interfaces;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class SharedTrackCollectionViewModel : TrackCollectionViewModel, ISharedTrackCollectionViewModel
    {
        private readonly IAddToMyPlaylistAction _addToMyPlaylistAction;

        public SharedTrackCollectionViewModel(
            IStorageManager storageManager,
            IDownloadedTracksOnlyFilter documentFilter,
            ITrackCollectionManager trackCollectionManager,
            IConnection connection,
            IDownloadQueue downloadQueue,
            INetworkSettings networkSettings,
            IAddToMyPlaylistAction addToMyPlaylistAction,
            ITrackPOFactory trackPOFactory)
            : base(
                storageManager,
                documentFilter,
                trackCollectionManager,
                connection,
                downloadQueue,
                networkSettings,
                trackPOFactory)
        {
            _addToMyPlaylistAction = addToMyPlaylistAction;
            _addToMyPlaylistAction.AttachDataContext(this);
        }

        public IMvxAsyncCommand AddToMyPlaylistCommand => _addToMyPlaylistAction.Command;

        public string SharingSecret { get; private set; }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy cachePolicy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var trackCollection = await Client.SharedPlaylist.Get(SharingSecret);
            MyCollection = trackCollection;
            DurationLabel = PrepareDurationLabel(MyCollection.Tracks.SumTrackDuration());
            return MyCollection.Tracks.Select(t=> TrackPOFactory.Create(TrackInfoProvider, OptionCommand, t));
        }
        
        public void Prepare(ISharedTrackCollectionParameter parameter)
        {
            SharingSecret = parameter.SharingSecret;
        }
    }
}
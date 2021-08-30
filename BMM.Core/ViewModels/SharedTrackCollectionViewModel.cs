using System.Collections.Generic;
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
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
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
            IAddToMyPlaylistAction addToMyPlaylistAction)
            : base(
                storageManager,
                documentFilter,
                trackCollectionManager,
                connection,
                downloadQueue,
                networkSettings)
        {
            _addToMyPlaylistAction = addToMyPlaylistAction;
            _addToMyPlaylistAction.AttachDataContext(this);
        }

        public override bool ShowFollowSharedPlaylistButton => true;

        public IMvxAsyncCommand AddToMyPlaylistCommand => _addToMyPlaylistAction.Command;

        public string SharingSecret { get; private set; }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy cachePolicy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var trackCollection = await Client.SharedPlaylist.Get(SharingSecret);
            MyCollection = trackCollection;
            return MyCollection.Tracks;
        }

        public void Prepare(ISharedTrackCollectionParameter parameter)
        {
            SharingSecret = parameter.SharingSecret;
        }
    }
}
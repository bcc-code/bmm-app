using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Messages;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class SharedTrackCollectionViewModel : TrackCollectionViewModel, ISharedTrackCollectionViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private string _sharingSecret;

        public SharedTrackCollectionViewModel(
            IStorageManager storageManager,
            IDownloadedTracksOnlyFilter documentFilter,
            ITrackCollectionManager trackCollectionManager,
            IConnection connection,
            IDownloadQueue downloadQueue,
            INetworkSettings networkSettings,
            IUserDialogs userDialogs)
            : base(
                storageManager,
                documentFilter,
                trackCollectionManager,
                connection,
                downloadQueue,
                networkSettings)
        {
            _userDialogs = userDialogs;
            AddToMyPlaylistCommand = new ExceptionHandlingCommand(async () =>
            {
                try
                {
                    await Client.SharedPlaylist.Follow(_sharingSecret);
                    _messenger.Publish(new PlaylistStateChangedMessage(this, MyCollection.Id));
                    await CloseCommand.ExecuteAsync();
                }
                catch (FollowOwnTrackCollectionException)
                {
                    await userDialogs.AlertAsync(TextSource.GetText("FollowOwnTrackCollection"));
                }
            });
        }

        public override bool ShowFollowSharedPlaylistButton => true;

        public IMvxAsyncCommand AddToMyPlaylistCommand { get; }

        public override void Prepare(ITrackCollectionParameter parameter)
        {
            _sharingSecret = parameter.SharingSecret;
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy cachePolicy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var trackCollection = await Client.SharedPlaylist.Get(_sharingSecret);
            MyCollection = trackCollection;
            return MyCollection.Tracks;
        }
    }
}
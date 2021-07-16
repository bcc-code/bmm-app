using System;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Messages;
using BMM.Core.ViewModels.MyContent;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels
{
    public class TrackCollectionViewModel : MyTracksViewModel
    {
        private MvxSubscriptionToken _trackCollectionOrderChangedToken;
        private MvxSubscriptionToken _playlistStateChangedMessageSubscriptionKey;

        public IMvxAsyncCommand DeleteCommand { get; }

        public IMvxAsyncCommand EditCommand { get; }

        public IMvxAsyncCommand ShareCommand { get; }

        public IMvxAsyncCommand RemoveCommand { get; }

        public bool IsConnectionOnline => Connection.GetStatus() == ConnectionStatus.Online;

        public override bool ShowSharingInfo => true;

        public override bool ShowImage => false;

        public TrackCollectionViewModel(
            IStorageManager storageManager,
            IDownloadedTracksOnlyFilter documentFilter,
            ITrackCollectionManager trackCollectionManager,
            IConnection connection,
            IDownloadQueue downloadQueue,
            INetworkSettings networkSettings
        )
            : base(
                storageManager,
                documentFilter,
                trackCollectionManager,
                downloadQueue,
                connection,
                networkSettings
            )
        {
            DeleteCommand = new ExceptionHandlingCommand(() => DeleteTrackCollection(MyCollection));

            EditCommand = new ExceptionHandlingCommand(() =>
                 {
                     return _navigationService
                         .Navigate<EditTrackCollectionViewModel, ITrackCollectionParameter>(
                             new TrackCollectionParameter(MyCollection.Id));
                 });

            ShareCommand = new ExceptionHandlingCommand(() => ShareTrackCollection(MyCollection.Id));
            RemoveCommand = new ExceptionHandlingCommand(() => RemoveSharedPlaylist(MyCollection.Id));
        }

        protected override Task Initialization()
        {
            _trackCollectionOrderChangedToken = _messenger.Subscribe<TrackCollectionOrderChangedMessage>(HandleTrackCollectionOrderChanged);
            _playlistStateChangedMessageSubscriptionKey =
                _messenger.Subscribe<PlaylistStateChangedMessage>(m => ReloadCommand.ExecuteAsync());

            return base.Initialization();
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            _messenger.Unsubscribe<PlaylistStateChangedMessage>(_playlistStateChangedMessageSubscriptionKey);
            _messenger.Unsubscribe<TrackCollectionOrderChangedMessage>(_trackCollectionOrderChangedToken);
            base.ViewDestroy(viewFinishing);
        }

        private void HandleTrackCollectionOrderChanged(TrackCollectionOrderChangedMessage message)
        {
            if (message.TrackCollection.Id != MyCollection.Id)
                return;
            MyCollection.Name = message.TrackCollection.Name;
            MyCollection.Tracks = message.TrackCollection.Tracks;
            RaisePropertyChanged(() => MyCollection);
            Documents.SwitchTo(message.TrackCollection.Tracks);
            ExceptionHandler.FireAndForgetWithoutUserMessages(TryRefresh);
        }

        protected override async Task<bool> DeleteTrackCollection(TrackCollection item)
        {
            var success = await base.DeleteTrackCollection(item);

            if (success)
            {
                await _navigationService.Close(this);
            }

            return success;
        }

        public async Task DeleteTrackFromTrackCollection(Track item)
        {
            var collection = MyCollection;
            collection.Tracks.Remove(item);

            try
            {
                await Client.TrackCollection.Save(collection);

                Documents.Remove(item);

                await UpdateTrackCollectionFiles(collection);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }
    }
}
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Storage;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.TrackListenedObservation
{
    public class ListenedTracksStorage : IListenedTracksStorage
    {
        private readonly IMvxMessenger _messenger;
        private HashSet<int> _listenedTracksIds;

        public ListenedTracksStorage(IMvxMessenger messenger)
        {
            _messenger = messenger;
        }

        public async Task AddTrackToListenedTracks(ITrackModel trackModel)
        {
            await InitAsyncIfNeeded();

            var oldTrackIdCount = _listenedTracksIds.Count;

            _listenedTracksIds.Add(trackModel.Id);

            if (oldTrackIdCount != _listenedTracksIds.Count)
            {
                AppSettings.ListenedTracks = _listenedTracksIds;
                _messenger.Publish(new TrackMarkedAsListenedMessage(this) {TrackId = trackModel.Id});
            }
        }

        public async Task<bool> TrackIsListened(ITrackModel trackModel)
        {
            await InitAsyncIfNeeded();
            return _listenedTracksIds.Contains(trackModel.Id);
        }

        private async Task InitAsyncIfNeeded()
        {
            if (_listenedTracksIds != null)
                return;

            _listenedTracksIds = AppSettings.ListenedTracks;
        }
    }
}
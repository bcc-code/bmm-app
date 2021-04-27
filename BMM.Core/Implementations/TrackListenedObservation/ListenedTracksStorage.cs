using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Abstraction;
using BMM.Core.Helpers;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.TrackListenedObservation
{
    public class ListenedTracksStorage : IListenedTracksStorage
    {
        private readonly IBlobCache _blobCache;
        private readonly IMvxMessenger _messenger;
        private HashSet<int> _listenedTracksIds;

        public ListenedTracksStorage(IBlobCache blobCache, IMvxMessenger messenger)
        {
            _blobCache = blobCache;
            _messenger = messenger;
        }

        public async Task AddTrackToListenedTracks(ITrackModel trackModel)
        {
            await InitAsyncIfNeeded();

            var oldTrackIdCount = _listenedTracksIds.Count;

            _listenedTracksIds.Add(trackModel.Id);

            if (oldTrackIdCount != _listenedTracksIds.Count)
            {
                await _blobCache.InsertObject(StorageKeys.ListenedTracks, _listenedTracksIds);
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

            _listenedTracksIds = await _blobCache
                .GetOrCreateObject(StorageKeys.ListenedTracks, () => new HashSet<int>());
        }
    }
}
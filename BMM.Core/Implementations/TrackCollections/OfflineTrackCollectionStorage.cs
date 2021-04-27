using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;

namespace BMM.Core.Implementations.TrackCollections
{
    public class OfflineTrackCollectionStorage : IOfflineTrackCollectionStorage
    {
        private readonly IBlobCache _blobCache;

        private ICollection<int> _offlineTrackCollections;

        public OfflineTrackCollectionStorage(IBlobCache blobCache)
        {
            _blobCache = blobCache;
        }

        public async Task InitAsync()
        {
            _offlineTrackCollections = await _blobCache.GetOrCreateObject(StorageKeys.LocalTrackCollections, () => new HashSet<int>(), null);
        }

        public bool IsOfflineAvailable(TrackCollection trackCollection)
        {
            ThrowIfNotInitialized();
            return _offlineTrackCollections.Contains(trackCollection.Id);
        }

        private void ThrowIfNotInitialized()
        {
            if (_offlineTrackCollections == null)
            {
                throw new NullReferenceException("You need to call the InitAsync method first");
            }
        }

        public ICollection<int> GetOfflineTrackCollectionIds()
        {
            return _offlineTrackCollections;
        }

        private async Task Save()
        {
            await _blobCache.InsertObject(StorageKeys.LocalTrackCollections, _offlineTrackCollections, null);
        }

        public async Task Add(int id)
        {
            ThrowIfNotInitialized();
            _offlineTrackCollections.Add(id);
            await Save();
        }

        /// <summary>
        /// In this method we want to update the LocalStorage but skip SynchronizeOfflineTracks() because GetCollectionTracksSupposedToBeDownloaded() was actually called from it.
        /// </summary>
        public async Task Remove(int id)
        {
            ThrowIfNotInitialized();
            _offlineTrackCollections.Remove(id);
            await Save();
        }

        public async Task Clear()
        {
            ThrowIfNotInitialized();
            _offlineTrackCollections.Clear();
            await Save();
        }
    }
}

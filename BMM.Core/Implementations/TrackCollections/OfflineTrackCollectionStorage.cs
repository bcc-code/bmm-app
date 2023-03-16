using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.TrackCollections
{
    public class OfflineTrackCollectionStorage : IOfflineTrackCollectionStorage
    {
        private ICollection<int> _offlineTrackCollections;

        public async Task InitAsync()
        {
            _offlineTrackCollections = AppSettings.LocalTrackCollections;
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

        private async Task Save() => AppSettings.LocalTrackCollections = _offlineTrackCollections.ToHashSet();

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

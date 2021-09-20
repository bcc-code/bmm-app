using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;

namespace BMM.Core.Implementations.PlayObserver.Storage
{
    public class TrackPlayedStorage : ITrackPlayedStorage
    {
        private readonly string _storageKey = StorageKeys.FinishedTrackPlayedEvents;
        private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);
        private readonly IBlobCache _blobCache;

        public TrackPlayedStorage(IBlobCache blobCache)
        {
            _blobCache = blobCache;
        }

        public async Task Add(IEnumerable<TrackPlayedEvent> trackPlayedEvents)
        {
            try
            {
                await _writeSemaphore.WaitAsync();
                var existingEvents = await GetExistingEvents();
                foreach (var playedEvent in trackPlayedEvents)
                {
                    existingEvents.Add(playedEvent);
                }

                await SaveEvents(existingEvents);
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        public async Task<IList<TrackPlayedEvent>> GetExistingEvents()
        {
            var result = await _blobCache.GetOrCreateObject<IList<TrackPlayedEvent>>(_storageKey, () => new List<TrackPlayedEvent>());
            return result;
        }

        public async Task DeleteEvents(IList<TrackPlayedEvent> trackPlayedEvents)
        {
            try
            {
                await _writeSemaphore.WaitAsync();
                var existingEvents = await GetExistingEvents();
                var remainingEvents = existingEvents.Except(trackPlayedEvents, TrackPlayedEvent.IdComparer).ToList();
                await SaveEvents(remainingEvents);
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        // this method should be called inside of a lock / semaphore
        private async Task SaveEvents(IList<TrackPlayedEvent> playedEvents)
        {
            await _blobCache.InsertObject(_storageKey, playedEvents);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.PlayObserver.Storage
{
    public class TrackPlayedStorage : ITrackPlayedStorage
    {
        private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);

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
            var result = AppSettings.FinishedTrackPlayedEvents;
            return result ?? new List<TrackPlayedEvent>();
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
            AppSettings.FinishedTrackPlayedEvents = playedEvents;
        }
    }
}
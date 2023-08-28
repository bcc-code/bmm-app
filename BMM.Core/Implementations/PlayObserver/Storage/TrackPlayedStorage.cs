using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.PlayObserver.Storage
{
    public class TrackPlayedStorage : ITrackPlayedStorage
    {
        private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

        public async Task Add(IEnumerable<TrackPlayedEvent> trackPlayedEvents)
        {
            await ThreadSafeCall(() =>
            {
                var existingEvents = GetUnsentTrackPlayedEvents();
                foreach (var playedEvent in trackPlayedEvents)
                    existingEvents.Add(playedEvent);

                SaveUnsentTrackPlayedEventsEvents(existingEvents);
            });
        }

        public async Task Add(IEnumerable<StreakPointEvent> streakPointEvents)
        {
            await ThreadSafeCall(() =>
            {
                var unsentStreakPointsEvents = GetUnsentStreakPointEvents();
                foreach (var streakPointEvent in streakPointEvents)
                    unsentStreakPointsEvents.Add(streakPointEvent);

                SaveUnsentStreakPointsEvents(unsentStreakPointsEvents);
            });
        }
        
        public async Task AddListeningEvents(IEnumerable<ListeningEvent> listeningEvents)
        {
            await ThreadSafeCall(() =>
            {
                var unsentEvents = GetUnsentListeningEvents();
                foreach (var listeningEvent in listeningEvents)
                    unsentEvents.Add(listeningEvent);

                AppSettings.UnsentListeningEvent = unsentEvents;
            });
        }

        public IList<TrackPlayedEvent> GetUnsentTrackPlayedEvents()
        {
            var result = AppSettings.FinishedTrackPlayedEvents;
            return result ?? new List<TrackPlayedEvent>();
        }

        public IList<StreakPointEvent> GetUnsentStreakPointEvents()
        {
            var result = AppSettings.UnsentStreakPointEvent;
            return result ?? new List<StreakPointEvent>();
        }

        public IList<ListeningEvent> GetUnsentListeningEvents()
        {
            return AppSettings.UnsentListeningEvent ?? new List<ListeningEvent>();
        }

        public async Task DeleteEvents(IList<TrackPlayedEvent> trackPlayedEvents)
        {
            try
            {
                await _writeSemaphore.WaitAsync();
                var existingEvents = GetUnsentTrackPlayedEvents();
                var remainingEvents = existingEvents.Except(trackPlayedEvents, TrackPlayedEvent.IdComparer).ToList(); 
                SaveUnsentTrackPlayedEventsEvents(remainingEvents);
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        public void ClearUnsentStreakPointsEvents()
        {
            AppSettings.UnsentStreakPointEvent = null;
        }

        public void ClearUnsentListeningEvents()
        {
            AppSettings.UnsentListeningEvent = null;
        }

        private async Task ThreadSafeCall(Action action)
        {
            try
            {
                await _writeSemaphore.WaitAsync();
                action();
            }
            finally
            {
                _writeSemaphore.TryRelease();
            }
        }
        
        private void SaveUnsentTrackPlayedEventsEvents(IList<TrackPlayedEvent> playedEvents)
        {
            AppSettings.FinishedTrackPlayedEvents = playedEvents;
        }
        
        private void SaveUnsentStreakPointsEvents(IList<StreakPointEvent> streakPointEvents)
        {
            AppSettings.UnsentStreakPointEvent = streakPointEvents;
        }
    }
}
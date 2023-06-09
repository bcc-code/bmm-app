using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.PlayObserver.Storage
{
    public interface ITrackPlayedStorage
    {
        Task Add(IEnumerable<TrackPlayedEvent> trackPlayedEvents);
        Task Add(IEnumerable<StreakPointEvent> streakPointEvents);
        IList<TrackPlayedEvent> GetUnsentTrackPlayedEvents();
        IList<StreakPointEvent> GetUnsentStreakPointEvents();
        Task DeleteEvents(IList<TrackPlayedEvent> trackPlayedEvents);
        void ClearUnsentStreakPointsEvents();
    }
}
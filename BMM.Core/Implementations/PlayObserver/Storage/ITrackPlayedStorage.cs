using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.PlayObserver.Storage
{
    public interface ITrackPlayedStorage
    {
        Task Add(IEnumerable<TrackPlayedEvent> trackPlayedEvents);

        Task<IList<TrackPlayedEvent>> GetExistingEvents();

        Task DeleteEvents(IList<TrackPlayedEvent> trackPlayedEvents);
    }
}
using System.Threading.Tasks;
using BMM.Api.Abstraction;

namespace BMM.Core.Implementations.TrackListenedObservation
{
    public interface IListenedTracksStorage
    {
        Task AddTrackToListenedTracks(ITrackModel trackModel);

        Task<bool> TrackIsListened(ITrackModel trackModel);
    }
}
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.MyTracks
{
    public interface IMyTracksManager
    {
        Task<int> GetCachedMyTracksIdOrLoadIt();
        Task<bool> CreateMyTracksCollection();
        Task<bool> AddTrackToMyTracks(int trackId, string language);
        Task AddAlbumToMyTracks(int albumId);
        Task<TrackCollection> LoadMyTracks();
        bool MyTracksContainsTrack(int trackId, string language);
        Task InvalidateMyTracksCollection();
    }
}

using BMM.Api.Abstraction;
using BMM.Core.Models.PlaybackHistory;

namespace BMM.Core.Implementations.Player.Interfaces
{
    public interface IPlaybackHistoryService
    {
        Task AddPlayedTrack(IMediaTrack mediaTrack, long lastPosition, DateTime playedAt);
        Task<IReadOnlyList<PlaybackHistoryEntry>> GetAll();
        Task SetTrackLikedOrUnliked(int trackId, bool isLiked);
    }
}
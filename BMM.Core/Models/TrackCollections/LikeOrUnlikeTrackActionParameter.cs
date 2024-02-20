
using BMM.Core.Models.TrackCollections.Interfaces;

namespace BMM.Core.Models.TrackCollections;

public class LikeOrUnlikeTrackActionParameter : ILikeOrUnlikeTrackActionParameter
{
    public LikeOrUnlikeTrackActionParameter(bool isLiked, int trackId)
    {
        IsLiked = isLiked;
        TrackId = trackId;
    }
    
    public bool IsLiked { get; }
    public int TrackId { get; }
}
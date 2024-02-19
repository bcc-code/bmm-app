namespace BMM.Core.Models.TrackCollections.Interfaces;

public interface ILikeOrUnlikeTrackActionParameter
{
    bool IsLiked { get; }
    int TrackId { get; }
}
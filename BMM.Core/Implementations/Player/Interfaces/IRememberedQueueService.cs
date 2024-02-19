using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Player.Interfaces
{
    public interface IRememberedQueueService
    {
        bool PreventRecoveringQueue { get; }
        void SetPlayerHasPendingOperation();
        void NotifyAfterRecoveringQueue();
        Task SaveQueue(IList<Track> queueTracks);
        Task SetTrackLikedOrUnliked(int trackId, bool isLiked);
    }
}
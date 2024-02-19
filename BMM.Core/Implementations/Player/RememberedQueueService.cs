using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.Player
{
    public class RememberedQueueService : IRememberedQueueService
    {
        private readonly SemaphoreSlim _writeSemaphore = new(1, 1);
        public bool PreventRecoveringQueue { get; private set; }

        public void SetPlayerHasPendingOperation()
        {
            PreventRecoveringQueue = true;
        }

        public void NotifyAfterRecoveringQueue()
        {
            PreventRecoveringQueue = false;
        }

        public async Task SaveQueue(IList<Track> queueTracks)
        {
            try
            {
                await _writeSemaphore.WaitAsync();
                AppSettings.RememberedQueue = queueTracks;
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        public async Task SetTrackLikedOrUnliked(int trackId, bool isLiked)
        {
            try
            {
                await _writeSemaphore.WaitAsync();
                var queue = AppSettings.RememberedQueue;
                
                var trackToUpdate = queue
                    .FirstOrDefault(t => t.Id == trackId);

                trackToUpdate.IfNotNull(t => t.IsLiked = isLiked);
                
                AppSettings.RememberedQueue = queue;
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }
    }
}
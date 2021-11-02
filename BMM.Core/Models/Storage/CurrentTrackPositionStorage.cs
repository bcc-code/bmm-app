using BMM.Api.Abstraction;

namespace BMM.Core.Models.Storage
{
    public class CurrentTrackPositionStorage
    {
        public CurrentTrackPositionStorage(int currentTrackId, long lastPosition)
        {
            CurrentTrackId = currentTrackId;
            LastPosition = lastPosition;
        }

        public int CurrentTrackId { get; }
        public long LastPosition { get; }
    }
}
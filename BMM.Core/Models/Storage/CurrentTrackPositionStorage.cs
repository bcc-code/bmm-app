namespace BMM.Core.Models.Storage
{
    public class CurrentTrackPositionStorage
    {
        public CurrentTrackPositionStorage(int currentTrackId, long lastPosition, string playbackOrigin)
        {
            CurrentTrackId = currentTrackId;
            LastPosition = lastPosition;
            PlaybackOrigin = playbackOrigin;
        }

        public int CurrentTrackId { get; }
        public long LastPosition { get; }
        public string PlaybackOrigin { get; }
    }
}
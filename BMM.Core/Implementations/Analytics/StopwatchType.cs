namespace BMM.Core.Implementations.Analytics
{
    public enum StopwatchType
    {
        PlaybackDelay,
        NavigateAtAppStart,
        /// <summary>
        /// Technically it's not the AppStart but the time when dependencies are registered and we have control over what the app does.
        /// </summary>
        AppStart,
        TrackCollectionAll,
        TrackCollectionSingle,
        LiveTrack
    }
}
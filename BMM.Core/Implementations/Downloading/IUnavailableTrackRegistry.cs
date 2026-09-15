namespace BMM.Core.Implementations.Downloading
{
    /// <summary>
    /// Remembers tracks the server refuses to hand over, so that a single unobtainable track cannot keep a
    /// whole collection reporting itself as not downloaded.
    /// </summary>
    /// <remarks>
    /// This only affects how "is this fully downloaded" is judged. The synchronization keeps offering these
    /// tracks to the queue on every run, so a track that becomes available again is picked up by itself and
    /// forgotten here the moment it succeeds.
    /// </remarks>
    public interface IUnavailableTrackRegistry
    {
        bool IsUnavailable(int trackId);

        void MarkUnavailable(int trackId);

        /// <summary>
        /// Called after a successful download. Cheap no-op when the track was never marked.
        /// </summary>
        void MarkAvailable(int trackId);
    }
}

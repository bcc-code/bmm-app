namespace BMM.Core.Implementations.Downloading.DownloadQueue
{
    /// <summary>
    /// The result of a single download attempt. The distinction that matters is whether the failure
    /// says something about <em>this file</em> or about <em>the network</em>: a file that is gone should be
    /// skipped so the queue can continue, while a dead connection means every remaining item would
    /// fail just as fast, so the queue has to stop instead of burning through it.
    /// </summary>
    public enum DownloadOutcome
    {
        Success,

        /// <summary>
        /// This particular file could not be downloaded and retrying now would not help.
        /// </summary>
        PermanentFailure,

        /// <summary>
        /// The network is unusable. Nothing is wrong with the file itself.
        /// </summary>
        ConnectionFailure,

        /// <summary>
        /// The download was cancelled deliberately (user interaction, app shutdown).
        /// </summary>
        Cancelled
    }
}

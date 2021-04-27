namespace BMM.Core.Implementations.DownloadManager
{
    public enum DownloadFileStatus
    {
        /// <summary>
        /// The download is intitalized. It may is already registered in the native download manager.
        /// </summary>
        INITIALIZED,

        /// <summary>
        /// The download is pending (Android only)
        /// </summary>
        PENDING,

        /// <summary>
        /// The download is still running.
        /// </summary>
        RUNNING,

        /// <summary>
        /// The download was paused.
        /// </summary>
        PAUSED,

        /// <summary>
        /// The download has completed.
        /// </summary>
        COMPLETED,

        /// <summary>
        /// The download was canceled.
        /// </summary>
        CANCELED,

        /// <summary>
        /// The download has failed. You'll find detailed information in the property StatusDetails.
        /// </summary>
        FAILED
    }
}

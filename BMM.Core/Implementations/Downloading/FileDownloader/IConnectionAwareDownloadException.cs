namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    /// <summary>
    /// Implemented by platform specific download exceptions that can say whether the failure was the
    /// network rather than the file itself. It lets the download queue stop and keep its items instead of
    /// skipping them, without BMM.Core having to know about <c>NSError</c> or its Android equivalents.
    /// </summary>
    public interface IConnectionAwareDownloadException
    {
        bool WasConnectionFailure { get; }
    }
}

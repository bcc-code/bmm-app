using System.Collections.Generic;
using System.Threading;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using Foundation;

namespace BMM.UI.iOS.Implementations.Download
{
    /// <summary>
    /// This is a wrapper around an ongoing downloads dictionary.
    /// We use locks here since both <see cref="IosFileDownloader"/> and <see cref="IosDownloadDelegate"/> access the collection in an async way.
    /// At the moment simultaneous access doesn't happen much but this would change if <see cref="DownloadQueue"/> would allow more that one download at a time
    /// </summary>
    public class OngoingDownloadsLockDictionary
    {
        private readonly Dictionary<System.nuint, OngoingDownload> _downloads = new Dictionary<System.nuint, OngoingDownload>();
        private readonly ReaderWriterLockSlim _downloadsLock = new ReaderWriterLockSlim();

        public void Add(OngoingDownload download)
        {
            _downloadsLock.EnterWriteLock();
            try
            {
                _downloads.Add(download.DownloadTask.TaskIdentifier, download);
            }
            finally
            {
                _downloadsLock.ExitWriteLock();
            }
        }

        public OngoingDownload? GetDownloadByTask(NSUrlSessionTask task)
        {
            _downloadsLock.EnterReadLock();
            try
            {
                if (_downloads.ContainsKey(task.TaskIdentifier))
                    return _downloads[task.TaskIdentifier];

                return null;
            }
            finally
            {
                _downloadsLock.ExitReadLock();
            }
        }

        public void RemoveDownloadForTask(NSUrlSessionTask task)
        {
            _downloadsLock.EnterWriteLock();
            try
            {
                if (_downloads.ContainsKey(task.TaskIdentifier))
                    _downloads.Remove(task.TaskIdentifier);
            }
            finally
            {
                _downloadsLock.ExitWriteLock();
            }
        }

        public void CancelAll()
        {
            _downloadsLock.EnterWriteLock();
            try
            {
                foreach (var (_, value) in _downloads)
                {
                    value.DownloadTask.Cancel();
                    value.TaskCompletionSource.TrySetCanceled();
                }

                _downloads.Clear();
            }
            finally
            {
                _downloadsLock.ExitWriteLock();
            }
        }
    }
}
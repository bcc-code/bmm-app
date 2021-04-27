using System.Collections.Generic;
using System.Threading;
using BMM.Core.Implementations.Downloading.DownloadQueue;

namespace BMM.UI.iOS.Implementations.Download
{
    /// <summary>
    /// This is a wrapper around an ongoing downloads dictionary.
    /// We use locks here since both <see cref="IosFileDownloader"/> and <see cref="IosDownloadDelegate"/> access the collection in an async way.
    /// At the moment simultaneous access doesn't happen much but this would change if <see cref="DownloadQueue"/> would allow more that one download at a time
    /// </summary>
    public class OngoingDownloadsLockDictionary
    {
        private readonly Dictionary<string, OngoingDownload> _downloads = new Dictionary<string, OngoingDownload>();
        private readonly ReaderWriterLockSlim _downloadsLock = new ReaderWriterLockSlim();

        public void Add(OngoingDownload download)
        {
            _downloadsLock.EnterWriteLock();
            try
            {
                _downloads.Add(download.Downloadable.Url, download);
            }
            finally
            {
                _downloadsLock.ExitWriteLock();
            }
        }

        public OngoingDownload? GetByUrl(string url)
        {
            _downloadsLock.EnterReadLock();
            try
            {
                if (_downloads.ContainsKey(url))
                    return _downloads[url];

                return null;
            }
            finally
            {
                _downloadsLock.ExitReadLock();
            }
        }

        public void RemoveByUrl(string url)
        {
            _downloadsLock.EnterWriteLock();
            try
            {
                if (_downloads.ContainsKey(url))
                    _downloads.Remove(url);
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
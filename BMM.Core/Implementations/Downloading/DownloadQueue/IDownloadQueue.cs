using System.Collections.Generic;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Downloading.DownloadQueue
{
    public interface IDownloadQueue
    {
        int InitialDownloadCount { get; }

        int RemainingDownloadsCount { get; }

        /// <summary>
        /// True while the queue is actually working through its items. Items can be queued without the
        /// queue running, e.g. after it stopped because the connection died.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// True when the last run ended with items still queued, e.g. because the connection died.
        /// An empty queue on its own does not mean the downloads succeeded.
        /// </summary>
        bool StoppedWithPendingDownloads { get; }

        void DequeueAllExcept(IEnumerable<IDownloadable> downloadables);

        void Enqueue(IDownloadable downloadable);

        void Enqueue(IEnumerable<IDownloadable> downloadables);

        bool IsDownloading(IDownloadable downloadable);

        bool IsQueued(IDownloadable downloadable);

        void StartDownloading();

        void AppWasKilled();
    }
}
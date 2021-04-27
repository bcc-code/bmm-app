using System.Collections.Generic;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Downloading.DownloadQueue
{
    public interface IDownloadQueue
    {
        int InitialDownloadCount { get; }

        int RemainingDownloadsCount { get; }

        void DequeueAllExcept(IEnumerable<IDownloadable> downloadables);

        void Enqueue(IDownloadable downloadable);

        void Enqueue(IEnumerable<IDownloadable> downloadables);

        bool IsDownloading(IDownloadable downloadable);

        bool IsQueued(IDownloadable downloadable);

        void StartDownloading();

        void AppWasKilled();
    }
}
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using Foundation;

namespace BMM.UI.iOS.Implementations.Download
{
    public readonly struct OngoingDownload
    {
        public OngoingDownload(TaskCompletionSource<bool> taskCompletionSource, NSUrlSessionDownloadTask downloadTask, IDownloadable downloadable)
        {
            TaskCompletionSource = taskCompletionSource;
            DownloadTask = downloadTask;
            Downloadable = downloadable;
        }

        public TaskCompletionSource<bool> TaskCompletionSource { get; }

        public NSUrlSessionDownloadTask DownloadTask { get; }

        public IDownloadable Downloadable { get; }
    }
}
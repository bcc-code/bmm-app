using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.Exceptions;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading.DownloadQueue
{
    public class DownloadQueue : IDownloadQueue
    {
        private readonly DownloadableEqualityComparer _downloadableEqualityComparer = new DownloadableEqualityComparer();
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IFileDownloader _fileDownloader;
        private readonly IMvxMessenger _messenger;
        private readonly IAnalytics _analytics;
        private readonly ConcurrentBag<IDownloadable> _queuedDownloads = new ConcurrentBag<IDownloadable>();
        private IDownloadable _currentDownloadingDownloadable;
        private int _finishedDownloadCount;
        private bool _isDownloading;

        public DownloadQueue(IFileDownloader fileDownloader, IMvxMessenger messenger, IExceptionHandler exceptionHandler, IAnalytics analytics)
        {
            _fileDownloader = fileDownloader;
            _messenger = messenger;
            _exceptionHandler = exceptionHandler;
            _analytics = analytics;
        }

        private int CurrentlyDownloadingCount => _currentDownloadingDownloadable == null ? 0 : 1;

        public int InitialDownloadCount => _queuedDownloads.Count + _finishedDownloadCount + CurrentlyDownloadingCount;

        public int RemainingDownloadsCount => _queuedDownloads.Count + CurrentlyDownloadingCount;

        private void CancelQueue()
        {
            DequeueAllExcept(new List<IDownloadable>());
        }

        public void DequeueAllExcept(IEnumerable<IDownloadable> downloadables)
        {
            while (!_queuedDownloads.IsEmpty)
            {
                _queuedDownloads.TryTake(out _);
            }

            foreach (var downloadable in downloadables)
            {
                Enqueue(downloadable);
            }

            _messenger.Publish(new DownloadQueueChangedMessage(this));
        }

        public void Enqueue(IDownloadable downloadable)
        {
            if (IsQueued(downloadable) || IsDownloading(downloadable))
                return;

            _queuedDownloads.Add(downloadable);
            _messenger.Publish(new DownloadQueueChangedMessage(this));
        }

        public void Enqueue(IEnumerable<IDownloadable> downloadables)
        {
            foreach (var downloadable in downloadables)
            {
                Enqueue(downloadable);
            }
        }

        public bool IsDownloading(IDownloadable downloadable)
        {
            if (_currentDownloadingDownloadable == null) return false;
            return _downloadableEqualityComparer.Equals(downloadable, _currentDownloadingDownloadable);
        }

        public bool IsQueued(IDownloadable downloadable)
        {
            return _queuedDownloads.Contains(downloadable, _downloadableEqualityComparer);
        }

        public void AppWasKilled()
        {
            if (_isDownloading)
            {
                _fileDownloader.CancelDownload();
            }
        }

        public void StartDownloading()
        {
            if (_isDownloading)
                return;

            _isDownloading = true;

            _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
            {
                var downloadSucceeded = true;
                try
                {
                    while (_queuedDownloads.Count > 0)
                    {
                        _queuedDownloads.TryTake(out var item);
                        _currentDownloadingDownloadable = item;
                        _messenger.Publish(new FileDownloadStartedMessage(this));
                        await SafeDownload(item);
                        _messenger.Publish(new FileDownloadCompletedMessage(this));
                        _analytics.LogEvent(Event.TrackHasBeenDownloaded, PrepareAdditionalEventArguments(item));
                    }
                }
                catch (TaskCanceledException)
                {
                    CancelQueue();
                    downloadSucceeded = false;

                    // We don't need to show an error message if the task was canceled due to a user interaction
                }
                finally
                {
                    _isDownloading = false;
                    _finishedDownloadCount = 0;
                    _currentDownloadingDownloadable = null;
                    _messenger.Publish(new QueueFinishedMessage(this, downloadSucceeded));
                }
            });
        }

        private static Dictionary<string, object> PrepareAdditionalEventArguments(IDownloadable item)
        {
            return new Dictionary<string, object>
            {
                { "trackId", item.Id },
                { "tags", string.Join(",", item.Tags.ToArray()) },
                { "url", item.Url }
            };
        }

        private async Task SafeDownload(IDownloadable item)
        {
            try
            {
                await _fileDownloader.DownloadFile(item);
            }
            catch (Exception e)
            {
                var arguments = PrepareAdditionalEventArguments(item);
                arguments.Add("exception", e.Message);
                _analytics.LogEvent(Event.TrackDownloadingException, arguments);
            }
            finally
            {
                _finishedDownloadCount++;
            }
        }
    }
}
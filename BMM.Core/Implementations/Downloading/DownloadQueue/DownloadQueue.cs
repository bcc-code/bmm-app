using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.Exceptions;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading.DownloadQueue
{
    public class DownloadQueue : IDownloadQueue
    {
        /// <summary>
        /// A connection can be down for a moment without being down. Retrying a couple of times with a
        /// growing pause absorbs that without hammering a network that is genuinely gone.
        /// </summary>
        private static readonly TimeSpan[] RetryDelays =
        {
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(5)
        };

        private readonly DownloadableEqualityComparer _downloadableEqualityComparer = new DownloadableEqualityComparer();
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IFileDownloader _fileDownloader;
        private readonly IMvxMessenger _messenger;
        private readonly IAnalytics _analytics;
        private readonly IConnection _connection;
        private readonly INetworkSettings _networkSettings;
        private readonly ILogger _logger;
        private readonly ConcurrentBag<IDownloadable> _queuedDownloads = new ConcurrentBag<IDownloadable>();
        private IDownloadable _currentDownloadingDownloadable;
        private int _finishedDownloadCount;
        private bool _isDownloading;
        private bool _cancellationWasRequested;

        public DownloadQueue(
            IFileDownloader fileDownloader,
            IMvxMessenger messenger,
            IExceptionHandler exceptionHandler,
            IAnalytics analytics,
            IConnection connection,
            INetworkSettings networkSettings,
            ILogger logger)
        {
            _fileDownloader = fileDownloader;
            _messenger = messenger;
            _exceptionHandler = exceptionHandler;
            _analytics = analytics;
            _connection = connection;
            _networkSettings = networkSettings;
            _logger = logger;
        }

        private int CurrentlyDownloadingCount => _currentDownloadingDownloadable == null ? 0 : 1;

        public int InitialDownloadCount => _queuedDownloads.Count + _finishedDownloadCount + CurrentlyDownloadingCount;

        public int RemainingDownloadsCount => _queuedDownloads.Count + CurrentlyDownloadingCount;

        public bool IsRunning => _isDownloading;

        /// <summary>
        /// True when the queue gave up with items still in it. Callers must not read an empty queue as
        /// "everything was downloaded" without checking this.
        /// </summary>
        public bool StoppedWithPendingDownloads { get; private set; }

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
                _cancellationWasRequested = true;
                _fileDownloader.CancelDownload();
            }
        }

        public void StartDownloading()
        {
            if (_isDownloading)
                return;

            _isDownloading = true;
            _cancellationWasRequested = false;
            StoppedWithPendingDownloads = false;

            _exceptionHandler.FireAndForgetWithoutUserMessages(RunQueue);
        }

        private async Task RunQueue()
        {
            int succeeded = 0;
            int skipped = 0;
            var stopReason = QueueStopReason.Completed;

            try
            {
                while (!_queuedDownloads.IsEmpty)
                {
                    // Checked per item, not once up front. The connection the sync validated before
                    // enqueueing may be long gone by the time we get to item 40 of 91.
                    if (!await IsDownloadingAllowed())
                    {
                        stopReason = QueueStopReason.NoConnection;
                        break;
                    }

                    if (!_queuedDownloads.TryTake(out var item))
                        break;

                    var (outcome, exception) = await DownloadWithRetries(item);

                    switch (outcome)
                    {
                        case DownloadOutcome.Success:
                            succeeded++;
                            _finishedDownloadCount++;
                            _messenger.Publish(new FileDownloadCompletedMessage(this, item.Id));
                            _analytics.LogEvent(Event.TrackHasBeenDownloaded, PrepareAdditionalEventArguments(item));
                            break;

                        case DownloadOutcome.PermanentFailure:
                            // Nothing about this file is going to get better by trying again, so let the
                            // rest of the queue through instead of stalling on it. The message resets the
                            // row, which would otherwise keep showing a download in progress.
                            skipped++;
                            _finishedDownloadCount++;
                            _messenger.Publish(new FileDownloadCanceledMessage(this, exception));
                            break;

                        case DownloadOutcome.ConnectionFailure:
                            // Every remaining item would fail the same way within milliseconds. Put this
                            // one back and stop, so the queue survives until the connection returns.
                            _queuedDownloads.Add(item);
                            _messenger.Publish(new FileDownloadCanceledMessage(this, exception));
                            stopReason = QueueStopReason.NoConnection;
                            break;

                        case DownloadOutcome.Cancelled:
                            _queuedDownloads.Add(item);
                            stopReason = QueueStopReason.Cancelled;
                            break;
                    }

                    if (stopReason != QueueStopReason.Completed)
                        break;
                }
            }
            finally
            {
                _currentDownloadingDownloadable = null;
                _isDownloading = false;
                StoppedWithPendingDownloads = !_queuedDownloads.IsEmpty;

                LogQueueOutcome(stopReason, succeeded, skipped);

                // Only a queue that actually emptied itself counts as a success. Reporting a queue we
                // abandoned as succeeded is what made a playlist with no files on disk render as
                // downloaded.
                bool succeededOverall = stopReason == QueueStopReason.Completed && !StoppedWithPendingDownloads;

                if (!succeededOverall)
                    _messenger.Publish(new DownloadQueueChangedMessage(this));

                _finishedDownloadCount = 0;
                _messenger.Publish(new QueueFinishedMessage(this, succeededOverall));
            }
        }

        /// <summary>
        /// Retries a download while the failures look like the network rather than the file.
        /// </summary>
        private async Task<(DownloadOutcome Outcome, Exception Exception)> DownloadWithRetries(IDownloadable item)
        {
            _currentDownloadingDownloadable = item;
            _messenger.Publish(new FileDownloadStartedMessage(this, item.Id));

            try
            {
                for (int attempt = 0; ; attempt++)
                {
                    var result = await SafeDownload(item, attempt);

                    if (result.Outcome != DownloadOutcome.ConnectionFailure || attempt >= RetryDelays.Length)
                        return result;

                    await WaitBeforeRetry(RetryDelays[attempt]);

                    if (_cancellationWasRequested)
                        return (DownloadOutcome.Cancelled, result.Exception);

                    if (!await IsDownloadingAllowed())
                        return result;
                }
            }
            finally
            {
                _currentDownloadingDownloadable = null;
            }
        }

        /// <summary>
        /// Overridable so tests do not have to wait out the real backoff.
        /// </summary>
        protected virtual Task WaitBeforeRetry(TimeSpan delay) => Task.Delay(delay);

        /// <summary>
        /// Mirrors the check the synchronization does before enqueueing, so that leaving Wi-Fi mid-queue
        /// stops the run instead of quietly continuing over a metered connection.
        /// </summary>
        private async Task<bool> IsDownloadingAllowed()
        {
            if (_cancellationWasRequested)
                return false;

            if (_connection.GetStatus() != ConnectionStatus.Online)
                return false;

            return _connection.IsUsingNetworkWithoutExtraCosts()
                   || await _networkSettings.GetMobileNetworkDownloadAllowed();
        }

        private static string DescribeException(Exception exception)
        {
            try
            {
                return exception.Message;
            }
            catch (Exception)
            {
                return exception.GetType().Name;
            }
        }

        private static Dictionary<string, object> PrepareAdditionalEventArguments(IDownloadable item)
        {
            return new Dictionary<string, object>
            {
                { "trackId", item.Id },
                { "tags", item.Tags == null ? string.Empty : string.Join(",", item.Tags) },
                { "url", item.Url }
            };
        }

        private async Task<(DownloadOutcome Outcome, Exception Exception)> SafeDownload(IDownloadable item, int attempt)
        {
            try
            {
                await _fileDownloader.DownloadFile(item);
                return (DownloadOutcome.Success, null);
            }
            catch (Exception e)
            {
                var outcome = DownloadFailureClassifier.Classify(e, _cancellationWasRequested);

                if (outcome == DownloadOutcome.Cancelled)
                    return (outcome, e);

                // Reporting must never be able to fail the queue. A platform exception whose own Message
                // getter threw used to escape this catch block and abandon every remaining download.
                try
                {
                    var arguments = PrepareAdditionalEventArguments(item);
                    arguments.Add("exception", DescribeException(e));
                    arguments.Add("outcome", outcome.ToString());
                    arguments.Add("attempt", attempt);
                    _analytics.LogEvent(Event.TrackDownloadingException, arguments);
                }
                catch (Exception reportingException)
                {
                    _logger.Error(nameof(DownloadQueue),
                        $"Could not report a failed download of track {item.Id}",
                        reportingException);
                }

                return (outcome, e);
            }
        }

        /// <summary>
        /// One event per queue run rather than one per item: the per-item detail is already in analytics,
        /// and a dead connection produces hundreds of identical item failures that would drown out
        /// everything else in the error tracker.
        /// </summary>
        private void LogQueueOutcome(QueueStopReason stopReason, int succeeded, int skipped)
        {
            if (stopReason == QueueStopReason.Completed && !StoppedWithPendingDownloads)
                return;

            if (stopReason == QueueStopReason.Cancelled)
                return;

            string message = $"Download queue stopped early ({stopReason}). " +
                             $"Succeeded: {succeeded}, skipped: {skipped}, still queued: {_queuedDownloads.Count}.";

            _logger.Error(nameof(DownloadQueue), message);

            _analytics.LogEvent(
                "Download queue stopped early",
                new Dictionary<string, object>
                {
                    { "reason", stopReason.ToString() },
                    { "succeeded", succeeded },
                    { "skipped", skipped },
                    { "stillQueued", _queuedDownloads.Count }
                });
        }

        private enum QueueStopReason
        {
            Completed,
            NoConnection,
            Cancelled
        }
    }
}

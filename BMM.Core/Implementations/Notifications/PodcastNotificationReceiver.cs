using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Notifications.Data;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using MvvmCross.Localization;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Notifications
{
    public class PodcastNotificationReceiver : IReceive<PodcastNotification>, IDisposable
    {
        private readonly IAnalytics _analytics;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IGlobalMediaDownloader _mediaDownloader;
        private readonly IDownloadQueue _downloadQueue;
        private readonly IMvxMessenger _messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IUiDependentExecutor _executor;

        private MvxSubscriptionToken _completedToken;

        private PodcastNotification _podcastNotificationOfDownloadFile;
        private TaskCompletionSource<bool> _tcs;

        public PodcastNotificationReceiver(
            IGlobalMediaDownloader mediaDownloader,
            IAnalytics analytics,
            IMvxMessenger messenger,
            IExceptionHandler exceptionHandler,
            IDownloadQueue downloadQueue,
            IMvxNavigationService navigationService,
            IUiDependentExecutor executor)
        {
            _mediaDownloader = mediaDownloader;
            _analytics = analytics;
            _exceptionHandler = exceptionHandler;
            _downloadQueue = downloadQueue;
            _navigationService = navigationService;
            _executor = executor;
            _messenger = messenger;

            _tcs = new TaskCompletionSource<bool>();
        }

        public void Dispose()
        {
            _tcs.SetCanceled();
            _tcs.Task.Wait(500);
            _tcs = null;
            GC.SuppressFinalize(this);
        }

        public void UserClickedRemoteNotification(PodcastNotification notification)
        {
            var podcast = new StartPlayingPodcast { Id = notification.PodcastId, Title = "", TrackId = notification.TrackIds.First() };
            // The notification handling is happening in parallel with the startup. Therefore the UI might not be ready yet to navigate to the PodcastViewModel here.
            _executor.ExecuteWhenReady(() =>
            {
                _exceptionHandler.FireAndForgetOnMainThread(async () =>
                {
                    await _navigationService.Navigate<PodcastViewModel, StartPlayingPodcast>(podcast);
                });
            });
        }

        public void OnNotificationReceived(PodcastNotification podcastNotification)
        {
            _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
            {
                _tcs = new TaskCompletionSource<bool>();

                LogPodcastEvent("Podcast notification received", podcastNotification);
                _podcastNotificationOfDownloadFile = podcastNotification;

                _completedToken = _messenger.Subscribe<QueueFinishedMessage>(QueueFinished);

                PodcastLoggingExtensions.PodcastTrackIdToDownload = podcastNotification.TrackIds.First();
                await _mediaDownloader.SynchronizeOfflineTracks();
                await AllDownloadsCompleted(podcastNotification);
                PodcastLoggingExtensions.PodcastTrackIdToDownload = null;

                LogPodcastEvent("Podcast notification offline tracks synchronized", podcastNotification);
            });
        }

        private async Task AllDownloadsCompleted(PodcastNotification podcastNotification)
        {
            if (_downloadQueue.RemainingDownloadsCount > 0)
            {
                LogPodcastEvent("Waiting for completing all downloads...", podcastNotification);
                var succeeded = await _tcs.Task;

                if (succeeded)
                    LogPodcastEvent("New podcast successfully downloaded", podcastNotification);
                else
                    LogPodcastEvent("Download of new podcast was interrupted", podcastNotification);
            }
            else
            {
                LogPodcastEvent("No new downloadable tracks found", podcastNotification);
            }
        }

        private void QueueFinished(QueueFinishedMessage message)
        {
            if (_tcs.Task.IsCompleted)
                LogPodcastEvent("DownloadManager tries to be completed a second time: ", _podcastNotificationOfDownloadFile);
            else
                _tcs.TrySetResult(message.Succeeded);

            _messenger.Unsubscribe<QueueFinishedMessage>(_completedToken);
        }

        private void LogPodcastEvent(string eventName, PodcastNotification notification)
        {
            try
            {
                _analytics.LogEvent("PodcastNotification - " + eventName, new Dictionary<string, object>
                {
                    {"podcastId", notification.PodcastId},
                    {"trackIds", string.Join(",", notification.TrackIds)}
                });
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
            }
        }
    }
}
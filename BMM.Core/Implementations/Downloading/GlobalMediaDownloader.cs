using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.Security;
using BMM.Core.Messages;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading
{
    public class GlobalMediaDownloader : IGlobalMediaDownloader
    {
        private readonly IAnalytics _analytics;
        private readonly IBMMClient _client;
        private readonly IConnection _connection;
        private readonly IDownloadQueue _downloadQueue;
        private readonly IAppContentLogger _appContentLogger;
        private readonly IGlobalTrackProvider _globalTrackProvider;
        private readonly IAppLanguageProvider _appLanguageProvider;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IMvxMessenger _messenger;
        private readonly INetworkSettings _networkSettings;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IStorageManager _storageManager;
        private readonly IUserStorage _user;
        private readonly IFirebaseRemoteConfig _config;

        public GlobalMediaDownloader(IStorageManager storageManager,
            IExceptionHandler exceptionHandler,
            IAnalytics analytics,
            INetworkSettings networkSettings,
            IConnection connection,
            IMvxMessenger messenger,
            IBMMClient client,
            IDownloadQueue downloadQueue,
            IAppContentLogger appContentLogger,
            IGlobalTrackProvider globalTrackProvider,
            IAppLanguageProvider appLanguageProvider,
            IUserStorage user,
            IFirebaseRemoteConfig config)
        {
            _storageManager = storageManager;
            _exceptionHandler = exceptionHandler;
            _analytics = analytics;
            _networkSettings = networkSettings;
            _connection = connection;
            _messenger = messenger;
            _client = client;
            _downloadQueue = downloadQueue;
            _appContentLogger = appContentLogger;
            _globalTrackProvider = globalTrackProvider;
            _appLanguageProvider = appLanguageProvider;
            _user = user;
            _config = config;
        }

        public async Task InitializeCacheAndSynchronizeTracks()
        {
            await _client.Podcast.GetAll(CachePolicy.ForceGetAndUpdateCache);
            await _client.TrackCollection.GetAll(CachePolicy.ForceGetAndUpdateCache);
            await UpdateOfflineTracks();
        }


        public async Task SynchronizeOfflineTracks()
        {
            await _appContentLogger.LogAppContent("Before SynchronizeOfflineTracks");
            await RunSynchronizationTask(UpdateOfflineTracks);
            await _appContentLogger.LogAppContent("After SynchronizeOfflineTracks");
        }

        private async Task<bool> CheckConnectivity()
        {
            var mobileNetworkDownloadAllowed = await _networkSettings.GetMobileNetworkDownloadAllowed();

            var isUsingNetworkWithoutExtraCosts = _connection.IsUsingNetworkWithoutExtraCosts();

            return isUsingNetworkWithoutExtraCosts || mobileNetworkDownloadAllowed;
        }

        private bool CheckIfDownloadableUrlIsInsideListOfPaths(IDownloadable downloadable, IEnumerable<string> listOfPaths)
        {
            var pathForDownloadable = _storageManager.SelectedStorage.GetUrlByFile(downloadable);
            return listOfPaths.Contains(pathForDownloadable);
        }

        private async Task RunSynchronizationTask(Func<Task> synchronizationTask, [CallerMemberName] string propertyName = null)
        {
            await _semaphore.WaitAsync();
            try
            {
                var connectionStatus = _connection.GetStatus();
                var isOnline = connectionStatus == ConnectionStatus.Online;

                if (!isOnline)
                {
                    _analytics.LogEvent($"{propertyName} - Application is offline, do nothing");
                    return;
                }

                if (await CheckConnectivity())
                {
                    await synchronizationTask.Invoke();
                    _analytics.LogEvent($"{propertyName} - Application is online, try to update offline tracks");
                }
                else
                    _analytics.LogEvent($"{propertyName} - Application is online, but connection settings not allow to download tracks");
            }
            catch (Exception exception)
            {
                _messenger.Publish(new FileDownloadCanceledMessage(this, exception));
                _exceptionHandler.HandleExceptionWithoutUserMessages(exception);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Update the home screen so that people wake up to an updated version of the home screen and <see cref="ListeningStreak"/>.
        /// </summary>
        private Task UpdateHomescreen()
        {
            var age = _config.SendAgeToDiscover ? _user.GetUser().Age : null;
            return _client.Discover.GetDocuments(_appLanguageProvider.GetAppLanguage(), age, CachePolicy.UseCacheAndWaitForUpdates);
        }

        private async Task UpdateOfflineTracks()
        {
            await UpdateHomescreen();
            var tracksSupposedToBeDownloaded = (await _globalTrackProvider.GetTracksSupposedToBeDownloaded())
                .ToList();

            var currentlyDownloadedFilePaths = _storageManager.SelectedStorage.PathsOfDownloadedFiles();

            var tracksToBeDownloaded = tracksSupposedToBeDownloaded
                .Where(x => !CheckIfDownloadableUrlIsInsideListOfPaths(x, currentlyDownloadedFilePaths))
                .ToList();

            _analytics.LogEvent(
                "In SynchronizeOfflineTracks",
                new Dictionary<string, object>
                {
                    {"NumberOfTracksInPodcastsAndPlaylistsFromLocalStorage", tracksSupposedToBeDownloaded.Count}
                });

            _downloadQueue.DequeueAllExcept(tracksToBeDownloaded);

            if (tracksToBeDownloaded.Any())
            {
                _downloadQueue.Enqueue(tracksToBeDownloaded);
                _downloadQueue.StartDownloading();
            }

            var urlsOfTracksSupposedToBeDownloaded = tracksSupposedToBeDownloaded
                .SelectMany(t => t.Media)
                .SelectMany(t => t.Files)
                .Select(t => _storageManager.SelectedStorage.GetUrlByFile(t));

            RemoveUnnecessaryTracks(currentlyDownloadedFilePaths, urlsOfTracksSupposedToBeDownloaded);
        }

        private void RemoveUnnecessaryTracks(IEnumerable<string> currentlyDownloadedUrls, IEnumerable<string> supposedToBeDownloadedUrls)
        {
            var toBeDeleted = currentlyDownloadedUrls
                .Where(x => !supposedToBeDownloadedUrls.Contains(x))
                .ToList();

            toBeDeleted.ForEach(x => { _storageManager.SelectedStorage.DeleteFileByUrl(x); });

            _analytics.LogEvent(
                "Removed unnecessary tracks",
                new Dictionary<string, object>
                {
                    {"tracksDeleted", toBeDeleted.Count}
                });

            _messenger.Publish(new DownloadedEpisodeRemovedMessage(this));
        }
    }
}
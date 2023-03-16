using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.App.Interfaces;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.Storage;
using BMM.Core.Models.PlaybackHistory;
using BMM.Core.Models.Storage;

namespace BMM.Core.GuardedActions.App
{
    public class MigrateAkavacheToAppStorageAction : GuardedActionWithResult<bool>, IMigrateAkavacheToAppStorageAction
    {
        private readonly IBlobCache _blobCache;
        private readonly IAnalytics _analytics;

        public MigrateAkavacheToAppStorageAction(
            IBlobCache blobCache,
            IAnalytics analytics)
        {
            _blobCache = blobCache;
            _analytics = analytics;
        }

        protected override async Task<bool> Execute()
        {
            if (AppSettings.AkavacheMigrationFinished)
                return true;
            
            await MigrateValue<IList<string>>(StorageKeys.History, v => AppSettings.SearchHistory = v);
            await MigrateSecureValue<User>(StorageKeys.CurrentUser, v => AppSettings.CurrentUser = v);
            await MigrateValue<string>(StorageKeys.LanguageApp, v => AppSettings.LanguageApp = v);
            await MigrateValue<string[]>(StorageKeys.LanguageContent, v => AppSettings.LanguageContent = v);
            
            await MigrateValue<bool>(StorageKeys.MobileDownloadEnabled, v => AppSettings.MobileDownloadEnabled = v);
            await MigrateValue<bool?>(StorageKeys.AutoplayEnabled, v => AppSettings.AutoplayEnabled = v);
            await MigrateValue<bool>(StorageKeys.StreakHidden, v => AppSettings.StreakHidden = v);
            await MigrateValue<bool>(StorageKeys.PushNotificationsEnabled, v => AppSettings.PushNotificationsEnabled = v);
            await MigrateValue<bool>(StorageKeys.UseExternalStorage, v => AppSettings.UseExternalStorage = v);
            await MigrateValue<bool>(StorageKeys.FirstLaunchWithPodcasts, v => AppSettings.FirstLaunchWithPodcasts = v);

            await MigrateValue<HashSet<int>>(StorageKeys.LocalPodcasts, v => AppSettings.LocalPodcasts = v);
            await MigrateValue<Dictionary<int, int>>(StorageKeys.AutomaticallyDownloadedTracks, v => AppSettings.AutomaticallyDownloadedTracks = v);
            await MigrateValue<HashSet<int>>(StorageKeys.LocalTrackCollections, v => AppSettings.LocalTrackCollections = v);
            
            await MigrateValue<TrackPlayedEvent>(StorageKeys.UnfinishedTrackPlayedEvent, v => AppSettings.UnfinishedTrackPlayedEvent = v);
            await MigrateValue<IList<TrackPlayedEvent>>(StorageKeys.FinishedTrackPlayedEvents, v => AppSettings.FinishedTrackPlayedEvents = v);
            await MigrateValue<PersistedDownloadable>(StorageKeys.CurrentDownload, v => AppSettings.CurrentDownload = v);
            await MigrateValue<ListeningStreak>(StorageKeys.LatestListeningStreak, v => AppSettings.LatestListeningStreak = v);
            
            await MigrateValue<HashSet<int>>(StorageKeys.ListenedTracks, v => AppSettings.ListenedTracks = v);
            await MigrateValue<HashSet<int>>(StorageKeys.LocalPlaylists, v => AppSettings.LocalPlaylists = v);
            await MigrateValue<List<PlaybackHistoryEntry>>(StorageKeys.PlaybackHistory, v => AppSettings.PlaybackHistory = v);
            await MigrateValue<IList<Track>>(StorageKeys.RememberedQueue, v => AppSettings.RememberedQueue = v);
            await MigrateValue<CurrentTrackPositionStorage>(StorageKeys.CurrentTrackPosition, v => AppSettings.CurrentTrackPosition = v);

            AppSettings.AkavacheMigrationFinished = true;
            _analytics.LogEvent(Event.AkavacheMigrationFinished);
            return true;
        }
        
        private async Task MigrateValue<T>(string key, Action<T> appSettingsSafeAction)
        {
            try
            {
                var value = await _blobCache.GetObject<T>(key);
                appSettingsSafeAction.Invoke(value);
            }
            catch (Exception ex)
            {
                //ignore
            }
        }
        
        private async Task MigrateSecureValue<T>(string key, Action<T> appSettingsSafeAction)
        {
            try
            {
                var value = await BlobCache.Secure.GetObject<T>(key);
                appSettingsSafeAction.Invoke(value);
            }
            catch (Exception ex)
            {
                //ignore
            }
        }
    }
}
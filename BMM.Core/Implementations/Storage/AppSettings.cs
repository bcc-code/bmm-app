using System.Collections.Generic;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Models.PlaybackHistory;
using BMM.Core.Models.Storage;
using BMM.Core.Models.Themes;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace BMM.Core.Implementations.Storage
{
    public static class AppSettings
    {
        private static ISettings Settings => CrossSettings.Current;

        public static Theme SelectedTheme
        {
            get => GetValueOrDefault(nameof(SelectedTheme), Theme.System);
            set => AddOrUpdateValue(value, nameof(SelectedTheme));
        }

        public static bool YearInReviewShown
        {
            get => GetValueOrDefault(nameof(YearInReviewShown), false);
            set => AddOrUpdateValue(value, nameof(YearInReviewShown));
        }
        
        public static IList<int> DismissedMessageTilesIds
        {
            get => GetValueOrDefault(nameof(DismissedMessageTilesIds), new List<int>());
            set => AddOrUpdateValue(value, nameof(DismissedMessageTilesIds));
        }
        
        public static IList<string> SearchHistory
        {
            get => GetValueOrDefault(nameof(SearchHistory), new List<string>());
            set => AddOrUpdateValue(value, nameof(SearchHistory));
        }
        
        public static User CurrentUser
        {
            get => GetValueOrDefault(nameof(CurrentUser), default(User));
            set => AddOrUpdateValue(value, nameof(CurrentUser));
        }
        
        public static string LanguageApp
        {
            get => GetValueOrDefault(nameof(LanguageApp), default(string));
            set => AddOrUpdateValue(value, nameof(LanguageApp));
        }
        
        public static string[] LanguageContent
        {
            get => GetValueOrDefault(nameof(LanguageContent), default(string[]));
            set => AddOrUpdateValue(value, nameof(LanguageContent));
        }
        
        public static bool MobileDownloadEnabled
        {
            get => GetValueOrDefault(nameof(MobileDownloadEnabled), GlobalConstants.MobileDownloadEnabledDefault);
            set => AddOrUpdateValue(value, nameof(MobileDownloadEnabled));
        }
        
        public static bool? AutoplayEnabled
        {
            get => GetValueOrDefault(nameof(AutoplayEnabled), default(bool?));
            set => AddOrUpdateValue(value, nameof(AutoplayEnabled));
        }
        
        public static bool StreakHidden
        {
            get => GetValueOrDefault(nameof(StreakHidden), default(bool));
            set => AddOrUpdateValue(value, nameof(StreakHidden));
        }
                
        public static bool PushNotificationsEnabled
        {
            get => GetValueOrDefault(nameof(PushNotificationsEnabled), GlobalConstants.PushNotificationsEnabledDefault);
            set => AddOrUpdateValue(value, nameof(PushNotificationsEnabled));
        }
                
        public static bool UseExternalStorage
        {
            get => GetValueOrDefault(nameof(UseExternalStorage), GlobalConstants.UseExternalStorageDefault);
            set => AddOrUpdateValue(value, nameof(UseExternalStorage));
        }
        
        public static HashSet<int> LocalPodcasts
        {
            get => GetValueOrDefault(nameof(LocalPodcasts), new HashSet<int>());
            set => AddOrUpdateValue(value, nameof(LocalPodcasts));
        }
        
        public static IDictionary<int, int> AutomaticallyDownloadedTracks
        {
            get => GetValueOrDefault(nameof(AutomaticallyDownloadedTracks), new Dictionary<int, int>());
            set => AddOrUpdateValue(value, nameof(AutomaticallyDownloadedTracks));
        }
        
        public static HashSet<int> LocalTrackCollections
        {
            get => GetValueOrDefault(nameof(LocalTrackCollections), new HashSet<int>());
            set => AddOrUpdateValue(value, nameof(LocalTrackCollections));
        }
        
        public static TrackPlayedEvent UnfinishedTrackPlayedEvent
        {
            get => GetValueOrDefault(nameof(UnfinishedTrackPlayedEvent), default(TrackPlayedEvent));
            set => AddOrUpdateValue(value, nameof(UnfinishedTrackPlayedEvent));
        }
        
        public static IList<TrackPlayedEvent> FinishedTrackPlayedEvents
        {
            get => GetValueOrDefault(nameof(FinishedTrackPlayedEvents), new List<TrackPlayedEvent>());
            set => AddOrUpdateValue(value, nameof(FinishedTrackPlayedEvents));
        }
        
        public static PersistedDownloadable CurrentDownload
        {
            get => GetValueOrDefault(nameof(CurrentDownload), default(PersistedDownloadable));
            set => AddOrUpdateValue(value, nameof(CurrentDownload));
        }
        
        public static ListeningStreak LatestListeningStreak
        {
            get => GetValueOrDefault(nameof(LatestListeningStreak), default(ListeningStreak));
            set => AddOrUpdateValue(value, nameof(LatestListeningStreak));
        }
        
        public static HashSet<int> ListenedTracks
        {
            get => GetValueOrDefault(nameof(ListenedTracks), new HashSet<int>());
            set => AddOrUpdateValue(value, nameof(ListenedTracks));
        }
        
        public static HashSet<int> LocalPlaylists
        {
            get => GetValueOrDefault(nameof(LocalPlaylists), new HashSet<int>());
            set => AddOrUpdateValue(value, nameof(LocalPlaylists));
        }
        
        public static IList<PlaybackHistoryEntry> PlaybackHistory
        {
            get => GetValueOrDefault(nameof(PlaybackHistory), new List<PlaybackHistoryEntry>());
            set => AddOrUpdateValue(value, nameof(PlaybackHistory));
        }
        
        public static IList<Track> RememberedQueue
        {
            get => GetValueOrDefault(nameof(RememberedQueue), new List<Track>());
            set => AddOrUpdateValue(value, nameof(RememberedQueue));
        }
        
        public static CurrentTrackPositionStorage CurrentTrackPosition
        {
            get => GetValueOrDefault(nameof(CurrentTrackPosition), default(CurrentTrackPositionStorage));
            set => AddOrUpdateValue(value, nameof(CurrentTrackPosition));
        }
        
        public static bool AkavacheMigrationFinished
        {
            get => GetValueOrDefault(nameof(AkavacheMigrationFinished), false);
            set => AddOrUpdateValue(value, nameof(AkavacheMigrationFinished));
        }
        
        public static bool FirstLaunchWithPodcasts
        {
            get => GetValueOrDefault(nameof(FirstLaunchWithPodcasts), true);
            set => AddOrUpdateValue(value, nameof(FirstLaunchWithPodcasts));
        }
        
        public static void Clear() => CrossSettings.Current.Clear();
        
        private static void AddOrUpdateValue<TValue>(TValue value, string settingsKey)
        {
            if (value is string stringValue)
            {
                Settings.AddOrUpdateValue(settingsKey, stringValue);
                return;
            }

            string serialized = value != null
                ? JsonConvert.SerializeObject(value)
                : null;

            Settings.AddOrUpdateValue(settingsKey, serialized);
        }

        private static TValue GetValueOrDefault<TValue>(string settingsKey, TValue defaultValue = default)
        {
            string value = Settings.GetValueOrDefault(settingsKey, null);

            switch (value)
            {
                case TValue stringValue:
                    return stringValue;
                case null:
                    return defaultValue;
                default:
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<TValue>(value);
                    }
                    catch
                    {
                        AddOrUpdateValue(defaultValue, settingsKey);
                        return defaultValue;
                    }
                }
            }
        }
    }
}
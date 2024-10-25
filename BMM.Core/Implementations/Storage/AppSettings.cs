using System.Collections.Generic;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Models.PlaybackHistory;
using BMM.Core.Models.Storage;
using BMM.Core.Models.Themes;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;

namespace BMM.Core.Implementations.Storage
{
    public static class AppSettings
    {
        private static IPreferences _settings;

        private static IPreferences Settings => _settings ?? Preferences.Default;

        public static void SetImplementation(IPreferences settings) => _settings = settings;
        public static void ClearImplementation() => _settings = null;

        public static Theme SelectedTheme
        {
            get => GetValueOrDefault(nameof(SelectedTheme), Theme.System);
            set => AddOrUpdateValue(value, nameof(SelectedTheme));
        }
        
        public static ColorTheme SelectedColorTheme
        {
            get => GetValueOrDefault(nameof(SelectedColorTheme), ColorTheme.Default);
            set => AddOrUpdateValue(value, nameof(SelectedColorTheme));
        }

        public static bool YearInReviewShown
        {
            get => GetValueOrDefault(nameof(YearInReviewShown), false);
            set => AddOrUpdateValue(value, nameof(YearInReviewShown));
        }

        public static bool IsProjectBoxExpanded(int projectId, bool defaultValue)
        {
            return GetValueOrDefault($"{nameof(IsProjectBoxExpanded)}_{projectId}", defaultValue);
        }
        
        public static void SetIsProjectBoxExpanded(int projectId, bool value)
        {
            AddOrUpdateValue(value, $"{nameof(IsProjectBoxExpanded)}_{projectId}");
        }

        public static bool HasAutoSubscribed(int podcastId)
        {
            if (podcastId == PodcastsConstants.FraKårePodcastId)
                return !FirstLaunchWithPodcasts;
            return GetValueOrDefault($"{nameof(HasAutoSubscribed)}_{podcastId}", false);
        }

        public static void SetHasAutoSubscribed(int podcastId)
        {
            if (podcastId == PodcastsConstants.FraKårePodcastId)
                FirstLaunchWithPodcasts = false;
            AddOrUpdateValue(true, $"{nameof(HasAutoSubscribed)}_{podcastId}");
        }
        
        /// <summary>
        /// Stores whether FraKåre has been auto subscribed yet or not.
        /// Returns the inverse of HasAutoSubscribed(1);
        /// </summary>
        [Obsolete("We keep this to stay backwards compatible")]
        public static bool FirstLaunchWithPodcasts
        {
            get => GetValueOrDefault(nameof(FirstLaunchWithPodcasts), true);
            set => AddOrUpdateValue(value, nameof(FirstLaunchWithPodcasts));
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
        
        public static bool? PlayInChronologicalOrderEnabled
        {
            get => GetValueOrDefault(nameof(PlayInChronologicalOrderEnabled), default(bool?));
            set => AddOrUpdateValue(value, nameof(PlayInChronologicalOrderEnabled));
        }
        
        public static bool StreakHidden
        {
            get => GetValueOrDefault(nameof(StreakHidden), default(bool));
            set => AddOrUpdateValue(value, nameof(StreakHidden));
        }
        
        public static bool BibleStudyBadgeEnabled
        {
            get => GetValueOrDefault(nameof(BibleStudyBadgeEnabled), true);
            set => AddOrUpdateValue(value, nameof(BibleStudyBadgeEnabled));
        }
        
        public static bool BibleStudyOnHomeEnabled
        {
            get => GetValueOrDefault(nameof(BibleStudyOnHomeEnabled), true);
            set => AddOrUpdateValue(value, nameof(BibleStudyOnHomeEnabled));
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
        
        public static IList<StreakPointEvent> UnsentStreakPointEvent
        {
            get => GetValueOrDefault(nameof(UnsentStreakPointEvent), new List<StreakPointEvent>());
            set => AddOrUpdateValue(value, nameof(UnsentStreakPointEvent));
        }

        public static IList<ListeningEvent> UnsentListeningEvent
        {
            get => GetValueOrDefault(nameof(UnsentListeningEvent), new List<ListeningEvent>());
            set => AddOrUpdateValue(value, nameof(UnsentListeningEvent));
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
        
        public static HashSet<int> LocalAlbums
        {
            get => GetValueOrDefault(nameof(LocalAlbums), new HashSet<int>());
            set => AddOrUpdateValue(value, nameof(LocalAlbums));
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
        
        public static bool HighlightedTextPopupAlreadyShown
        {
            get => GetValueOrDefault(nameof(HighlightedTextPopupAlreadyShown), false);
            set => AddOrUpdateValue(value, nameof(HighlightedTextPopupAlreadyShown));
        }
        
        public static bool DarkGreenRewardUnlocked
        {
            get => GetValueOrDefault(nameof(DarkGreenRewardUnlocked), false);
            set => AddOrUpdateValue(value, nameof(DarkGreenRewardUnlocked));
        }
        
        public static bool OrangeRewardUnlocked
        {
            get => GetValueOrDefault(nameof(OrangeRewardUnlocked), false);
            set => AddOrUpdateValue(value, nameof(OrangeRewardUnlocked));
        }
        
        public static bool VioletRewardUnlocked
        {
            get => GetValueOrDefault(nameof(VioletRewardUnlocked), false);
            set => AddOrUpdateValue(value, nameof(VioletRewardUnlocked));
        }
        
        public static bool RedRewardUnlocked
        {
            get => GetValueOrDefault(nameof(RedRewardUnlocked), false);
            set => AddOrUpdateValue(value, nameof(RedRewardUnlocked));
        }
        
        public static bool GoldenRewardUnlocked
        {
            get => GetValueOrDefault(nameof(GoldenRewardUnlocked), false);
            set => AddOrUpdateValue(value, nameof(GoldenRewardUnlocked));
        }
        
        public static bool IsBadgeSet
        {
            get => GetValueOrDefault(nameof(IsBadgeSet), false);
            set => AddOrUpdateValue(value, nameof(IsBadgeSet));
        }
            
        public static DateTime BadgeSetAt
        {
            get => GetValueOrDefault(nameof(BadgeSetAt), DateTime.MinValue);
            set => AddOrUpdateValue(value, nameof(BadgeSetAt));
        }

        public static Guid DeviceId => GetValueOrCreateDefault(nameof(DeviceId), Guid.NewGuid());

        public static void Clear() => Settings.Clear();
        
        private static void AddOrUpdateValue<TValue>(TValue value, string settingsKey)
        {
            if (value is string stringValue)
            {
                Settings.Set(settingsKey, stringValue);
                return;
            }

            string? serialized = value != null
                ? JsonConvert.SerializeObject(value)
                : null;

            Settings.Set(settingsKey, serialized);
        }

        private static TValue GetValueOrDefault<TValue>(string settingsKey, TValue defaultValue = default)
        {
            string value = Settings.Get<string>(settingsKey, null);

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

        private static TValue GetValueOrCreateDefault<TValue>(string settingsKey, TValue defaultValue)
        {
            string value = Settings.Get<string>(settingsKey, null);

            switch (value)
            {
                case TValue stringValue:
                    return stringValue;
                case null:
                    AddOrUpdateValue(defaultValue, settingsKey);
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
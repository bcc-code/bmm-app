using System.Threading.Tasks;
using BMM.Core.Implementations.FeatureToggles;

namespace BMM.Core.Implementations.FirebaseRemoteConfig
{
    public interface IFirebaseRemoteConfig
    {
        Task UpdateValuesFromFirebaseRemoteConfig();

        string[] SupportedContentLanguages { get; }

        SemanticVersion AndroidVersionPlannedToBeUnsupported { get; }

        SemanticVersion IosVersionPlannedToBeUnsupported { get; }

        SemanticVersion MinimumRequiredAndroidVersion { get; }

        SemanticVersion MinimumRequiredIosVersion { get; }

        SemanticVersion MinimumRequiredAppVersion { get; }

        SemanticVersion AppVersionPlannedToBeUnsupported { get; }

        string EditProfileUrl { get; }

        string IdentityUserInfoEndpoint { get; }

        bool AutoplayEnabledDefaultSetting { get; }

        bool UseExtendedStreakLogging { get; }

        string UserVoiceLink { get; }
        
        string SongTreasuresSongLink  { get; }
        
        string ExperimentId { get; }
        
        bool SendAgeToDiscover { get; }
        
        bool IsSleepTimerEnabled { get; }
        
        bool IsPlaybackSpeedEnabled { get; }
    }
}
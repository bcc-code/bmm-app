using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FeatureToggles;

namespace BMM.Core.Implementations.FirebaseRemoteConfig
{
    public class FirebaseRemoteConfig : IFirebaseRemoteConfig
    {
        private readonly IPlatformSpecificRemoteConfig _platformSpecificRemoteConfig;
        private readonly SemanticVersionParser _semanticVersionParser;

        private class Variables
        {
            public const string SupportedContentLanguages = "supported_languages";

            public const string MinimumRequiredAndroidVersion = "minimum_required_android_version";
            public const string MinimumRequiredIosVersion = "minimum_required_ios_version";

            public const string AndroidVersionPlannedToBeUnsupported = "android_version_planned_to_be_unsupported";
            public const string IosVersionPlannedToBeUnsupported = "ios_version_planned_to_be_unsupported";

            public const string MinimumRequiredAppVersion = "minimum_required_app_version";
            public const string AppVersionPlannedToBeUnsupported = "app_version_planned_to_be_unsupported";

            public const string EditProfileUrl = "edit_profile_url";
            public const string IdentityUserInfoEndpoint = "identity_user_info_endpoint";

            public const string UseExtendedStreakLogging = "extended_streak_logging";
            public const string AutoplayEnabledDefaultSetting = "autoplay_enabled_default_setting";

            public const string UseAnalyticsId = "use_analytics_id";

            public const string UserVoiceLink = "user_voice_link";
        }

        public static readonly Dictionary<string, string> Defaults = new Dictionary<string, string>
        {
            {Variables.SupportedContentLanguages, GlobalConstants.DefaultContentLanguages},
            {Variables.AndroidVersionPlannedToBeUnsupported, GlobalConstants.DefaultAndroidVersionPlannedToBeUnsupported},
            {Variables.MinimumRequiredAndroidVersion, GlobalConstants.DefaultMinimumRequiredAndroidVersion},
            {Variables.IosVersionPlannedToBeUnsupported, GlobalConstants.DefaultIosVersionPlannedToBeUnsupported},
            {Variables.MinimumRequiredIosVersion, GlobalConstants.DefaultMinimumRequiredIosVersion},
            {Variables.MinimumRequiredAppVersion, GlobalConstants.DefaultMinimumRequiredAppVersion},
            {Variables.AppVersionPlannedToBeUnsupported, GlobalConstants.DefaultAppVersionPlannedToBeUnsupported},
            {Variables.EditProfileUrl, "https://members.bcc.no/profile/"},
            {Variables.IdentityUserInfoEndpoint, "https://login.bcc.no/userinfo"},
            {Variables.UseExtendedStreakLogging, true.ToString()},
            {Variables.AutoplayEnabledDefaultSetting, false.ToString()},
            {Variables.UseAnalyticsId, false.ToString()},
            {Variables.UserVoiceLink, "https://uservoice.bcc.no/?tags=bmm"}
        };

        public FirebaseRemoteConfig(IPlatformSpecificRemoteConfig platformSpecificRemoteConfig, SemanticVersionParser semanticVersionParser)
        {
            _platformSpecificRemoteConfig = platformSpecificRemoteConfig;
            _semanticVersionParser = semanticVersionParser;
        }

        public async Task UpdateValuesFromFirebaseRemoteConfig()
        {
            await _platformSpecificRemoteConfig.UpdateValuesFromFirebaseRemoteConfig();
        }

        public bool UseAnalyticsId => _platformSpecificRemoteConfig.GetBoolValue(Variables.UseAnalyticsId);

        public string UserVoiceLink => _platformSpecificRemoteConfig.GetStringValue(Variables.UserVoiceLink);

        public bool AutoplayEnabledDefaultSetting => _platformSpecificRemoteConfig.GetBoolValue(Variables.AutoplayEnabledDefaultSetting);

        public bool UseExtendedStreakLogging => _platformSpecificRemoteConfig.GetBoolValue(Variables.UseExtendedStreakLogging);

        public string EditProfileUrl => _platformSpecificRemoteConfig.GetStringValue(Variables.EditProfileUrl);

        public string IdentityUserInfoEndpoint => _platformSpecificRemoteConfig.GetStringValue(Variables.IdentityUserInfoEndpoint);

        public string[] SupportedContentLanguages => _platformSpecificRemoteConfig.GetStringValue(Variables.SupportedContentLanguages).Split(',');

        public SemanticVersion AndroidVersionPlannedToBeUnsupported =>
            _semanticVersionParser.ParseStringToSemanticVersionObject(_platformSpecificRemoteConfig.GetStringValue(Variables.AndroidVersionPlannedToBeUnsupported));

        public SemanticVersion MinimumRequiredAndroidVersion =>
            _semanticVersionParser.ParseStringToSemanticVersionObject(_platformSpecificRemoteConfig.GetStringValue(Variables.MinimumRequiredAndroidVersion));

        public SemanticVersion IosVersionPlannedToBeUnsupported =>
            _semanticVersionParser.ParseStringToSemanticVersionObject(_platformSpecificRemoteConfig.GetStringValue(Variables.IosVersionPlannedToBeUnsupported));

        public SemanticVersion MinimumRequiredIosVersion =>
            _semanticVersionParser.ParseStringToSemanticVersionObject(_platformSpecificRemoteConfig.GetStringValue(Variables.MinimumRequiredIosVersion));

        public SemanticVersion MinimumRequiredAppVersion =>
            _semanticVersionParser.ParseStringToSemanticVersionObject(_platformSpecificRemoteConfig.GetStringValue(Variables.MinimumRequiredAppVersion));

        public SemanticVersion AppVersionPlannedToBeUnsupported =>
            _semanticVersionParser.ParseStringToSemanticVersionObject(_platformSpecificRemoteConfig.GetStringValue(Variables.AppVersionPlannedToBeUnsupported));
    }
}
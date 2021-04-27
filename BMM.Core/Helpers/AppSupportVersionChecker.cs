using BMM.Core.Implementations.FeatureToggles;
using BMM.Core.Implementations.FirebaseRemoteConfig;

namespace BMM.Core.Helpers
{
    public class AppSupportVersionChecker
    {
        private readonly IFirebaseRemoteConfig _remoteConfig;
        private readonly SemanticVersionParser _semanticVersionParser;
        private readonly SemanticVersionComparer _semanticVersionComparer;

        private readonly SemanticVersion _currentAppVersion;
        private readonly SemanticVersion _minimumRequiredVersion;
        private readonly SemanticVersion _versionToBeUnsupported;

        public AppSupportVersionChecker(IFirebaseRemoteConfig remoteConfig, SemanticVersionParser semanticVersionParser, SemanticVersionComparer semanticVersionComparer)
        {
            _remoteConfig = remoteConfig;
            _semanticVersionParser = semanticVersionParser;
            _semanticVersionComparer = semanticVersionComparer;

            _currentAppVersion = _semanticVersionParser.ParseStringToSemanticVersionObject(GlobalConstants.AppVersion);
            _minimumRequiredVersion = _remoteConfig.MinimumRequiredAppVersion;
            _versionToBeUnsupported = _remoteConfig.AppVersionPlannedToBeUnsupported;
        }

        public bool IsCurrentAppVersionSupported()
        {
            return _semanticVersionComparer.SatisfiesMinVersion(_currentAppVersion, _minimumRequiredVersion);
        }

        public bool IsCurrentAppVersionPlannedToBeUnsupported()
        {
            return _semanticVersionComparer.LessThanOrEqual(_currentAppVersion, _versionToBeUnsupported);
        }
    }
}

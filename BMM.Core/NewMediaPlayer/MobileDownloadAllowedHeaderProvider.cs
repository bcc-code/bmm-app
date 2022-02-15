using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Implementations.Connection;

namespace BMM.Core.NewMediaPlayer
{
    public class MobileDownloadAllowedHeaderProvider : IHeaderProvider, INeedInitialization
    {
        private readonly INetworkSettings _networkSettings;

        /// <summary>
        /// To prevent wasting time during each request we cache the value the first time it's accessed.
        /// Whenever the user changes the setting it's returning the wrong value but since it's only for analytics that is an acceptable trade-off.
        /// </summary>
        private bool? _mobileDownloadAllowedAtAppStart;

        public MobileDownloadAllowedHeaderProvider(INetworkSettings networkSettings)
        {
            _networkSettings = networkSettings;
        }

        public async Task InitializeWhenLoggedIn()
        {
            _mobileDownloadAllowedAtAppStart = await _networkSettings.GetMobileNetworkDownloadAllowed();
        }

        public async Task<KeyValuePair<string, string>?> GetHeader()
        {
            return new KeyValuePair<string, string>("MobileDownloadAllowed", IsMobileDownloadAllowed().ToString());
        }

        private bool IsMobileDownloadAllowed()
        {
            if (_mobileDownloadAllowedAtAppStart.HasValue)
                return _mobileDownloadAllowedAtAppStart.Value;
            
            return false;
        }
    }
}
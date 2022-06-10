using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Constants;
using BMM.Core.Implementations.Security;

namespace BMM.Core.NewMediaPlayer
{
    public class BearerTokenSyncAuthorizationHeaderProvider : ISyncAuthorizationHeaderProvider
    {
        private readonly IAccessTokenProvider _accessTokenProvider;

        public BearerTokenSyncAuthorizationHeaderProvider(IAccessTokenProvider accessTokenProvider)
        {
            _accessTokenProvider = accessTokenProvider;
        }
        
        /// <summary>
        /// Can't do async in ExoPlayer when trying to play a track, so we can't wait for token refresh.
        /// Not a problem since the token should have been refreshed by that point anyways.
        /// </summary>
        public async Task<KeyValuePair<string, string>?> GetHeader()
        {
            await Task.CompletedTask;
            return new KeyValuePair<string, string>(HeaderNames.Authorization, $"Bearer {_accessTokenProvider.AccessToken}");
        }
    }
}
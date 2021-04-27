using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Implementations.Security;

namespace BMM.Core.NewMediaPlayer
{
    public class BearerTokenAuthorizationHeaderProvider : IAuthorizationHeaderProvider
    {
        private readonly IAccessTokenProvider _accessTokenProvider;

        public BearerTokenAuthorizationHeaderProvider(IAccessTokenProvider accessTokenProvider)
        {
            _accessTokenProvider = accessTokenProvider;
        }

        public async Task<KeyValuePair<string, string>> GetHeader()
        {
            var accessToken = await _accessTokenProvider.GetAccessToken().ConfigureAwait(false);
            return new KeyValuePair<string, string>("Authorization", $"Bearer {accessToken}");
        }
    }
}
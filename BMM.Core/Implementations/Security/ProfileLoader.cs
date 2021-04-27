using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework.HTTP;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using Newtonsoft.Json;

namespace BMM.Core.Implementations.Security
{
    public class ProfileLoader : IProfileLoader
    {
        private readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;
        private readonly IResponseDeserializer _responseDeserializer;
        private readonly IFirebaseRemoteConfig _remoteConfig;
        private HttpClient _client;

        public ProfileLoader(IAuthorizationHeaderProvider authorizationHeaderProvider, IResponseDeserializer responseDeserializer, IFirebaseRemoteConfig remoteConfig)
        {
            _authorizationHeaderProvider = authorizationHeaderProvider;
            _responseDeserializer = responseDeserializer;
            _remoteConfig = remoteConfig;
        }

        public async Task<ProfileResponse> LoadProfile()
        {
            if (_client == null)
            {
                _client = new HttpClient();
                var authHeader = await _authorizationHeaderProvider.GetHeader();
                _client.DefaultRequestHeaders.Add(authHeader.Key, authHeader.Value);
            }

            var response = await _client.GetAsync(_remoteConfig.IdentityUserInfoEndpoint);
            return await _responseDeserializer.DeserializeResponse<ProfileResponse>(response);
        }

        public class ProfileResponse
        {
            [JsonProperty("given_name")]
            public string GivenName { get; set; }
            [JsonProperty("family_name")]
            public string FamilyName { get; set; }
            public string Nickname { get; set; }
            public string Picture { get; set; }
            public string Gender { get; set; }
        }
    }
}

using System;
using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Security.Oidc.Interfaces;
using BMM.Core.Implementations.Startup;
using IdentityModel.OidcClient;

namespace BMM.Core.Implementations.Security.Oidc
{
    public class OidcUserStartupTask : IDelayedStartupTask
    {
        private readonly IOidcCredentialsStorage _credentialsStorage;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly IUserStorage _userStorage;
        private readonly IClaimUserInformationExtractor _userInformationExtractor;
        private readonly OidcClient _client;

        public OidcUserStartupTask(
            IOidcCredentialsStorage credentialsStorage,
            IAccessTokenProvider accessTokenProvider,
            IUserStorage userStorage,
            IClaimUserInformationExtractor userInformationExtractor
        )
        {
            _credentialsStorage = credentialsStorage;
            _accessTokenProvider = accessTokenProvider;
            _userStorage = userStorage;
            _userInformationExtractor = userInformationExtractor;

            var options = new OidcClientOptions
            {
                Authority = OidcConstants.AuthorizationServerUrl,
                ClientId = OidcConstants.ClientId,
                Scope = OidcConstants.Scopes,
                RedirectUri = OidcConstants.LoginRedirectUrl,
                Policy = {RequireAccessTokenHash = false},
                Browser = null,
            };

            _client = new OidcClient(options);
        }

        public async Task RunAfterStartup()
        {
            var user = _userStorage.GetUser();
            var accessTokenFromStorage = await _credentialsStorage.GetAccessToken();

            if (user != null && !string.IsNullOrEmpty(accessTokenFromStorage) &&
                (user.LastUpdated == null || user.LastUpdated.Value < DateTime.UtcNow.AddDays(-7)))
            {
                var result = await _client.GetUserInfoAsync(await _accessTokenProvider.GetAccessToken());
                if (result.IsError)
                    return;

                var userInfo = _userInformationExtractor.ExtractUser(result.Claims);

                user.AnalyticsId = userInfo.AnalyticsId;
                user.ProfileImage = userInfo.ProfileImage;
                user.FirstName = userInfo.FirstName;
                user.LastName = userInfo.LastName;
                user.Birthdate = userInfo.Birthdate;
                user.LastUpdated = DateTime.UtcNow;

                _userStorage.StoreUser(user);
            }
        }
    }
}
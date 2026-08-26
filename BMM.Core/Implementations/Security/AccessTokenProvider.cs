using System;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Security.Oidc.Interfaces;
using BMM.Core.Messages;
using BMM.Core.Models.App;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Security
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        public const int TimeToRefreshTokenBeforeExpirationInHours = 3;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly IOidcCredentialsStorage _credentialsStorage;
        private readonly IOidcAuthService _authService;
        private readonly ILogger _logger;
        private readonly IJwtTokenReader _jwtTokenReader;
        private MvxSubscriptionToken _loggedOutMessageToken;
        private bool _initialized;

        public AccessTokenProvider(
            IOidcCredentialsStorage credentialsStorage,
            IOidcAuthService authService,
            IMvxMessenger messenger,
            ILogger logger,
            IJwtTokenReader jwtTokenReader)
        {
            _credentialsStorage = credentialsStorage;
            _authService = authService;
            _logger = logger;
            _jwtTokenReader = jwtTokenReader;
            _loggedOutMessageToken = messenger.Subscribe<LoggedOutMessage>(message =>
            {
                _initialized = false;
                AccessToken = null;
            });
        }

        private string _accessToken;
        private DateTime _accessTokenExpiration = DateTime.MinValue;

        /// <summary>
        /// Reading the expiration date means parsing the JWT, which is far too expensive to do on every
        /// request now that media and image requests ask for the token as well. The token only changes
        /// when it is set, so the expiration date is worked out here and cached alongside it.
        /// </summary>
        public string AccessToken
        {
            get => _accessToken;
            private set
            {
                _accessToken = value;
                _accessTokenExpiration = ReadExpirationDate(value);
            }
        }

        public AccessTokenState CheckAccessTokenState()
        {
            var expirationDate = GetTokenExpirationDate();

            if (expirationDate < DateTime.UtcNow)
                return AccessTokenState.Expired;
            
            if (expirationDate < DateTime.UtcNow.AddHours(TimeToRefreshTokenBeforeExpirationInHours))
                return AccessTokenState.AboutToExpire;
            
            return AccessTokenState.Valid;
        }

        public async Task<string> GetAccessToken()
        {
            if (!_initialized)
                await Initialize();
            
            await UpdateAccessTokenIfNeeded();
            return AccessToken;
        }

        public async Task Initialize()
        {
            AccessToken = await _credentialsStorage.GetAccessToken();
            _initialized = true;
        }

        public async Task UpdateAccessTokenIfNeeded()
        {
            var accessTokenState = CheckAccessTokenState();
            
            switch (accessTokenState)
            {
                case AccessTokenState.Expired:
                    await RefreshAccessToken();
                    break;
                case AccessTokenState.AboutToExpire:
                    Task.Run(RefreshAccessToken).FireAndForget();
                    break;
            }
        }

        public DateTime GetTokenExpirationDate() => _accessTokenExpiration;

        /// <summary>
        /// A missing or unreadable token counts as expired, so callers go and fetch a real one. Without
        /// this the JWT reader throws on a null token, which would surface as a failed media or image
        /// request rather than as a token refresh.
        /// </summary>
        private DateTime ReadExpirationDate(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return DateTime.MinValue;

            try
            {
                return _jwtTokenReader.GetExpirationTime(accessToken);
            }
            catch (Exception ex)
            {
                _logger.Warn(GetType().Name, $"Could not read the expiration date of the access token, treating it as expired. {ex.Message}");
                return DateTime.MinValue;
            }
        }

        private async Task RefreshAccessToken()
        {
            await _lock.Run(async () =>
            {
                try
                {
                    if (CheckAccessTokenState() == AccessTokenState.Valid)
                        return;

                    await _authService.RefreshAccessTokenWithRetry();
                    AccessToken = await _credentialsStorage.GetAccessToken();
                }
                catch (InternetProblemsException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Error(GetType().Name, "Refreshing the access token failed", ex);
                }
            });
        }
    }
}
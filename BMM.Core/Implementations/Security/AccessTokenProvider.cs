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
        private const int TimeToRefreshTokenBeforeExpirationInHours = 3;
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

        public string AccessToken { get; private set; }

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

        public DateTime GetTokenExpirationDate() => _jwtTokenReader.GetExpirationTime(AccessToken);

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
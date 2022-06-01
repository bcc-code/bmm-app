using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.Messages;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Security
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly IOidcCredentialsStorage _credentialsStorage;
        private readonly IOidcAuthService _authService;
        private readonly ILogger _logger;
        private DateTime? _expirationDate;
        private string _accessToken;
        private MvxSubscriptionToken _loggedOutMessageToken;

        public AccessTokenProvider(IOidcCredentialsStorage credentialsStorage, IOidcAuthService authService, IMvxMessenger messenger, ILogger logger)
        {
            _credentialsStorage = credentialsStorage;
            _authService = authService;
            _logger = logger;
            _loggedOutMessageToken = messenger.Subscribe<LoggedOutMessage>(message =>
            {
                _expirationDate = null;
                _accessToken = null;
            });
        }

        public string AccessToken => _accessToken;

        public async Task<string> GetAccessToken()
        {
            await RefreshAccessTokenIfNeeded();

            if (string.IsNullOrEmpty(_accessToken))
                _accessToken = await _credentialsStorage.GetAccessToken();

            if (string.IsNullOrEmpty(_accessToken))
                _logger.Error(GetType().Name, "Access token is still null after refreshing from secure storage");

            return _accessToken;
        }
        
        public bool CheckIsTokenValid(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return false;

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenInfo = jwtSecurityTokenHandler.ReadToken(accessToken);
            return tokenInfo.ValidTo > DateTime.UtcNow;
        }

        public async Task<bool> IsAccessTokenValid()
        {
            if (_expirationDate == null)
                _expirationDate = await _credentialsStorage.GetAccessTokenExpirationDate();

            bool accessTokenNeedsRefresh = AccessTokenNeedsRefresh();
            return !accessTokenNeedsRefresh;
        }

        private async Task RefreshAccessTokenIfNeeded()
        {
            if (_expirationDate == null)
                _expirationDate = await _credentialsStorage.GetAccessTokenExpirationDate();

            if (!AccessTokenNeedsRefresh())
                return;

            try
            {
                await _lock.WaitAsync();
                _expirationDate = await _credentialsStorage.GetAccessTokenExpirationDate();

                // check again inside of the lock
                // to make sure refresh is not called several times
                if (!AccessTokenNeedsRefresh())
                    return;

                await _authService.RefreshAccessTokenWithRetry();
                _expirationDate = await _credentialsStorage.GetAccessTokenExpirationDate();
                _accessToken = await _credentialsStorage.GetAccessToken();
            }
            catch (InternetProblemsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(GetType().Name, "Refreshing the access token failed", ex);
                _accessToken = null;
                _expirationDate = null;
            }
            finally
            {
                _lock.Release();
            }
        }

        private bool AccessTokenNeedsRefresh()
        {
            if (!_expirationDate.HasValue)
            {
                _logger.Error(GetType().Name, "Access token expiration date is null");
                return true;
            }

            var expirationDateInUtc = _expirationDate.Value.ToUniversalTime();
            var currentUtc = DateTime.UtcNow;
            return expirationDateInUtc < currentUtc;
        }
    }
}
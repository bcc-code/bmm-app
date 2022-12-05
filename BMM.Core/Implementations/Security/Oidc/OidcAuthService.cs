using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Security.Oidc.Interfaces;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;

namespace BMM.Core.Implementations.Security.Oidc
{
    public class OidcAuthService : IOidcAuthService, IUserAuthChecker
    {
        private const int RefreshAccessTokenRetryCount = 3;
        private readonly TimeSpan _refreshAccessTokenRetryDelay = TimeSpan.FromSeconds(2);
        private readonly IOidcCredentialsStorage _credentialsStorage;
        private readonly IUserStorage _userStorage;
        private readonly IClaimUserInformationExtractor _claimUserInformationExtractor;
        private readonly ILogger _logger;
        private readonly OidcClient _client;
        private readonly IAnalytics _analytics;

        public OidcAuthService(
            IOidcCredentialsStorage credentialsStorage,
            IUserStorage userStorage,
            IBrowser browser,
            IClaimUserInformationExtractor claimUserInformationExtractor,
            ILogger logger, IAnalytics analytics)
        {
            _credentialsStorage = credentialsStorage;
            _userStorage = userStorage;
            _claimUserInformationExtractor = claimUserInformationExtractor;
            _logger = logger;
            _analytics = analytics;
            var options = new OidcClientOptions
            {
                Authority = OidcConstants.AuthorizationServerUrl,
                ClientId = OidcConstants.ClientId,
                Scope = OidcConstants.Scopes,
                RedirectUri = OidcConstants.LoginRedirectUrl,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                Policy = {RequireAccessTokenHash = false},
                Browser = browser,
            };

            _client = new OidcClient(options);
        }

        public async Task<User> PerformLogin()
        {
            try
            {
                await _credentialsStorage.FlushStorage();
                _analytics.LogEvent("LoginFlow - started");
                var result = await _client.LoginAsync(CreateLoginRequest());
                _analytics.LogEvent("LoginFlow - completed",
                    new Dictionary<string, object> {{"IsError", result.IsError}, {"Error", result.Error}});
                if (result.IsError)
                {
                    throw result.Error.Contains("UserCancel")
                        ? new UserCanceledOidcLoginException()
                        : new Exception(result.Error);
                }

                await _credentialsStorage.SetAccessToken(result.AccessToken);
                await _credentialsStorage.SetRefreshToken(result.RefreshToken);

                var user = _claimUserInformationExtractor.ExtractUser(result.User.Claims);
                return user;
            }
            catch (Exception exception)
            {
                _analytics.LogEvent("LoginFlow - Error", new Dictionary<string, object> {{"Message", exception.Message}});

                if (exception.Message.Contains("Error loading discovery document"))
                    throw new InternetProblemsException(exception);
                
                throw;
            }
        }

        public async Task PerformLogout()
        {
            await _credentialsStorage.FlushStorage();
            await _userStorage.RemoveUser();
        }

        public async Task RefreshAccessTokenWithRetry()
        {
            int currentRetry = 0;

            while (true)
            {
                try
                {
                    await RefreshAccessToken();

                    break;
                }
                catch (Exception ex)
                {
                    currentRetry++;

                    if (currentRetry > RefreshAccessTokenRetryCount || !(ex is InternetProblemsException))
                        throw;
                }

                await Task.Delay(_refreshAccessTokenRetryDelay);
            }
        }

        public async Task RefreshAccessToken()
        {
            var refreshToken = await _credentialsStorage.GetRefreshToken();

            // todo how is this happening???
            if (refreshToken == null)
                throw new Exception("refresh_token from secure storage is null");

            try
            {
                var result = await _client.RefreshTokenAsync(refreshToken);
                if (result.IsError)
                    throw new Exception(result.Error);

                await _credentialsStorage.SetAccessToken(result.AccessToken);
            }
            catch (Exception exception) when (IsInternetException(exception))
            {
                throw new InternetProblemsException(exception);
            }
        }

        public async Task<bool> IsAuthenticated()
        {
            var hasAccessToken = await _credentialsStorage.GetAccessToken() != null;
            var hasRefreshToken = await _credentialsStorage.GetRefreshToken() != null;
            var hasUser = _userStorage.GetUser() != null;

            if (hasUser && hasAccessToken && !hasRefreshToken)
                _logger.Error(GetType().Name, "User and access_token are available but no refresh_token");
            
            return hasUser && hasAccessToken && hasRefreshToken;
        }

        public Task<bool> IsUserAuthenticated()
        {
            return IsAuthenticated();
        }

        private LoginRequest CreateLoginRequest()
        {
            return new LoginRequest
            {
                FrontChannelExtraParameters =
                {
                    {"audience", OidcConstants.Audience}, {"prompt", OidcConstants.PromptParameter}
                }
            };
        }

        private bool IsInternetException(Exception exception)
        {
            return exception is TaskCanceledException // request times out
                   || exception.Message.Contains("A task was canceled") // since we wrap the exception from a string
                   || exception.Message.Contains("The operation was canceled.") // since we wrap the exception from a string
                   || exception.Message.Contains("Error loading discovery document")
                   || exception.Message.Contains("A data connection is not currently allowed")
                   || exception.Message.Contains("The network connection was lost")
                   || exception.Message.Contains("The Internet connection appears to be offline")
                   || exception.Message.Contains("The network connection was lost")
                   || exception.Message.Contains("Network is unreachable")
                   || exception.Message.Contains("The request timed out")
                   || exception.Message.Contains("An error occurred while sending the request");
        }
    }
}
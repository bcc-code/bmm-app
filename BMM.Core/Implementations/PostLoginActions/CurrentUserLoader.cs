using System;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Security;

namespace BMM.Core.Implementations.PostLoginActions
{
    public class CurrentUserLoader : ICurrentUserLoader
    {
        private const int RetryThresholdInSeconds = 10;
        private const int LoadingAttemptsBeforeCritical = 3;
        private readonly IUserStorage _userStorage;
        private readonly IUsersClient _usersClient;
        private readonly ILogger _logger;
        private readonly TimeSpan _retryTimout = new TimeSpan(0, 2, 0);

        public CurrentUserLoader(IUserStorage userStorage, IUsersClient usersClient, ILogger logger)
        {
            _userStorage = userStorage;
            _usersClient = usersClient;
            _logger = logger;
        }

        public async Task TryToLoadUserFromApi(User user)
        {
            var userFromApi = await _usersClient.GetCurrentLoggedInUser();
            await MergeUsersAndSaveToLocalStorage(user, userFromApi);
        }

        public async Task CreateUserInApiAndTryToLoadIt(User user)
        {
            await _usersClient.CreateCurrentUser();
            var timeout = DateTime.Now.Add(_retryTimout);
            var lastTryDateTime = DateTime.Now;
            var loadingAttempts = 0;

            var userExists = false;

            while (!userExists && !CurrentDateTimeExceedsTimout(timeout))
            {
                if (lastTryDateTime.AddSeconds(RetryThresholdInSeconds) > DateTime.Now)
                    continue;

                try
                {
                    lastTryDateTime = DateTime.Now;
                    loadingAttempts += 1;
                    if (loadingAttempts > LoadingAttemptsBeforeCritical)
                    {
                        var secondsWaitedWhileLoadingUser = loadingAttempts * RetryThresholdInSeconds;
                        _logger.Warn(GetType().ToString(), $"Tried {loadingAttempts} times in {secondsWaitedWhileLoadingUser} seconds to get the current user without success");
                    }

                    await TryToLoadUserFromApi(user);
                    userExists = true;
                }

                // user is still not created
                catch (UserDoesNotExistInApiException)
                {
                    userExists = false;
                    await _usersClient.CreateCurrentUser();
                }
            }

            if (!userExists)
                throw new CouldNotCreateUserException("Could not create and get user after defined timeout");
        }

        private bool CurrentDateTimeExceedsTimout(DateTime timeout)
        {
            return DateTime.Now > timeout;
        }

        private async Task MergeUsersAndSaveToLocalStorage(User user, User userFromApi)
        {
            user.Roles = userFromApi.Roles;
            user.Username = userFromApi.Username;
            _userStorage.StoreUser(user);
        }
    }
}
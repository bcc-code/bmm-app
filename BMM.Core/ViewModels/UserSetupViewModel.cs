using System;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.PostLoginActions;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.Implementations.Security.Oidc.Interfaces;
using BMM.Core.Messages;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public struct UserSetupViewModelParameters
    {
        public User UserToCreate { get; set; }
    }

    public class UserSetupViewModel : BaseViewModel<UserSetupViewModelParameters>
    {
        private readonly ICurrentUserLoader _currentUserLoader;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IAppNavigator _appNavigator;
        private readonly IOidcAuthService _oidcAuthService;

        public UserSetupViewModel(ICurrentUserLoader currentUserLoader, IExceptionHandler exceptionHandler, IAppNavigator appNavigator, IOidcAuthService oidcAuthService)
        {
            _currentUserLoader = currentUserLoader;
            _exceptionHandler = exceptionHandler;
            _appNavigator = appNavigator;
            _oidcAuthService = oidcAuthService;
            IsLoading = true;
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            _exceptionHandler.HandleException(GetUserFromApiAndSave());
        }

        private async Task GetUserFromApiAndSave()
        {
            try
            {
                if (NavigationParameter.UserToCreate == null)
                    throw new Exception($"No user or null was passed to {typeof(UserSetupViewModel).Name} pass a valid user inside of {typeof(UserSetupViewModelParameters).Name}");

                await _currentUserLoader.CreateUserInApiAndTryToLoadIt(NavigationParameter.UserToCreate);
                Messenger.Publish(new LoggedInOnlineMessage(this));
                await _appNavigator.NavigateAfterLoggedIn();
            }
            catch (Exception)
            {
                await _appNavigator.NavigateToLogin(false);
                await _oidcAuthService.PerformLogout();
                throw;
            }
        }
    }
}
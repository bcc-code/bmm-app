using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.PostLoginActions;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.Implementations.Security.Oidc.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;
using MvvmCross.Localization;

namespace BMM.Core.ViewModels
{
    public struct OidcLoginParameters
    {
        public bool IsInitialLogin { get; set; }
    }

    public class OidcLoginViewModel : BaseViewModel<OidcLoginParameters>
    {
        private readonly IConnection _connection;
        private readonly IDeviceInfo _deviceInfo;
        private readonly IOidcAuthService _oidcAuthService;
        private readonly IUserDialogs _userDialogs;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ICurrentUserLoader _currentUserLoader;
        private readonly IAppNavigator _appNavigator;
        private readonly IAccessTokenProvider _accessTokenProvider;

        public OidcLoginViewModel(
            IDeviceInfo deviceInfo,
            IOidcAuthService oidcAuthService,
            IConnection connection,
            IUserDialogs userDialogs,
            IExceptionHandler exceptionHandler,
            ICurrentUserLoader currentUserLoader,
            IAppNavigator appNavigator,
            IAccessTokenProvider accessTokenProvider)
        {
            _deviceInfo = deviceInfo;
            _oidcAuthService = oidcAuthService;
            _connection = connection;
            _userDialogs = userDialogs;
            _exceptionHandler = exceptionHandler;
            _currentUserLoader = currentUserLoader;
            _appNavigator = appNavigator;
            _accessTokenProvider = accessTokenProvider;
            LoginCommand = new ExceptionHandlingCommand(
                async () => await StartLoginFlow()
            );
        }

        public IMvxAsyncCommand LoginCommand { get; }

        public bool IsInitialLogin { get; set; }

        public override async Task Initialize()
        {
            await base.Initialize();
            
            if (NavigationParameter.IsInitialLogin)
            {
                IsLoading = true;
                IsInitialLogin = true;
            }
        }

        private async Task StartLoginFlow()
        {
            IsLoading = true;

            // Ensure the API is online when sending the login request
            if (_connection.GetStatus() != ConnectionStatus.Online)
            {
                await _userDialogs.AlertAsync(TextSource[Translations.LoginViewModel_LoginNoConnectionMessage], TextSource[Translations.LoginViewModel_LoginFailureTitle]);
                IsLoading = false;
                return;
            }

            User user = null;
            try
            {
                user = await _oidcAuthService.PerformLogin();
                await _accessTokenProvider.Initialize();
                await _currentUserLoader.TryToLoadUserFromApi(user);
                // The corresponding activity has a launch mode of SingleTask
                // and therefore never gets destroyed. Close it manually
                if (_deviceInfo.IsAndroid)
                    await NavigationService.Close(this);
                await _appNavigator.NavigateAfterLoggedIn();
            }
            catch (UserCanceledOidcLoginException)
            {
                IsLoading = false;
            }
            catch (UserDoesNotExistInApiException)
            {
                await NavigationService.Navigate<UserSetupViewModel, UserSetupViewModelParameters>(new UserSetupViewModelParameters { UserToCreate = user});
            }
            catch (Exception exception)
            {
                IsLoading = false;

                _exceptionHandler.HandleException(exception);
            }
        }
    }
}
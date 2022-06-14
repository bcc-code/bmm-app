using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.Implementations.UI;
using BMM.Core.Translation;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Localization;

namespace BMM.Core.Implementations.Exceptions
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly IToastDisplayer _toastDisplayer;
        private readonly ILogger _logger;
        private readonly IAnalytics _analytics;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public ExceptionHandler(
            IToastDisplayer toastDisplayer,
            ILogger logger,
            IAnalytics analytics,
            IBMMLanguageBinder bmmLanguageBinder)
        {
            _toastDisplayer = toastDisplayer;
            _logger = logger;
            _analytics = analytics;
            _bmmLanguageBinder = bmmLanguageBinder;
        }

        public void FireAndForget(Func<Task> action)
        {
            Task.Run(async () =>
            {
                try
                {
                    await action.Invoke();
                }
                catch (Exception e)
                {
                    HandleException(e);
                }
            });
        }

        public void FireAndForgetOnMainThread(Func<Task> action)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>().ExecuteOnMainThreadAsync(action);
                }
                catch (Exception e)
                {
                    HandleException(e);
                }
            });
        }

        public void FireAndForgetWithoutUserMessages(Func<Task> action)
        {
            Task.Run(async () =>
            {
                try
                {
                    await action.Invoke();
                }
                catch (Exception e)
                {
                    HandleExceptionWithoutUserMessages(e);
                }
            });
        }

        public void HandleException(Task task)
        {
            task.ContinueWith((t) =>
            {
                if (t.IsFaulted)
                {
                    HandleException(t.Exception);
                }
            });
        }

        public void HandleExceptionWithoutUserMessages(Exception ex)
        {
            HandleException(ex, null);
        }

        public void HandleException(Exception ex)
        {
            HandleException(ex, _toastDisplayer);
        }

        private void HandleException(Exception ex, IToastDisplayer toastDisplayer)
        {
            if (ex is InternetProblemsException)
            {
                _logger.Debug("Internet issues", ex.Message);
                toastDisplayer?.Error(_bmmLanguageBinder[Translations.Global_InternetConnectionOffline]);
            }
            else if (ex is WebException)
            {
                _logger.Debug("Request failed", ex.Message);
                toastDisplayer?.Error(_bmmLanguageBinder.GetText(Translations.RequestExceptionHandler_RequestFailedMessage, ex.Message));
            }
            else if (ex is UnauthorizedException unauthorizedException)
            {
                GoToLogin(unauthorizedException);
            }
            else
            {
                _logger.Error("Unexpected Error", ex.Message, ex, toastDisplayer != null);
                toastDisplayer?.Error(_bmmLanguageBinder[Translations.Global_UnexpectedError]);
            }
        }

        private void GoToLogin(UnauthorizedException ex)
        {
            var appNavigator = Mvx.IoCProvider.Resolve<IAppNavigator>();
            appNavigator.NavigateToLogin(false);
            FireAndForgetWithoutUserMessages(async () =>
            {
                var expirationDate = Mvx.IoCProvider.Resolve<IAccessTokenProvider>().GetTokenExpirationDate();
                var accessTokenExpiryDateString = expirationDate.ToString("MM/dd/yyyy HH:mm:ss");
                var currentDate = DateTime.UtcNow;
                var currentDateString = currentDate.ToString("MM/dd/yyyy HH:mm:ss");
                var tokenExpired = expirationDate < currentDate;
                var parameters = new Dictionary<string, object>
                {
                    {"RequestUrl", ex.Request.RequestUri},
                    {"RequestMethod", ex.Request.Method},
                    {"HasAccessToken", !string.IsNullOrEmpty(ex.Request.Headers.Authorization.Parameter)},
                    {"RequestDetails", ex.ToString()},
                    {"AccessTokenExpirationDate", accessTokenExpiryDateString},
                    {"CurrentDate", currentDateString},
                    {"AccessTokenIsExpired", tokenExpired}
                };
                _analytics.LogEvent("Login screen shown because unauthorized request", parameters);
            });
        }
    }
}
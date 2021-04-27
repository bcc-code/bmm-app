using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.Implementations.UI;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Localization;

namespace BMM.Core.Implementations.Exceptions
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly IToastDisplayer _toastDisplayer;
        private readonly IMvxLanguageBinder _textSource;
        private readonly IMvxLanguageBinder _globalTextSource;
        private readonly ILogger _logger;
        private readonly IAnalytics _analytics;

        public ExceptionHandler(IToastDisplayer toastDisplayer, ILogger logger, IAnalytics analytics)
        {
            _toastDisplayer = toastDisplayer;
            _logger = logger;
            _analytics = analytics;
            _textSource = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "RequestExceptionHandler");
            _globalTextSource = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "Global");
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
                toastDisplayer?.Error(_globalTextSource.GetText("InternetConnectionOffline"));
            }
            else if (ex is WebException)
            {
                _logger.Debug("Request failed", ex.Message);
                toastDisplayer?.Error(_textSource.GetText("RequestFailedMessage", ex.Message));
            }
            else if (ex is UnauthorizedException unauthorizedException)
            {
                GoToLogin(unauthorizedException);
            }
            else
            {
                _logger.Error("Unexpected Error", ex.Message, ex);
                toastDisplayer?.Error(_globalTextSource.GetText("UnexpectedError"));
            }
        }

        private void GoToLogin(UnauthorizedException ex)
        {
            var appNavigator = Mvx.IoCProvider.Resolve<IAppNavigator>();
            appNavigator.NavigateToLogin(false);
            FireAndForgetWithoutUserMessages(async () =>
            {
                var expirationDate = await Mvx.IoCProvider.Resolve<IOidcCredentialsStorage>().GetAccessTokenExpirationDate();
                var accessTokenExpiryDateString = expirationDate.HasValue ? expirationDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : "";
                var currentDate = DateTime.Now;
                var currentDateString = currentDate.ToString("MM/dd/yyyy HH:mm:ss");
                var tokenExpired = !expirationDate.HasValue || expirationDate.Value < currentDate;
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
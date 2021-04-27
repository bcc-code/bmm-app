using System.Threading;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Messages;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading
{
    public class BackgroundLogger
    {
        private readonly IMvxMessenger _messenger;
        private readonly IAppContentLogger _appContentLogger;
        private readonly IExceptionHandler _exceptionHandler;

        private readonly MvxSubscriptionToken _loggedInToken;
        private readonly MvxSubscriptionToken _logoutToken;

        private Timer _timer;

        public BackgroundLogger(IMvxMessenger messenger, IAppContentLogger appContentLogger, IExceptionHandler exceptionHandler)
        {
            _messenger = messenger;
            _appContentLogger = appContentLogger;
            _exceptionHandler = exceptionHandler;

            _loggedInToken = _messenger.Subscribe<LoggedInMessage>(UserLoggedIn);
            _logoutToken = _messenger.Subscribe<LoggedOutMessage>(UserLoggedOut);
        }

        public void UserLoggedIn(LoggedInMessage message)
        {
            const int fiveMinutesInMilliseconds = 1000 * 60 * 5;
            _timer = new Timer(LogAppContent, null, fiveMinutesInMilliseconds, fiveMinutesInMilliseconds);
        }

        private void LogAppContent(object state)
        {
            _exceptionHandler.FireAndForgetWithoutUserMessages(() => _appContentLogger.LogAppContent("Log app content every 5 minutes"));
        }

        private void UserLoggedOut(LoggedOutMessage message)
        {
            _timer?.Dispose();
            _timer = null;
        }
    }
}
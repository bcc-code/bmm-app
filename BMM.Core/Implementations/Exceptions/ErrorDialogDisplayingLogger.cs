using System;
using Acr.UserDialogs;
using BMM.Api.Framework;
using MvvmCross.Base;

namespace BMM.Core.Implementations.Exceptions
{
    public class ErrorDialogDisplayingLogger: ILogger
    {
        private readonly IUserDialogs _userDialogs;
        private readonly ILogger _logger;
        private readonly IMvxMainThreadAsyncDispatcher _mainThreadAsyncDispatcher;

        public ErrorDialogDisplayingLogger(IUserDialogs userDialogs, ILogger logger, IMvxMainThreadAsyncDispatcher mainThreadAsyncDispatcher)
        {
            _userDialogs = userDialogs;
            _logger = logger;
            _mainThreadAsyncDispatcher = mainThreadAsyncDispatcher;
        }

        public void Debug(string tag, string message)
        {
            _logger.Debug(tag, message);
        }

        public void Info(string tag, string message)
        {
            _logger.Info(tag, message);
        }

        public void Warn(string tag, string message)
        {
            _logger.Warn(tag, message);
        }

        public void Error(string tag, string message)
        {
            _logger.Error(tag, message);
            _mainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() => _userDialogs.AlertAsync(message));
        }

        public void Error(string tag, string message, Exception exception, bool presentedToUser)
        {
            _logger.Error(tag, message, exception, presentedToUser);
            _mainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() => _userDialogs.AlertAsync(exception.Message));
        }
    }
}
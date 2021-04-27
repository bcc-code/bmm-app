using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Core.Implementations.Exceptions;

namespace BMM.Core.Implementations.UI
{
    public class ExceptionHandlingUserDialogs : IUserDialogs
    {
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IUserDialogs _userDialogs;

        public ExceptionHandlingUserDialogs(IExceptionHandler exceptionHandler)
        {
            _userDialogs = UserDialogs.Instance;
            _exceptionHandler = exceptionHandler;
        }

        public IDisposable Alert(string message, string title = null, string okText = null)
        {
            try
            {
                return _userDialogs.Alert(message, title, okText);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Disposable.Empty;
            }
        }

        public IDisposable Alert(AlertConfig config)
        {
            try
            {
                return _userDialogs.Alert(config);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Disposable.Empty;
            }
        }

        public Task AlertAsync(string message, string title = null, string okText = null, CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.AlertAsync(message, title, okText, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(0);
            }
        }

        public Task AlertAsync(AlertConfig config, CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.AlertAsync(config, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(0);
            }
        }

        public IDisposable ActionSheet(ActionSheetConfig config)
        {
            try
            {
                return _userDialogs.ActionSheet(config);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Disposable.Empty;
            }
        }

        public Task<string> ActionSheetAsync(string title, string cancel, string destructive, CancellationToken? cancelToken = null,
            params string[] buttons)
        {
            try
            {
                return _userDialogs.ActionSheetAsync(title, cancel, destructive, cancelToken, buttons);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(default(string));
            }
        }

        public IDisposable Confirm(ConfirmConfig config)
        {
            try
            {
                return _userDialogs.Confirm(config);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Disposable.Empty;
            }
        }

        public Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null,
            CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.ConfirmAsync(message, title, okText, cancelText, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> ConfirmAsync(ConfirmConfig config, CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.ConfirmAsync(config, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(false);
            }
        }

        public IDisposable DatePrompt(DatePromptConfig config)
        {
            try
            {
                return _userDialogs.DatePrompt(config);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Disposable.Empty;
            }
        }

        public Task<DatePromptResult> DatePromptAsync(DatePromptConfig config, CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.DatePromptAsync(config, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(default(DatePromptResult));
            }
        }

        public Task<DatePromptResult> DatePromptAsync(string title = null, DateTime? selectedDate = null, CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.DatePromptAsync(title, selectedDate, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(default(DatePromptResult));
            }
        }

        public IDisposable TimePrompt(TimePromptConfig config)
        {
            try
            {
                return _userDialogs.TimePrompt(config);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Disposable.Empty;
            }
        }

        public Task<TimePromptResult> TimePromptAsync(TimePromptConfig config, CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.TimePromptAsync(config, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(default(TimePromptResult));
            }
        }

        public Task<TimePromptResult> TimePromptAsync(string title = null, TimeSpan? selectedTime = null, CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.TimePromptAsync(title, selectedTime, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(default(TimePromptResult));
            }
        }

        public IDisposable Prompt(PromptConfig config)
        {
            try
            {
                return _userDialogs.Prompt(config);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Disposable.Empty;
            }
        }

        public Task<PromptResult> PromptAsync(string message, string title = null, string okText = null, string cancelText = null,
            string placeholder = "", InputType inputType = InputType.Default, CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.PromptAsync(message, title, okText, cancelText, placeholder, inputType, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(default(PromptResult));
            }
        }

        public Task<PromptResult> PromptAsync(PromptConfig config, CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.PromptAsync(config, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(default(PromptResult));
            }
        }

        public IDisposable Login(LoginConfig config)
        {
            try
            {
                return _userDialogs.Login(config);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Disposable.Empty;
            }
        }

        public Task<LoginResult> LoginAsync(string title = null, string message = null, CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.LoginAsync(title, message, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(default(LoginResult));
            }
        }

        public Task<LoginResult> LoginAsync(LoginConfig config, CancellationToken? cancelToken = null)
        {
            try
            {
                return _userDialogs.LoginAsync(config, cancelToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Task.FromResult(default(LoginResult));
            }
        }

        public IProgressDialog Progress(ProgressDialogConfig config)
        {
            try
            {
                return _userDialogs.Progress(config);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return default(IProgressDialog);
            }
        }

        public IProgressDialog Loading(string title = null, Action onCancel = null, string cancelText = null, bool show = true,
            MaskType? maskType = null)
        {
            try
            {
                return _userDialogs.Loading(title, onCancel, cancelText, show, maskType);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return default(IProgressDialog);
            }
        }

        public IProgressDialog Progress(string title = null, Action onCancel = null, string cancelText = null, bool show = true,
            MaskType? maskType = null)
        {
            try
            {
                return _userDialogs.Progress(title, onCancel, cancelText, show, maskType);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return default(IProgressDialog);
            }
        }

        public void ShowLoading(string title = null, MaskType? maskType = null)
        {
            try
            {
                _userDialogs.ShowLoading(title, maskType);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
            }
        }

        public void HideLoading()
        {
            try
            {
                _userDialogs.HideLoading();
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
            }
        }

        public IDisposable Toast(string title, TimeSpan? dismissTimer = null)
        {
            try
            {
                return _userDialogs.Toast(title, dismissTimer);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Disposable.Empty;
            }
        }

        public IDisposable Toast(ToastConfig cfg)
        {
            try
            {
                return _userDialogs.Toast(cfg);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
                return Disposable.Empty;
            }
        }
    }
}
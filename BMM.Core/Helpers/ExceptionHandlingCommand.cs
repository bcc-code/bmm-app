using System;
using System.Threading.Tasks;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Commands;

namespace BMM.Core.Helpers
{
    public class ExceptionHandlingCommand<T> : MvxAsyncCommand<T>
    {
        private readonly bool _allowConcurrentExecutions;

        public ExceptionHandlingCommand(Func<T, Task> execute, Func<T, bool> canExecute = null, bool allowConcurrentExecutions = false) :
            base(HandleException(execute), canExecute, allowConcurrentExecutions)
        {
            _allowConcurrentExecutions = allowConcurrentExecutions;
        }

        protected override bool CanExecuteImpl(object parameter)
        {
            var result = base.CanExecuteImpl(parameter);

            if (result == false && IsRunning)
            {
                ExceptionHandlingCommand.LogIfIsRunning(this, _allowConcurrentExecutions);
            }

            return result;
        }

        private static Func<T, Task> HandleException(Func<T, Task> execute)
        {
            return async (T parameter) =>
            {
                try
                {
                    await execute.Invoke(parameter);
                }
                catch (Exception ex)
                {
                    var exceptionHandler = Mvx.IoCProvider.Resolve<IExceptionHandler>();
                    exceptionHandler.HandleException(ex);
                }
            };
        }
    }

    public class ExceptionHandlingCommand : MvxAsyncCommand
    {
        private readonly bool _allowConcurrentExecutions;

        public ExceptionHandlingCommand(Func<Task> execute, Func<bool> canExecute = null, bool allowConcurrentExecutions = false) :
            base(HandleException(execute), canExecute, allowConcurrentExecutions)
        {
            _allowConcurrentExecutions = allowConcurrentExecutions;
        }

        protected override bool CanExecuteImpl(object parameter)
        {
            var result = base.CanExecuteImpl(parameter);

            if (result == false && IsRunning)
            {
                LogIfIsRunning(this, _allowConcurrentExecutions);
            }

            return result;
        }

        private static Func<Task> HandleException(Func<Task> execute)
        {
            return async () =>
            {
                try
                {
                    await execute.Invoke();
                }
                catch (Exception ex)
                {
                    var exceptionHandler = Mvx.IoCProvider.Resolve<IExceptionHandler>();
                    exceptionHandler.HandleException(ex);
                }
            };
        }

        /// <summary>
        /// Logs an event if we don't run the command since it's already running. We do this to learn how often it happens and if it is a problem.
        /// In case it happens a lot when navigating to a ViewModel we need to introduce CancellationToken that are passed to all async actions during <see cref="DocumentsViewModel.Initialization"/>
        /// and cancel that token when destroying the view.
        /// Another alternative is to make the <see cref="DocumentsViewModel.Initialization"/> leaner. That means to not wait for the result of async methods before finishing the initialization.
        /// </summary>
        public static void LogIfIsRunning(MvxAsyncCommandBase command, bool allowConcurrentExecutions)
        {
            if (!allowConcurrentExecutions && command.IsRunning)
            {
                var analytics = Mvx.IoCProvider.Resolve<IAnalytics>();
                analytics.LogEvent("ignored a command since it's already running");
            }
        }
    }
}
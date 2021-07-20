using System;
using System.Collections.Generic;
using System.Diagnostics;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Abstractions.Delegates;
using BMM.Core.GuardedActions.Abstractions.Interfaces;
using MvvmCross.IoC;

namespace BMM.Core.GuardedActions.Abstractions
{
    public abstract class BaseGuardedAction : IBaseGuardedAction
    {
        private readonly Dictionary<string, object> _executionContext = new();

        [MvxInject]
        public IGuardInvoker Invoker { get; set; }

        public void Attach<T>(string key, T value)
        {
            _executionContext[key] = value;

            if (key == ExecutionContextKeys.DataContext)
                OnDataContextAttached(value);
            else
                OnAttached(key, value);
        }

        public T Get<T>(string key)
        {
            if (!_executionContext.ContainsKey(key))
            {
                Debug.WriteLine($"{key} not found!");
                return default;
            }

            if (_executionContext[key] is T value)
                return value;

            return default;
        }

        protected virtual void OnAttached<T>(string key, T value) { }
        protected virtual void OnDataContextAttached<T>(T value) { }
        protected virtual IEnumerable<HandleException> GetExceptionHandlers() => new List<HandleException>();

        protected IEnumerable<HandleException> ExceptionHandlers()
        {
            foreach (var exceptionHandler in GetExceptionHandlers())
                yield return exceptionHandler;

            yield return DefaultExceptionHandler;
        }

        private bool DefaultExceptionHandler(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception.Message);
            return true;
        }
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using BMM.Core.Constants;
using BMM.Core.ExceptionHandlers.Interfaces;
using BMM.Core.ExceptionHandlers.Interfaces.Base;
using BMM.Core.GuardedActions.Base.Interfaces;
using MvvmCross.IoC;

namespace BMM.Core.GuardedActions.Base
{
    public abstract class BaseGuardedAction : IBaseGuardedAction
    {
        private readonly Dictionary<string, object> _executionContext = new Dictionary<string, object>();

        [MvxInject]
        public IGuardInvoker Invoker { get; set; }

        [MvxInject]
        public IGenericActionExceptionHandler GenericExceptionHandler { get; set; }

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
        protected virtual IEnumerable<IActionExceptionHandler> GetExceptionHandlers() => new List<IActionExceptionHandler>();

        protected IEnumerable<IActionExceptionHandler> ExceptionHandlers()
        {
            foreach (var exceptionHandler in GetExceptionHandlers())
                yield return exceptionHandler;

            yield return GenericExceptionHandler;
        }
    }
}
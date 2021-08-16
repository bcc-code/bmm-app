using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Core.ExceptionHandlers.Interfaces.Base;

namespace BMM.Core.GuardedActions.Base.Interfaces
{
    public interface IGuardInvoker
    {
        Task Invoke(
            Func<Task> task,
            Func<Exception, Task> onException,
            Func<Task> onFinally,
            IEnumerable<IActionExceptionHandler> exceptionHandlers);

        Task<TResult> Invoke<TResult>(
            Func<Task<TResult>> task,
            Func<Exception, Task> onException,
            Func<Task> onFinally,
            IEnumerable<IActionExceptionHandler> exceptionHandlers);
    }
}
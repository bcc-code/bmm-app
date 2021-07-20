using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Core.GuardedActions.Abstractions.Delegates;

namespace BMM.Core.GuardedActions.Abstractions.Interfaces
{
    public interface IGuardInvoker
    {
        Task Invoke(
            Func<Task> task,
            Func<Exception, Task> onException,
            Func<Task> onFinally,
            IEnumerable<HandleException> exceptionHandlers);

        Task<TResult> Invoke<TResult>(
            Func<Task<TResult>> task,
            Func<Exception, Task> onException,
            Func<Task> onFinally,
            IEnumerable<HandleException> exceptionHandlers);
    }
}
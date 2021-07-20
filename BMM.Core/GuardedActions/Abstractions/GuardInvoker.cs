using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.GuardedActions.Abstractions.Delegates;
using BMM.Core.GuardedActions.Abstractions.Interfaces;

namespace BMM.Core.GuardedActions.Abstractions
{
    public class GuardInvoker : IGuardInvoker
    {
        public async Task Invoke(
            Func<Task> task,
            Func<Exception, Task> onException,
            Func<Task> onFinally,
            IEnumerable<HandleException> exceptionHandlers)
        {
            await Task.Run(
                async () =>
                {
                    try
                    {
                        if (task != null)
                            await task().ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        await HandleException(onException, exceptionHandlers, e).ConfigureAwait(false);
                    }
                    finally
                    {
                        if (onFinally != null)
                            await onFinally().ConfigureAwait(false);
                    }
                })
                .ConfigureAwait(false);
        }

        public async Task<TResult> Invoke<TResult>(
            Func<Task<TResult>> task,
            Func<Exception, Task> onException,
            Func<Task> onFinally,
            IEnumerable<HandleException> exceptionHandlers)
        {
            return await Task.Run(
                async () =>
                {
                    try
                    {
                        if (task != null)
                            return await task().ConfigureAwait(false);

                        return default;
                    }
                    catch (Exception e)
                    {
                        await HandleException(onException, exceptionHandlers, e).ConfigureAwait(false);
                        return default;
                    }
                    finally
                    {
                        if (onFinally != null)
                            await onFinally().ConfigureAwait(false);
                    }
                });
        }

        private static async Task HandleException(
            Func<Exception, Task> onException,
            IEnumerable<HandleException> exceptionHandlers,
            Exception e)
        {
            var handlers = exceptionHandlers?.ToList() ?? new List<HandleException>();
            foreach (var handleException in handlers)
            {
                if (handleException(e))
                    break;
            }

            if (onException != null)
                await onException(e).ConfigureAwait(false);
        }
    }
}
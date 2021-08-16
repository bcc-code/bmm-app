using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.ExceptionHandlers.Interfaces;
using BMM.Core.ExceptionHandlers.Interfaces.Base;
using BMM.Core.GuardedActions.Base.Interfaces;

namespace BMM.Core.GuardedActions.Base
{
    public class GuardInvoker : IGuardInvoker
    {
        public async Task Invoke(
            Func<Task> task,
            Func<Exception, Task> onException,
            Func<Task> onFinally,
            IEnumerable<IActionExceptionHandler> exceptionHandlers)
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
            IEnumerable<IActionExceptionHandler> exceptionHandlers)
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
            IEnumerable<IActionExceptionHandler> exceptionHandlers,
            Exception e)
        {
            var handlers = exceptionHandlers?.ToList() ?? new List<IActionExceptionHandler>();

            bool handled = false;
            var genericHandler = handlers.First(h => h.GetType().GetInterfaces().Contains(typeof(IGenericActionExceptionHandler)));

            foreach (var handler in handlers.Where(h => h != genericHandler))
            {
                if (!handler.GetTriggeringExceptionTypes().Contains(e.GetType()))
                    continue;

                await handler.HandleException(e);
                handled = true;
                break;
            }

            if (!handled)
                await genericHandler.HandleException(e);

            if (onException != null)
                await onException(e);
        }
    }
}
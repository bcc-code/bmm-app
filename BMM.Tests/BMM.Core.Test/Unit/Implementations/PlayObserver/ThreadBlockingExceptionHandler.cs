using System;
using System.Threading.Tasks;
using BMM.Core.Implementations.Exceptions;

namespace BMM.Core.Test.Unit.Implementations.PlayObserver
{
    public class ThreadBlockingExceptionHandler: IExceptionHandler
    {
        public void HandleException(Task task)
        {
            throw new NotImplementedException();
        }

        public void HandleException(Exception ex)
        {
            throw new NotImplementedException();
        }

        public void FireAndForget(Func<Task> action)
        {
            throw new NotImplementedException();
        }

        public void FireAndForgetOnMainThread(Func<Task> action)
        {
            throw new NotImplementedException();
        }

        public void FireAndForgetWithoutUserMessages(Func<Task> action)
        {
            action().GetAwaiter().GetResult();
        }

        public void HandleExceptionWithoutUserMessages(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
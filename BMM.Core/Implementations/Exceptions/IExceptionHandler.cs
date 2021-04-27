using System;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Exceptions
{
    public interface IExceptionHandler
    {
        void HandleException(Task task);

        void HandleException(Exception ex);

        void FireAndForget(Func<Task> action);

        void FireAndForgetOnMainThread(Func<Task> action);

        void FireAndForgetWithoutUserMessages(Func<Task> action);

        void HandleExceptionWithoutUserMessages(Exception ex);
    }
}
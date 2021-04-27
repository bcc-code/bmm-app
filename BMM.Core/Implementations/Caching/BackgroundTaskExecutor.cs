using BMM.Core.Implementations.Exceptions;
using BMM.Core.Messages;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Caching
{
    public class BackgroundTaskExecutor
    {
        private readonly IExceptionHandler _exceptionHandler;
        private MvxSubscriptionToken _token;

        public BackgroundTaskExecutor(IMvxMessenger messenger, IExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            _token = messenger.Subscribe<BackgroundTaskMessage>(Execute);
        }

        private void Execute(BackgroundTaskMessage message)
        {
            _exceptionHandler.FireAndForgetWithoutUserMessages(async () => { await message.BackgroundTask.Invoke(); });
        }
    }
}
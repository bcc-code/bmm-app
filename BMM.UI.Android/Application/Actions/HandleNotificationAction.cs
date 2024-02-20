using System.Threading.Tasks;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.UI.Droid.Application.Actions.Interfaces;
using BMM.UI.Droid.Application.Implementations.Notifications;

namespace BMM.UI.Droid.Application.Actions
{
    public class HandleNotificationAction
        : GuardedAction,
          IHandleNotificationAction
    {
        private readonly INotificationHandler _notificationHandler;
        private readonly IRememberedQueueService _rememberedQueueService;

        public HandleNotificationAction(
            INotificationHandler notificationHandler,
            IRememberedQueueService rememberedQueueService)
        {
            _notificationHandler = notificationHandler;
            _rememberedQueueService = rememberedQueueService;
        }
        
        protected override async Task Execute()
        {
            await Task.CompletedTask;
            
            if (SplashScreenActivity.UnhandledNotification == null)
                return;

            var intent = SplashScreenActivity.UnhandledNotification;
            SplashScreenActivity.UnhandledNotification = null;
            var notification = new AndroidIntentNotification(intent);

            _notificationHandler.UserClickedNotification(notification);

            if (_notificationHandler.WillNotificationStartPlayer(notification))
                _rememberedQueueService.SetPlayerHasPendingOperation();
        }
    }
}
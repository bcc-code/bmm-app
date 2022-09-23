using System;
using System.Collections.Generic;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Notifications.Data;
using BMM.Core.Implementations.UI;
using Foundation;
using MvvmCross;
using MvvmCross.Base;
using UIKit;

namespace BMM.UI.iOS
{
    public class NotificationDisplayer : INotificationDisplayer
    {
        private readonly IViewModelAwareViewPresenter _viewPresenter;
        private readonly IMvxMainThreadAsyncDispatcher _mainThreadDispatcher;
        private readonly IAnalytics _analytics;

        public NotificationDisplayer(IViewModelAwareViewPresenter viewPresenter, IMvxMainThreadAsyncDispatcher mainThreadDispatcher, IAnalytics analytics)
        {
            _viewPresenter = viewPresenter;
            _mainThreadDispatcher = mainThreadDispatcher;
            _analytics = analytics;
        }

        public void DisplayNotificationOrPopup(LocalNotification notification)
        {
            _mainThreadDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                if (ApplicationIsInBackground())
                {
                    _analytics.LogEvent("show local notification", new Dictionary<string, object> {{"title", notification.Title}});
                    DisplayBackgroundNotification(notification);
                }
                else
                {
                    _analytics.LogEvent("show foreground message", new Dictionary<string, object> {{"title", notification.Title}});
                    DisplayMessageWindow(notification);
                }
            });
        }

        private void DisplayMessageWindow(LocalNotification notification)
        {
            var message = UIAlertController.Create(notification.Title, notification.Message, UIAlertControllerStyle.Alert);
            message.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            var viewController = ((Presenter)_viewPresenter).CurrentRootViewController;
            viewController.PresentViewController(message, true, null);
        }

        private void DisplayBackgroundNotification(LocalNotification notification)
        {
            var localNotification = new UILocalNotification
            {
                AlertTitle = notification.Title,
                AlertBody = notification.Message,
                SoundName = UILocalNotification.DefaultSoundName
            };

            UIApplication.SharedApplication.PresentLocalNotificationNow(localNotification);
        }

        private bool ApplicationIsInBackground()
        {
            return UIApplication.SharedApplication.ApplicationState == UIApplicationState.Background;
        }
    }
}
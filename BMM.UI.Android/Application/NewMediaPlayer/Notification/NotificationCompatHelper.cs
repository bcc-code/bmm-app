using Android.Content;
using AndroidX.Core.App;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Notification
{
    public static class NotificationCompatHelper
    {
        public static void AddAction(this NotificationCompat.Builder builder, INotificationAction action, Context context)
        {
            builder.AddAction(action.Icon, action.Title, action.GetIntent(context));
        }
    }
}
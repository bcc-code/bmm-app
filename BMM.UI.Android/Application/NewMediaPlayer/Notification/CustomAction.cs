using Android.App;
using Android.Content;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Notification
{
    public class CustomAction : INotificationAction
    {
        private readonly string _action;

        public CustomAction(int icon, string title, string action)
        {
            Icon = icon;
            Title = title;
            _action = action;
        }

        public int Icon { get; }

        public string Title { get; }

        public PendingIntent GetIntent(Context context)
        {
            return PendingIntent.GetBroadcast(context, 100, new Intent(_action).SetPackage(context.PackageName), PendingIntentFlags.CancelCurrent);
        }
    }
}
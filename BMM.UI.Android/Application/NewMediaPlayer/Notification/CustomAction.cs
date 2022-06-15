using Android.App;
using Android.Content;
using BMM.UI.Droid.Utils;

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
            var intent = new Intent(_action);
            intent.SetPackage(context.PackageName);

            ComponentName componentName = new ComponentName(context, Java.Lang.Class.FromType(typeof(CustomActionBroadcastReceiver)));
            intent.SetComponent(componentName);

            return PendingIntent.GetBroadcast(context, 100, intent, PendingIntentsUtils.GetImmutable());
        }
    }
}
using Android.App;
using Android.Content;
using AndroidX.Media.Session;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Notification
{
    public class ExoPlayerAction : INotificationAction
    {
        private readonly long _action;

        public ExoPlayerAction(int icon, string title, long action)
        {
            _action = action;
            Icon = icon;
            Title = title;
        }

        public int Icon { get; }

        public string Title { get; }

        public PendingIntent GetIntent(Context context)
        {
            return MediaButtonReceiver.BuildMediaButtonPendingIntent(context, _action);
        }
    }
}
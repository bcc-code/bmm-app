using Android.App;
using Android.Content;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Notification
{
    public interface INotificationAction
    {
        int Icon { get; }

        string Title { get; }

        PendingIntent GetIntent(Context context);
    }
}
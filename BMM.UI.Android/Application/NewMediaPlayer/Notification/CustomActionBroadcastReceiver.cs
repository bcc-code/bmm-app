using Android.App;
using Android.Content;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Notification
{
    [BroadcastReceiver(Enabled = true, Exported = true), IntentFilter(new[] { NowPlayingNotificationBuilder.ActionSkipBackward, NowPlayingNotificationBuilder.ActionSkipForward })]
    public class CustomActionBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            SetupHelper.EnsureInitialized();

            var handler = Mvx.IoCProvider.Resolve<IMediaPlayer>();

            if (intent.Action == NowPlayingNotificationBuilder.ActionSkipForward)
                handler.JumpForward();
            else
                handler.JumpBackward();
        }
    }
}
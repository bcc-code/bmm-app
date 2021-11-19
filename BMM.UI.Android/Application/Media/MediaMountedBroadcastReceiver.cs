using Android.App;
using Android.Content;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross;

namespace BMM.UI.Droid.Application.Media
{
    [BroadcastReceiver(Enabled = true, Exported = true), IntentFilter(new[] { Intent.ActionMediaRemoved, Intent.ActionMediaMounted, Intent.ActionMediaUnmounted, Intent.ActionMediaBadRemoval }, DataScheme = "file")]
    public class MediaMountedBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            SetupHelper.EnsureInitialized();

            var handler = Mvx.IoCProvider.Resolve<MediaMountedHandler>();
            if (intent.Action == Intent.ActionMediaMounted)
            {
                handler.MediaMounted(context, intent);
            }
            else
            {
                handler.MediaUnmounted(context, intent);
            }
        }
    }
}
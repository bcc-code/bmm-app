using Android.App;
using Android.Content;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross;

namespace BMM.UI.Droid.Application.DownloadManager
{
    [BroadcastReceiver(Enabled = true, Exported = true), IntentFilter(new[] { Android.App.DownloadManager.ActionDownloadComplete})]
    public class DownloadCompletedBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            SetupHelper.EnsureInitialized();

            var handler = Mvx.IoCProvider.Resolve<DownloadCompletedHandler>();
            handler.DownloadCompleted(context, intent);
        }
    }
}
using System;
using System.Linq;
using Android.Content;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.DownloadManager;
using BMM.Core.Messages;
using MvvmCross.Plugin.Messenger;

namespace BMM.UI.Droid.Application.DownloadManager
{
    // ToDo: What's the purpose of this class?
    [Obsolete]
    public class DownloadCompletedHandler
    {
        private readonly AndroidDownloadManager _downloadManager;

        private readonly IAnalytics _analytics;
        private readonly IMvxMessenger _messenger;

        public DownloadCompletedHandler(IDownloadManager downloadManager, IAnalytics analytics, IMvxMessenger messenger)
        {
            this._analytics = analytics;
            _messenger = messenger;
            this._downloadManager = (AndroidDownloadManager) downloadManager;
        }

        public void DownloadCompleted(Context context, Intent intent)
        {
            _analytics.LogEvent("DownloadCompletedHandler called");

            var reference = intent.GetLongExtra(Android.App.DownloadManager.ExtraDownloadId, -1);

            var downloadFile = _downloadManager.Queue.Cast<DownloadFileImplementation>().FirstOrDefault(f => f.Id == reference);
            if (downloadFile == null) return;

            var query = new Android.App.DownloadManager.Query();
            query.SetFilterById(downloadFile.Id);

            try
            {
                using (var cursor = ((Android.App.DownloadManager)context.GetSystemService(Context.DownloadService)).InvokeQuery(query))
                {
                    if (cursor == null || cursor.Count == 0)
                    {
                        _analytics.LogEvent("DownloadCompletedHandler Download Canceled");
                        _messenger.Publish(new DownloadCanceledMessage(this));
                        _downloadManager.AbortAll();
                    }

                    while (cursor != null && cursor.MoveToNext())
                    {
                        _downloadManager.UpdateFileProperties(cursor, downloadFile);
                    }
                    cursor?.Close();
                }
            }
            catch (Android.Database.Sqlite.SQLiteException)
            {
                // I lately got an exception that the database was unaccessible ...
            }
        }
    }
}
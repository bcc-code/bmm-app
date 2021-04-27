﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Database;
using BMM.Core.Implementations.DownloadManager;

namespace BMM.UI.Droid.Application.DownloadManager
{
    [Obsolete]
    public class AndroidDownloadManager : IDownloadManager
    {
        private Android.OS.Handler _downloadWatcherHandler;
        private Java.Lang.Runnable _downloadWatcherHandlerRunnable;

        private readonly Android.App.DownloadManager _downloadManager;

        private readonly IList<IDownloadFile> _queue;

        public IEnumerable<IDownloadFile> Queue
        {
            get
            {
                lock (_queue)
                {
                    return _queue.ToList();
                }
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public Func<IDownloadFile, string> PathNameForDownloadedFile { get; set; }

        public DownloadVisibility NotificationVisibility;

        public bool IsVisibleInDownloadsUi { get; set; } = false;

        public AndroidDownloadManager()
        {
            _queue = new List<IDownloadFile>();

            _downloadManager = (Android.App.DownloadManager)Android.App.Application.Context.GetSystemService(Context.DownloadService);

            // Add all items to the Queue that are pending, paused or running
            LoopOnDownloads(ReinitializeFile);

            // Check sequentially if parameters for any of the registered downloads changed
            // ToDo: Creating the DownloadWatcher causes an exception since no longer using the NuGet package. That is probably because we didn't migrate the LazyThreadSafetyMode.PublicationOnly; Check if it is really needed since the app seems to look fine without it
            //StartDownloadWatcher();
        }

        public IDownloadFile CreateDownloadFile(string url)
        {
            return CreateDownloadFile(url, new Dictionary<string, string>());
        }

        public IDownloadFile CreateDownloadFile(string url, IDictionary<string, string> headers)
        {
            return new DownloadFileImplementation(url, headers);
        }

        public void Start(IDownloadFile i, bool mobileNetworkAllowed = true)
        {
            var file = (DownloadFileImplementation)i;

            string destinationPathName = null;
            if (PathNameForDownloadedFile != null)
            {
                destinationPathName = PathNameForDownloadedFile(file);
            }

            file.StartDownload(_downloadManager, destinationPathName, mobileNetworkAllowed, NotificationVisibility, IsVisibleInDownloadsUi);
            AddFile(file);
        }

        public void Abort(IDownloadFile i)
        {
            var file = (DownloadFileImplementation)i;

            file.Status = DownloadFileStatus.CANCELED;
            _downloadManager.Remove(file.Id);
            RemoveFile(file);
        }

        public void AbortAll()
        {
            foreach (var file in Queue)
            {
                Abort(file);
            }
        }

        void LoopOnDownloads(Action<ICursor> runnable)
        {
            // Reinitialize downloads that were started before the app was terminated or suspended
            var query = new Android.App.DownloadManager.Query();
            query.SetFilterByStatus(
                DownloadStatus.Paused |
                DownloadStatus.Pending |
                DownloadStatus.Running
            );

            try
            {
                using (var cursor = _downloadManager.InvokeQuery(query))
                {
                    while (cursor != null && cursor.MoveToNext())
                    {
                        runnable.Invoke(cursor);
                    }
                    cursor?.Close();
                }
            }
            catch (Android.Database.Sqlite.SQLiteException)
            {
                // I lately got an exception that the database was unaccessible ...
            }
        }

        void ReinitializeFile(ICursor cursor)
        {
            var downloadFile = new DownloadFileImplementation(cursor);

            AddFile(downloadFile);
            UpdateFileProperties(cursor, downloadFile);
        }

        /// <summary>
        /// Since the android DownloadManager does not provide any way of being notified when a file is download is finished this 
        /// watcher polls the download and updates its status. This allows for the UI to update when the download is finished.
        /// </summary>
        void StartDownloadWatcher()
        {
            // Create an instance for a runnable-handler
            _downloadWatcherHandler = new Android.OS.Handler();

            // Create a runnable, restarting itself to update every file in the queue
            _downloadWatcherHandlerRunnable = new Java.Lang.Runnable(() => {
                var downloads = Queue.Cast<DownloadFileImplementation>().ToList();
                LoopOnDownloads(cursor => {
                    int id = cursor.GetInt(cursor.GetColumnIndex(Android.App.DownloadManager.ColumnId));
                    var downloadFile = downloads.FirstOrDefault(f => f.Id == id);

                    if (downloadFile != null)
                    {
                        downloads.Remove(downloadFile);
                        UpdateFileProperties(cursor, downloadFile);
                    }
                });

                // All downloads still in this list are not listed in the native download-manager of Android. Mark them as canceled.
                foreach (var file in downloads)
                {
                    Abort(file);
                }

                _downloadWatcherHandler.PostDelayed(_downloadWatcherHandlerRunnable, 1000);
            });

            // Start this playing handler immediately
            _downloadWatcherHandler.PostDelayed(_downloadWatcherHandlerRunnable, 0);
        }

        /**
         * Update the properties for a file by it's cursor.
         * This method should be called in an interval and on reinitialization.
         */
        public void UpdateFileProperties(ICursor cursor, DownloadFileImplementation downloadFile)
        {
            downloadFile.TotalBytesWritten = cursor.GetInt(cursor.GetColumnIndex(Android.App.DownloadManager.ColumnBytesDownloadedSoFar));
            downloadFile.TotalBytesExpected = cursor.GetInt(cursor.GetColumnIndex(Android.App.DownloadManager.ColumnTotalSizeBytes));

            switch ((DownloadStatus)cursor.GetInt(cursor.GetColumnIndex(Android.App.DownloadManager.ColumnStatus)))
            {
                case DownloadStatus.Successful:
                    downloadFile.StatusDetails = default(string);
                    downloadFile.Status = DownloadFileStatus.COMPLETED;
                    RemoveFile(downloadFile);
                    break;

                case DownloadStatus.Failed:
                    downloadFile.Status = DownloadFileStatus.FAILED;
                    RemoveFile(downloadFile);

                    var reasonFailed = cursor.GetInt(cursor.GetColumnIndex(Android.App.DownloadManager.ColumnReason));
                    if (reasonFailed < 600)
                    {
                        downloadFile.StatusDetails = "Error.HttpCode: " + reasonFailed;
                    }
                    else
                    {
                        switch ((DownloadError)reasonFailed)
                        {
                            case DownloadError.CannotResume:
                                downloadFile.StatusDetails = "Error.CannotResume";
                                break;
                            case DownloadError.DeviceNotFound:
                                downloadFile.StatusDetails = "Error.DeviceNotFound";
                                break;
                            case DownloadError.FileAlreadyExists:
                                downloadFile.StatusDetails = "Error.FileAlreadyExists";
                                break;
                            case DownloadError.FileError:
                                downloadFile.StatusDetails = "Error.FileError";
                                break;
                            case DownloadError.HttpDataError:
                                downloadFile.StatusDetails = "Error.HttpDataError";
                                break;
                            case DownloadError.InsufficientSpace:
                                downloadFile.StatusDetails = "Error.InsufficientSpace";
                                break;
                            case DownloadError.TooManyRedirects:
                                downloadFile.StatusDetails = "Error.TooManyRedirects";
                                break;
                            case DownloadError.UnhandledHttpCode:
                                downloadFile.StatusDetails = "Error.UnhandledHttpCode";
                                break;
                            case DownloadError.Unknown:
                                downloadFile.StatusDetails = "Error.Unknown";
                                break;
                            default:
                                downloadFile.StatusDetails = "Error.Unregistered: " + reasonFailed;
                                break;
                        }
                    }
                    break;

                case DownloadStatus.Paused:
                    downloadFile.Status = DownloadFileStatus.PAUSED;

                    var reasonPaused = cursor.GetInt(cursor.GetColumnIndex(Android.App.DownloadManager.ColumnReason));
                    switch ((DownloadPausedReason)reasonPaused)
                    {
                        case DownloadPausedReason.QueuedForWifi:
                            downloadFile.StatusDetails = "Paused.QueuedForWifi";
                            break;
                        case DownloadPausedReason.WaitingToRetry:
                            downloadFile.StatusDetails = "Paused.WaitingToRetry";
                            break;
                        case DownloadPausedReason.WaitingForNetwork:
                            downloadFile.StatusDetails = "Paused.WaitingForNetwork";
                            break;
                        case DownloadPausedReason.Unknown:
                            downloadFile.StatusDetails = "Paused.Unknown";
                            break;
                        default:
                            downloadFile.StatusDetails = "Paused.Unregistered: " + reasonPaused;
                            break;
                    }
                    break;

                case DownloadStatus.Pending:
                    downloadFile.StatusDetails = default(string);
                    downloadFile.Status = DownloadFileStatus.PENDING;
                    break;

                case DownloadStatus.Running:
                    downloadFile.StatusDetails = default(string);
                    downloadFile.Status = DownloadFileStatus.RUNNING;
                    break;
            }
        }

        protected internal void AddFile(IDownloadFile file)
        {
            lock (_queue)
            {
                _queue.Add(file);
            }

            CollectionChanged?.Invoke(Queue, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, file));
        }

        protected internal void RemoveFile(IDownloadFile file)
        {
            lock (_queue)
            {
                _queue.Remove(file);
            }

            CollectionChanged?.Invoke(Queue, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, file));
        }
    }
}
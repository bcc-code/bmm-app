using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Downloading.FileDownloader;
using BMM.Core.Implementations.FileStorage;
using Foundation;

namespace BMM.UI.iOS.Implementations.Download
{
    public class IosFileDownloader : IFileDownloader
    {
        public static Action BackgroundSessionCompletionHandler;
        public readonly OngoingDownloadsLockDictionary OngoingDownloadsLockDictionary = new OngoingDownloadsLockDictionary();

        private readonly IMediaRequestHttpHeaders _headers;
        private readonly NSUrlSession _session;
        private readonly IStorageManager _storageManager;
        private readonly INetworkSettings _networkSettings;
        private readonly ILogger _logger;
        private readonly IAnalytics _analytics;
        private readonly NSFileManager _fileManager;

        public IosFileDownloader(IStorageManager storageManager,
            INetworkSettings networkSettings, ILogger logger, IAnalytics analytics, IMediaRequestHttpHeaders headers)
        {
            _storageManager = storageManager;
            _networkSettings = networkSettings;
            _logger = logger;
            _analytics = analytics;
            _headers = headers;
            _fileManager = new NSFileManager();
            _session = InitBackgroundSession();
        }

        // See https://developer.apple.com/documentation/foundation/url_loading_system/downloading_files_in_the_background
        private NSUrlSession InitBackgroundSession()
        {
            var identifier = NSBundle.MainBundle.BundleIdentifier + ".BackgroundTransferSession";
            var config = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration(identifier);
            var downloadDelegate = new IosDownloadDelegate(this, _logger) as INSUrlSessionDownloadDelegate;
            var session = NSUrlSession.FromConfiguration(config, downloadDelegate, null);
            return session;
        }

        public async Task DownloadFile(IDownloadable downloadable)
        {
            var ongoingDownload = await StartDownloadFor(downloadable);
            OngoingDownloadsLockDictionary.Add(ongoingDownload);
            await ongoingDownload.TaskCompletionSource.Task;
        }

        public bool MoveFile(NSUrl temporaryLocation, IDownloadable downloadable, out NSError error)
        {
            var destinationPath = new NSUrl(_storageManager.SelectedStorage.GetUrlByFile(downloadable), false);
            var moveSuccessful = _fileManager.Replace(
                destinationPath,
                temporaryLocation,
                null,
                NSFileManagerItemReplacementOptions.UsingNewMetadataOnly,
                out var resultingPath,
                out error);

            if (!moveSuccessful)
                return false;

            if (IsFileCorrupted(resultingPath.Path))
            {
                RecoverFromCorruptFile(resultingPath, downloadable, out error);
                return false;
            }

            return true;
        }

        public void CancelDownload()
        {
            OngoingDownloadsLockDictionary.CancelAll();
        }

        private async Task<OngoingDownload> StartDownloadFor(IDownloadable downloadable)
        {
            var request = await BuildRequestFor(downloadable);

            var taskCompletionSource = new TaskCompletionSource<bool>();
            var downloadTask = _session.CreateDownloadTask(request);
            var ongoingDownload = new OngoingDownload(taskCompletionSource, downloadTask, downloadable);

            downloadTask.Resume();
            return ongoingDownload;
        }

        private async Task<NSMutableUrlRequest> BuildRequestFor(IDownloadable downloadable)
        {
            var url = NSUrl.FromString(downloadable.Url);
            var headerDictionary = new NSMutableDictionary();
            foreach (var header in await _headers.GetHeaders())
            {
                headerDictionary.Add(new NSString(header.Key), new NSString(header.Value));
            }

            var request = new NSMutableUrlRequest(url)
            {
                AllowsCellularAccess = await _networkSettings.GetMobileNetworkDownloadAllowed(),
                Headers = headerDictionary
            };
            return request;
        }

        private bool IsFileCorrupted(string localPath)
        {
            try
            {
                using (TagLib.File.Create(localPath))
                { }

                return false;
            }
            catch
            {
                return true;
            }
        }

        // Todo #20112 remove logging after resolving the issue
        private void RecoverFromCorruptFile(NSUrl localPath, IDownloadable originalDownloadable, out NSError error)
        {
            _fileManager.Remove(localPath, out error);
            _analytics.LogEvent("Downloaded file is corrupted",
                new Dictionary<string, object>
                {
                    {"trackId", originalDownloadable.Id}
                });
            error = null;
        }
    }
}
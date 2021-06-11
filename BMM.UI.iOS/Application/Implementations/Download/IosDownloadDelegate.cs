using System;
using BMM.Api.Framework;
using BMM.UI.iOS.Implementations.Download.Exceptions;
using CoreFoundation;
using Foundation;

namespace BMM.UI.iOS.Implementations.Download
{
    public class IosDownloadDelegate : NSUrlSessionDownloadDelegate
    {
        private readonly IosFileDownloader _fileDownloader;
        private readonly ILogger _logger;

        public IosDownloadDelegate(IosFileDownloader fileDownloader, ILogger logger)
        {
            _fileDownloader = fileDownloader;
            _logger = logger;
        }

        public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
        {
            var originalRequestUrl = task?.OriginalRequest?.Url.AbsoluteString;
            try
            {
                if (error == null)
                    return;

                var download = _fileDownloader.OngoingDownloadsLockDictionary.GetDownloadByTask(task);
                if (download != null)
                {
                    download.Value.TaskCompletionSource.TrySetException(new IosDownloadException(originalRequestUrl, error));
                    _fileDownloader.OngoingDownloadsLockDictionary.RemoveDownloadForTask(task);
                    return;
                }

                var exception = new IosFailedDownloadRecoverException(originalRequestUrl);
                _logger.Error(GetType().Name, exception.Message, exception);
            }
            catch (Exception ex)
            {
                var wrappedEx = new UnexpectedIosDownloadException(originalRequestUrl, ex);
                _logger.Error(GetType().Name, wrappedEx.Message, wrappedEx);
            }
        }

        public override void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl temporaryLocation)
        {
            var originalRequestUrl = downloadTask?.OriginalRequest?.Url.AbsoluteString;
            try
            {
                var download = _fileDownloader.OngoingDownloadsLockDictionary.GetDownloadByTask(downloadTask);
                if (download == null)
                {
                    var exception = new IosDownloadDoesNotExistException(originalRequestUrl);
                    _logger.Error(GetType().Name, exception.Message, exception);
                    return;
                }

                _fileDownloader.OngoingDownloadsLockDictionary.RemoveDownloadForTask(downloadTask);

                if (_fileDownloader.MoveFile(temporaryLocation, download.Value.Downloadable, out NSError moveError))
                {
                    download.Value.TaskCompletionSource.TrySetResult(true);
                    return;
                }

                download.Value.TaskCompletionSource.TrySetException(new IosDownloadException(originalRequestUrl, moveError));
            }
            catch (Exception ex)
            {
                var wrappedEx = new UnexpectedIosDownloadException(originalRequestUrl, ex);
                _logger.Error(GetType().Name, wrappedEx.Message, wrappedEx);
            }
        }

        public override void DidFinishEventsForBackgroundSession(NSUrlSession session)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => IosFileDownloader.BackgroundSessionCompletionHandler?.Invoke());
        }
    }
}
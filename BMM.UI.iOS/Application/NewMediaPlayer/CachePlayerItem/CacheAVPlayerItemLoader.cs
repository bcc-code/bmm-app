using System;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class CacheAVPlayerItemLoader
    {
        private readonly IMediaRequestHttpHeaders _mediaRequestHttpHeaders;
        private CacheMediaFileHandle _cacheMediaFileHandle;
        private NSUrlSession _session;
        private NSUrlResponse _response;

        public CacheAVPlayerItemLoader(IMediaRequestHttpHeaders mediaRequestHttpHeaders,
            string uniqueKey)
        {
            _mediaRequestHttpHeaders = mediaRequestHttpHeaders;
            UniqueKey = uniqueKey;
        }

        public bool IsFullyDownloaded { get; private set; }
        public bool MoreThanAHalfFinished { get; private set; }
        public string UniqueKey { get; private set; }
        
        /// <summary>
        ///     Notifies about finishing of download.
        ///     Bool parameter indicated if download was finished because of an error.
        /// </summary>
        public event EventHandler<bool> FinishedLoading;

        public async Task StartDataRequest(string url)
        {
            var configuration = NSUrlSessionConfiguration.BackgroundSessionConfiguration(UniqueKey);

            var urlSessionDelegate = new CacheAVPlayerItemURLSessionDelegate();
            urlSessionDelegate.ReceivedData = (urlSession, task, data) =>
            {
                _cacheMediaFileHandle.Append(data);
                CheckDownloadStatus();

                if (!IsFullyDownloaded)
                    return;
                
                Finish();
                _cacheMediaFileHandle.RemoveLoadingIndicator();
                FinishedLoading?.Invoke(this, false);
            };

            urlSessionDelegate.ReceivedResponse = (urlSession, task, response) =>
            {
                _response = response;
                _cacheMediaFileHandle = CacheMediaFileHandle.CreateNewFile(UniqueKey, response.ExpectedContentLength);
            };

            urlSessionDelegate.CompletedWithError = (urlSession, task, nsError) =>
            {
                _session?.InvalidateAndCancel();
                
                if (nsError == null)
                {
                    CheckDownloadStatus();
                    return;
                }
                
                CloseFileHandle();
                FinishedLoading?.Invoke(this, true);
            };

            _session = NSUrlSession.FromConfiguration(configuration, urlSessionDelegate, null);
            var nsMutableUrlRequest = new NSMutableUrlRequest
            {
                Url = new NSUrl(url),
                Headers = await GetOptionsWithHeaders()
            };

            var dataTask = _session.CreateDataTask(nsMutableUrlRequest);
            dataTask.Resume();
        }

        private async Task<NSMutableDictionary> GetOptionsWithHeaders()
        {
            var mediaHeaders = await _mediaRequestHttpHeaders.GetHeaders();
            var headerDictionary = new NSMutableDictionary();

            foreach (var header in mediaHeaders)
                headerDictionary.Add(new NSString(header.Key), new NSString(header.Value));

            return headerDictionary;
        }

        private void CheckDownloadStatus()
        {
            if (_response == null)
                return;

            long currentSize = _cacheMediaFileHandle.GetFileSize();
            
            // ReSharper disable once PossibleLossOfFraction
            MoreThanAHalfFinished = currentSize / _response.ExpectedContentLength > 0.5;
            IsFullyDownloaded = currentSize == _response.ExpectedContentLength;
        }

        public void Cancel()
        {
            _session?.InvalidateAndCancel();
            CloseFileHandle();
        }
        
        public void Finish()
        {
            _session?.FinishTasksAndInvalidate();
            CloseFileHandle();
        }

        private void CloseFileHandle()
        {
            _cacheMediaFileHandle?.Close();
        }
    }
}
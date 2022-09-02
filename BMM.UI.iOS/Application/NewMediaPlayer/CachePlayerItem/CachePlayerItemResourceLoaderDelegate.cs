using System;
using System.Diagnostics;
using AVFoundation;
using BMM.UI.iOS.NewMediaPlayer.CachePlayerItem;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class CachePlayerItemResourceLoaderDelegate
        : AVAssetResourceLoaderDelegate
    {
        private readonly NSDictionary _headers;
        private NSMutableData _mediaData;
        private NSUrlResponse _response;
        private AVAssetResourceLoadingRequest _pendingRequest;

        public CachePlayerItemResourceLoaderDelegate(NSDictionary headers)
        {
            _headers = headers;
        }

        public NSUrlSession Session { get; private set; }
        public CacheAVPlayerItem Owner { get; set; }
        
        public override bool ShouldWaitForLoadingOfRequestedResource(AVAssetResourceLoader resourceLoader,
            AVAssetResourceLoadingRequest loadingRequest)
        {
            _pendingRequest = loadingRequest;
            ProcessRequest();
            return true;
        }

        public override void DidCancelLoadingRequest(AVAssetResourceLoader resourceLoader, AVAssetResourceLoadingRequest loadingRequest)
        {
            _pendingRequest = null;
        }

        public void StartDataRequest(string url)
        {
            var configuration = NSUrlSessionConfiguration.DefaultSessionConfiguration;
            
            var urlSessionDelegate = new CacheAVPlayerItemURLSessionDelegate();
            urlSessionDelegate.ReceivedData = (urlSession, task, data) =>
            {
                _mediaData.AppendData(data);
                ProcessRequest();
                CheckIsDownloaded();
            };
            
            urlSessionDelegate.ReceivedResponse = (urlSession, task, response) =>
            {
                 _mediaData = new NSMutableData();
                _response = response;
                ProcessRequest();
            };
            
            urlSessionDelegate.CompletedWithError = (urlSession, task, nsError) =>
            {
                ProcessRequest();
                Session?.InvalidateAndCancel();
                CheckIsDownloaded();
            };
            
            Session = NSUrlSession.FromConfiguration(configuration, urlSessionDelegate, null);
            
            var nsMutableUrlRequest = new NSMutableUrlRequest
            {
                Url = new NSUrl(url),
                Headers = _headers
            };
            
            var dataTask = Session.CreateDataTask(nsMutableUrlRequest);
            dataTask.Resume();
        }

        private void ProcessRequest()
        {
            if (_pendingRequest == null)
                return;

            FillInContentInformationRequest(_pendingRequest.ContentInformationRequest);
            if (!HasEnoughDataToFulfillRequest(_pendingRequest.DataRequest))
                return;
            
            _pendingRequest.FinishLoading();
            _pendingRequest = null;
        }

        private void CheckIsDownloaded()
        {
            if (_response == null)
                return;

            Owner.IsDownloaded = (long)_mediaData.Length == _response.ExpectedContentLength;
        }

        private void FillInContentInformationRequest(AVAssetResourceLoadingContentInformationRequest contentInformationRequest)
        {
            if (contentInformationRequest == null)
                return;

            contentInformationRequest.ByteRangeAccessSupported = true;
            
            if (_response == null)
                return;
            
            contentInformationRequest.ContentType = _response.MimeType;
            contentInformationRequest.ContentLength = _response.ExpectedContentLength;
        }

        private bool HasEnoughDataToFulfillRequest(AVAssetResourceLoadingDataRequest dataRequest)
        {
            long requestedOffset = dataRequest.RequestedOffset;
            long requestedLength = dataRequest.RequestedLength;
            long currentOffset = dataRequest.CurrentOffset;

            var songDataUnwrapped = _mediaData;

            if (songDataUnwrapped == null || (long)songDataUnwrapped.Length <= currentOffset)
                return false;

            long bytesToRespond = Math.Min((long)songDataUnwrapped.Length - currentOffset, requestedLength);
            var dataToRespond = songDataUnwrapped.Subdata(new NSRange((nint)currentOffset, (nint)(currentOffset + bytesToRespond)));
            dataRequest.Respond(dataToRespond);

            return (long)songDataUnwrapped.Length >= requestedLength + requestedOffset;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Session?.InvalidateAndCancel();
        }
    }
}
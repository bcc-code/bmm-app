using System;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer;
using BMM.UI.iOS.NewMediaPlayer.Interfaces;
using Foundation;
using MvvmCross;
using MvvmCross.Plugin.Messenger;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class CacheAVPlayerItemLoader : ICacheAVPlayerItemLoader
    {
        private readonly IMvxMessenger _mvxMessenger;
        private readonly IMediaRequestHttpHeaders _mediaRequestHttpHeaders;
        private CacheMediaFileHandle _cacheMediaFileHandle;
        private NSUrlSession _session;
        private NSUrlResponse _response;
        private NSUrlSessionDataTask _dataTask;
        private MvxSubscriptionToken _playbackStateSubscriptionToken;
        private IAudioPlayback _audioPlayback;

        public CacheAVPlayerItemLoader(
            IMvxMessenger mvxMessenger,
            IMediaRequestHttpHeaders mediaRequestHttpHeaders,
            string uniqueKey)
        {
            _mvxMessenger = mvxMessenger;
            _mediaRequestHttpHeaders = mediaRequestHttpHeaders;
            UniqueKey = uniqueKey;
        }

        private IAudioPlayback AudioPlayback => _audioPlayback ??= Mvx.IoCProvider.Resolve<IAudioPlayback>();
        public bool IsFullyDownloaded { get; private set; }
        public bool MoreThanAHalfFinished { get; private set; }
        public string UniqueKey { get; private set; }
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

            _dataTask = _session.CreateDataTask(nsMutableUrlRequest);
            AttachMessageListener();
            ResumeDataTask(AudioPlayback.Status);
        }

        private void OnPlaybackStateChangedAction(PlaybackStatusChangedMessage message)
        {
            if (_dataTask == null)
                return;
                
            bool shouldSuspend = message.PlaybackState.PlayStatus == PlayStatus.Buffering
                                 && _dataTask.State == NSUrlSessionTaskState.Running;

            if (shouldSuspend)
                _dataTask.Suspend();
            else
                ResumeDataTask(message.PlaybackState.PlayStatus);
        }

        private void ResumeDataTask(PlayStatus playStatus)
        {
            if (_dataTask == null)
                return;
            
            bool canResume = playStatus.IsNoneOf(PlayStatus.Buffering, PlayStatus.Stopped)
                             && _dataTask.State == NSUrlSessionTaskState.Suspended;

            if (!canResume)
                return;

            _dataTask.Resume();
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
            MoreThanAHalfFinished = currentSize / (double)_response.ExpectedContentLength > 0.5;
            IsFullyDownloaded = currentSize == _response.ExpectedContentLength;
        }

        public void Cancel()
        {
            _dataTask?.Cancel();
            _session?.InvalidateAndCancel();
            CloseFileHandle();
            _dataTask?.Dispose();
            _dataTask = null;
            DetachMessageListener();
        }

        private void Finish()
        {
            _session?.FinishTasksAndInvalidate();
            CloseFileHandle();
            _dataTask?.Dispose();
            _dataTask = null;
            DetachMessageListener();
        }
        
        private void AttachMessageListener()
        {
            _playbackStateSubscriptionToken = _mvxMessenger.Subscribe<PlaybackStatusChangedMessage>(OnPlaybackStateChangedAction);
        }
        
        private void DetachMessageListener()
        {
            _mvxMessenger.UnsubscribeSafe<PlaybackStatusChangedMessage>(_playbackStateSubscriptionToken);
        }

        private void CloseFileHandle()
        {
            _cacheMediaFileHandle?.Close();
        }
    }
}
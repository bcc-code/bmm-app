using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.NewMediaPlayer.Notification;
using BMM.UI.Droid.Application.NewMediaPlayer.Playback;
using BMM.UI.Droid.Application.NewMediaPlayer.Service;
using MvvmCross;
using MvvmCross.Platforms.Android;
using MvvmCross.Plugin.Messenger;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Controller
{
    public class AndroidMediaPlayer : MediaBrowserCompat.ConnectionCallback, IPlatformSpecificMediaPlayer
    {
        private const string StartTimeInMsKey = "startTimeInMs";

        private readonly IMediaQueue _mediaQueue;
        private readonly MediaControllerCallback _callback;
        private readonly PlaybackStateCompatMapper _mapper;
        private readonly IMetadataMapper _metadataMapper;
        private readonly ILogger _logger;
        private readonly IMvxMessenger _messenger;

        private Activity _activity;
        private MediaBrowserCompat _mediaBrowser;
        private MediaControllerCompat _mediaController;

        private ITrackModel _lastTrack;
        private IList<IMediaTrack> _lastQueue;
        private long _lastPosition;

        public AndroidMediaPlayer(
            IMediaQueue mediaQueue,
            MediaControllerCallback callback,
            PlaybackStateCompatMapper mapper,
            IMvxMessenger messenger,
            IMetadataMapper metadataMapper,
            ILogger logger)
        {
            _mediaQueue = mediaQueue;
            _callback = callback;
            _mapper = mapper;
            _metadataMapper = metadataMapper;
            _logger = logger;
            _messenger = messenger;
        }

        public Action ContinuingPreviousSession { get; set; }

        public Action AfterConnectedAction { get; set; }

        public void Connect(Activity activity)
        {
            if (activity is null)
                throw new ArgumentException("Activity cant be null. First time please connect with activity.");

            _activity = activity;
            if (_mediaBrowser == null)
            {
                // our goal is to use the ApplicationContext instead of the ActivityContext. But with the current implementation OnTaskRemoved() is never called
                // which prevents us from stopping the music when BMM is killed (swiped out of recent apps).
                // The theory is that this is fixed by upgrading to ExoPlayer 0.9.0 once Xamarin supports D8/R8. Until then we accept that the music is stopped when leaving the app using the back button.
                _mediaBrowser = new MediaBrowserCompat(activity, new ComponentName(activity, Java.Lang.Class.FromType(typeof(MusicService))), this, null);
            }

            if (_mediaBrowser.IsConnected)
                OnConnected();
            else
                _mediaBrowser.Connect();
        }

        public void Disconnect()
        {
            if (_mediaController != null)
            {
                _mediaController.UnregisterCallback(_callback);
                _mediaController.Dispose();
                _mediaController = null;
            }

            if (_mediaBrowser != null)
            {
                _mediaBrowser.Disconnect();
                _mediaBrowser.Dispose();
                _mediaBrowser = null;
            }
        }

        public ITrackModel CurrentTrack
        {
            get
            {
                if (_mediaController?.Metadata == null)
                {
                    return null;
                }

                return _metadataMapper.LookupTrackFromMetadata(_mediaController.Metadata, _mediaQueue);
            }
        }

        public bool IsPlaying => _mediaController?.PlaybackState?.IsPlaying() ?? false;

        public RepeatType RepeatType => _mapper.ConvertRepeatMode(_mediaController?.RepeatMode);

        public bool IsShuffleEnabled =>
            _mediaController?.ShuffleMode == PlaybackStateCompat.ShuffleModeGroup || _mediaController?.ShuffleMode == PlaybackStateCompat.ShuffleModeAll;

        public IPlaybackState PlaybackState => _mediaController?.PlaybackState?.ToPlaybackState(_mediaQueue) ?? new DefaultPlaybackState();

        public long CurrentPosition => PlaybackState.CurrentPosition;

        public async Task ShuffleList(IList<IMediaTrack> tracks, string playbackOrigin)
        {
            if (_mediaController != null)
            {
                var controls = _mediaController.GetTransportControls();

                if (await _mediaQueue.Replace(tracks, null))
                    controls.PlayFromMediaId(ExoPlaybackPreparer.MediaIdShuffle, null);
            }
        }

        public async Task Play(IList<IMediaTrack> mediaTracks, IMediaTrack currentTrack, long startTimeInMs = 0)
        {
            if (_mediaController != null)
            {
                bool queueStaysTheSame = _mediaQueue.IsSameQueue(mediaTracks);

                var controls = _mediaController.GetTransportControls();
                if (queueStaysTheSame && _mediaController.Queue != null)
                {
                    controls.SkipToQueueItem(mediaTracks.IndexOf(currentTrack));
                    controls.Play();
                    if (startTimeInMs > 0) controls.SeekTo(startTimeInMs);
                }
                else
                {
                    if (await _mediaQueue.Replace(mediaTracks, currentTrack))
                    {
                        var bundle = new Bundle();
                        bundle.PutLong(StartTimeInMsKey, startTimeInMs);
                        controls.PlayFromMediaId(currentTrack.Id.ToString(), bundle);
                    }
                }
            }
        }

        public async Task RecoverQueue(IList<IMediaTrack> mediaTracks, IMediaTrack currentTrack, long startTimeInMs = 0)
        {
            if (_mediaController == null)
                return;

            var controls = _mediaController.GetTransportControls();
            bool replaceSuccessful = await _mediaQueue.Replace(mediaTracks, currentTrack);

            if (!replaceSuccessful)
                return;

            var bundle = new Bundle();
            bundle.PutLong(StartTimeInMsKey, startTimeInMs);
            controls.PrepareFromMediaId(currentTrack.Id.ToString(), bundle);
        }

        public void PlayPause()
        {
            if (_mediaController != null)
            {
                var controls = _mediaController.GetTransportControls();

                if (_mediaController.PlaybackState != null && _mediaController.PlaybackState.IsPlaying())
                {
                    controls.Pause();
                }
                else
                {
                    if (CurrentTrack?.IsLivePlayback == true)
                        // For Live tracks we jump to the beginning of the current section. It jumps back a bit but it works consistently and prevents a "BehindLivewWindowException".
                        controls.SeekTo(0);

                    controls.Play();
                }
            }
        }

        public void Stop()
        {
            _mediaController.GetTransportControls().Stop();
            _messenger.Publish(new PlaybackStatusChangedMessage(this, _mediaController.PlaybackState.ToPlaybackState(_mediaQueue)));
            _messenger.Publish(new CurrentTrackChangedMessage(null, this));
            Disconnect();
        }

        /// <summary>
        /// Explanation why we log the error (It's in the summary so we can use cref):
        /// In case of error Player.IsCurrentWindowSeekable is false thereby preventing OnSeekTo ever being forwarded to <see cref="IdleRecoveringControlDispatcher.DispatchSeekTo"/>.
        /// To fix seeking in case of error we would have to make sure that IsCurrentWindowSeekable is true even in the case of an error.
        /// </summary>
        public void SeekTo(long newPosition)
        {
            if (_mediaController != null)
            {
                var controls = _mediaController.GetTransportControls();

                if (_mediaController.PlaybackState.State == PlaybackStateCompat.StateError)
                {
                    _logger.Error("AndroidMediaPlayer", "SeekTo has been triggered while the player is in error state.");
                }
                else
                {
                    controls.SeekTo(newPosition);
                    controls.Play();
                }
            }
        }

        public Task<bool> AddToEndOfQueue(IMediaTrack track, string playbackOrigin)
        {
            return AddToQueueAtIndex(track);
        }

        public Task<bool> QueueToPlayNext(IMediaTrack track, string playbackOrgin)
        {
            var nextPlayedIndex = _mediaQueue.Tracks.IndexOf(CurrentTrack as IMediaTrack) + 1;
            return AddToQueueAtIndex(track, nextPlayedIndex);
        }

        private async Task<bool> AddToQueueAtIndex(IMediaTrack track, int? index = null)
        {
            if (_mediaController != null)
            {
                // ToDo: maybe we should check if that item is already in the queue.
                var description = _metadataMapper.BuildMediaDescription(track);

                if (index == null)
                {
                    _mediaController.AddQueueItem(description);
                    return await _mediaQueue.Append(track);
                }

                _mediaController.AddQueueItem(description, index.Value);
                return await _mediaQueue.PlayNext(track, CurrentTrack as IMediaTrack);
            }

            return false;
        }

        public void SetRepeatType(RepeatType type)
        {
            var controls = _mediaController.GetTransportControls();
            controls.SetRepeatMode(_mapper.ConvertRepeatType(type));
        }

        public void SetShuffle(bool isShuffleEnabled)
        {
            // We don't update the queue after shuffling. To implement that we could implement a CustomAction and follow
            // these suggestions: https://github.com/google/ExoPlayer/issues/4255#issuecomment-389086814

            var controls = _mediaController.GetTransportControls();
            controls.SetShuffleMode(isShuffleEnabled ? PlaybackStateCompat.ShuffleModeGroup : PlaybackStateCompat.ShuffleModeNone);
        }

        public void PlayNext()
        {
            var controls = _mediaController.GetTransportControls();
            controls.SkipToNext();
            controls.Play();
        }

        public void PlayPrevious()
        {
            var controls = _mediaController.GetTransportControls();
            controls.SkipToPrevious();
            controls.Play();
        }

        public void PlayPreviousOrSeekToStart()
        {
            var controls = _mediaController.GetTransportControls();
            controls.SkipToPrevious();
            controls.Play();
        }

        public override async void OnConnected()
        {
            if (_mediaController == null)
            {
                _mediaController = new MediaControllerCompat(_activity, _mediaBrowser.SessionToken);
                _mediaController.RegisterCallback(_callback);
            }

            if (CurrentTrack != null)
            {
                ContinuingPreviousSession?.Invoke();
                _callback.OnMetadataChanged(_mediaController.Metadata);
            }

            AfterConnectedAction?.Invoke();
            AfterConnectedAction = null;
        }

        public void SaveCurrentTrackAndQueueAfterThemeChanged()
        {
            _lastTrack = CurrentTrack;
            _lastPosition = CurrentPosition;
            _lastQueue = _mediaQueue?.Tracks;
        }

        public async Task RestoreLastPlayingTrackAfterThemeChangedIfAvailable()
        {
            if (_lastTrack == null || _lastQueue == null || !_lastQueue.Any())
                return;

            await RecoverQueue(_lastQueue, (IMediaTrack)_lastTrack, _lastPosition);
            ClearCurrentTrackAndQueueAfterThemeChanged();
        }

        private void ClearCurrentTrackAndQueueAfterThemeChanged()
        {
            _lastTrack = default;
            _lastPosition = default;
            _lastQueue = default;
        }

        public override void OnConnectionFailed()
        {
            Mvx.IoCProvider.Resolve<IPlayerAnalytics>().MediaBrowserConnectionFailed();
        }

        public override void OnConnectionSuspended()
        {
            Disconnect();
            Mvx.IoCProvider.Resolve<IPlayerAnalytics>().MediaBrowserConnectionSuspended();
        }
    }
}
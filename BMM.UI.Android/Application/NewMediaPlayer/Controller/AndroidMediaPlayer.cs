using Android.Content;
using Android.Media;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.NewMediaPlayer.Constants;
using BMM.UI.Droid.Application.Constants.Player;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.NewMediaPlayer.Service;
using Com.Google.Android.Exoplayer2;
using Java.Lang;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Plugin.Messenger;
using Exception = System.Exception;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Controller;

public class AndroidMediaPlayer : MediaBrowserCompat.ConnectionCallback, IPlatformSpecificMediaPlayer
{
    public const string StartTimeInMsKey = "startTimeInMs";

    private readonly IMediaQueue _mediaQueue;
    private readonly MediaControllerCallback _callback;
    private readonly PlaybackStateCompatMapper _mapper;
    private readonly IMetadataMapper _metadataMapper;
    private readonly ILogger _logger;
    private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;
    private readonly IMvxMessenger _messenger;

    private Activity _activity;
    private MediaBrowserCompat _mediaBrowser;
    private MediaControllerCompat _mediaController;

    private ITrackModel _lastTrack;
    private IList<IMediaTrack> _lastQueue;
    private long _lastPosition;
    private decimal _lastPlaybackSpeed;
    private SemaphoreSlim _connectSemaphoreSlim = new(1, 1);

    public AndroidMediaPlayer(
        IMediaQueue mediaQueue,
        MediaControllerCallback callback,
        PlaybackStateCompatMapper mapper,
        IMvxMessenger messenger,
        IMetadataMapper metadataMapper,
        ILogger logger,
        IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher)
    {
        _mediaQueue = mediaQueue;
        _callback = callback;
        _mapper = mapper;
        _metadataMapper = metadataMapper;
        _logger = logger;
        _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
        _messenger = messenger;
    }

    public bool IsConnected { get; private set; }
    public Action ContinuingPreviousSession { get; set; }

    public Func<Task> AfterConnectedAction { get; set; }

    public async Task Connect(Activity activity)
    {
        await _connectSemaphoreSlim.WaitAsync();
        
        if (activity is null)
            throw new ArgumentException("Activity cant be null. First time please connect with activity.");

        _activity = activity;
        if (_mediaBrowser == null)
        {
            // our goal is to use the ApplicationContext instead of the ActivityContext. But with the current implementation OnTaskRemoved() is never called
            // which prevents us from stopping the music when BMM is killed (swiped out of recent apps).
            // The theory is that this is fixed by upgrading to ExoPlayer 0.9.0 once Xamarin supports D8/R8. Until then we accept that the music is stopped when leaving the app using the back button.
            _mediaBrowser = new MediaBrowserCompat(activity,
                new ComponentName(activity, Class.FromType(typeof(MusicService))),
                this,
                null);
        }

        if (_mediaBrowser.IsConnected)
            OnConnected();
        else
            _mediaBrowser.Connect();
    }

    public void Disconnect()
    {
        IsConnected = false;
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
        _mediaController?.ShuffleMode == PlaybackStateCompat.ShuffleModeGroup ||
        _mediaController?.ShuffleMode == PlaybackStateCompat.ShuffleModeAll;

    public IPlaybackState PlaybackState =>
        _mediaController?.PlaybackState?.ToPlaybackState(_mediaQueue, CurrentPlaybackSpeed) ??
        new DefaultPlaybackState();

    public long CurrentPosition => PlaybackState.CurrentPosition;

    public async Task ShuffleList(IList<IMediaTrack> tracks, string playbackOrigin)
    {
        if (_mediaController != null)
        {
            var controls = _mediaController.GetTransportControls();

            if (await _mediaQueue.Replace(tracks, null))
                controls.PlayFromMediaId(ExoPlayerConstants.MediaIdShuffle, null);
        }
    }

    public async Task Play(IList<IMediaTrack> mediaTracks, IMediaTrack currentTrack, long startTimeInMs = 0,
        bool resetPlaybackSpeed = true)
    {
        if (_mediaController == null)
            return;

        bool queueStaysTheSame = _mediaQueue.IsSameQueue(mediaTracks);

        var controls = _mediaController.GetTransportControls();
        if (queueStaysTheSame && _mediaController.Queue != null)
        {
            controls.SkipToQueueItem(mediaTracks.IndexOf(currentTrack));
            SeekTo(startTimeInMs);
        }
        else
        {
            if (await _mediaQueue.Replace(mediaTracks, currentTrack))
            {
                var bundle = new Bundle();
                bundle.PutLong(StartTimeInMsKey, startTimeInMs);
                _messenger.Publish(new CurrentTrackChangedMessage(currentTrack, startTimeInMs, this));
                controls.PlayFromMediaId(currentTrack.Id.ToString(), bundle);
            }
        }

        if (resetPlaybackSpeed)
            ChangePlaybackSpeed(PlayerConstants.NormalPlaybackSpeed);
    }

    public async Task ForceReloadQueueAndPlayCurrentTrack()
    {
        await _mediaQueue.Replace(_mediaQueue.Tracks, (IMediaTrack)CurrentTrack);
        var controls = _mediaController.GetTransportControls();
        controls!.PlayFromMediaId(CurrentTrack.Id.ToString(), null);
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
                controls.Pause();
            else
                controls.Play();
        }
    }

    public void Stop()
    {
        _mediaController.GetTransportControls().Stop();
        _messenger.Publish(new PlaybackStatusChangedMessage(this,
            _mediaController.PlaybackState.ToPlaybackState(_mediaQueue, CurrentPlaybackSpeed)));
        _messenger.Publish(new CurrentTrackChangedMessage(null, 0, this));
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
                
               _messenger.Publish(new PlaybackSeekedMessage(this)
                    {
                        CurrentPosition = CurrentPosition,
                        SeekedPosition = newPosition
                    });
            }
        }
    }

    public void ChangePlaybackSpeed(decimal playbackSpeed)
    {
        _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
        {
            CurrentPlaybackSpeed = playbackSpeed;
            _callback.CurrentPlaybackSpeed = CurrentPlaybackSpeed;
            MusicService.CurrentExoPlayerInstance.IfNotNull(s =>
                s.PlaybackParameters = new PlaybackParameters((float)playbackSpeed));
            _messenger.Publish(new PlaybackStatusChangedMessage(this,
                _mediaController.PlaybackState.ToPlaybackState(_mediaQueue, CurrentPlaybackSpeed)));
        });
    }

    public decimal CurrentPlaybackSpeed { get; private set; } = PlayerConstants.NormalPlaybackSpeed;

    public Task<bool> AddToEndOfQueue(IMediaTrack track, string playbackOrigin, bool ignoreIfAlreadyAdded = false)
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
            var description = _metadataMapper.FromTrack(track);

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
        controls.SetShuffleMode(isShuffleEnabled
            ? PlaybackStateCompat.ShuffleModeGroup
            : PlaybackStateCompat.ShuffleModeNone);
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
        _connectSemaphoreSlim.TryRelease();
        
        IsConnected = true;
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

        if (AfterConnectedAction == null)
            return;

        await AfterConnectedAction.Invoke();
        AfterConnectedAction = null;
    }

    public void SaveCurrentTrackAndQueueAfterThemeChanged()
    {
        _lastTrack = CurrentTrack;
        _lastPosition = CurrentPosition;
        _lastQueue = _mediaQueue?.Tracks;
        _lastPlaybackSpeed = CurrentPlaybackSpeed;
    }

    public async Task RestoreLastPlayingTrackAfterThemeChangedIfAvailable()
    {
        if (_lastTrack == null || _lastQueue == null || !_lastQueue.Any())
            return;

        await RecoverQueue(_lastQueue, (IMediaTrack)_lastTrack, _lastPosition);
        ChangePlaybackSpeed(_lastPlaybackSpeed);
        ClearCurrentTrackAndQueueAfterThemeChanged();
    }

    private void ClearCurrentTrackAndQueueAfterThemeChanged()
    {
        _lastTrack = default;
        _lastPosition = default;
        _lastQueue = default;
        _lastPlaybackSpeed = default;
    }

    public override void OnConnectionFailed()
    {
        _connectSemaphoreSlim.TryRelease();
        Mvx.IoCProvider.Resolve<IPlayerAnalytics>().MediaBrowserConnectionFailed();
    }

    public override void OnConnectionSuspended()
    {
        _connectSemaphoreSlim.TryRelease();
        Disconnect();
        Mvx.IoCProvider.Resolve<IPlayerAnalytics>().MediaBrowserConnectionSuspended();
    }
}
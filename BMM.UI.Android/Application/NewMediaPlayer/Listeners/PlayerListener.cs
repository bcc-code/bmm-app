using BMM.Core.Implementations.UI;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.NewMediaPlayer.Constants;
using BMM.UI.Droid.Application.NewMediaPlayer.Controller;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Audio;
using Com.Google.Android.Exoplayer2.Metadata;
using Com.Google.Android.Exoplayer2.Text;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Video;
using MvvmCross;
using MvvmCross.Plugin.Messenger;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Listeners;

public class PlayerListener : Java.Lang.Object, IPlayer.IListener
{
    private const int TransitionReasonSeek = 2;
    private const int TransitionReasonAuto = 1;
    
    private readonly IExoPlayer _playerInstance;
    private double _lastPosition;
    private readonly IMvxMessenger _mvxMessenger;
    private readonly MvxSubscriptionToken _token;
    private readonly AndroidMediaPlayer _mediaPlayer;

    public PlayerListener(IExoPlayer playerInstance)
    {
        _playerInstance = playerInstance;
        _mvxMessenger = Mvx.IoCProvider.Resolve<IMvxMessenger>();
        _mediaPlayer = (AndroidMediaPlayer)Mvx.IoCProvider.Resolve<IPlatformSpecificMediaPlayer>();
        _token = _mvxMessenger.Subscribe<PlaybackPositionChangedMessage>((message => _lastPosition = message.CurrentPosition));
    }

    public void OnAudioAttributesChanged(AudioAttributes audioAttributes)
    {
    }

    public void OnAudioSessionIdChanged(int audioSessionId)
    {
    }

    public void OnAvailableCommandsChanged(IPlayer.Commands availableCommands)
    {
    }

    public void OnCues(CueGroup cueGroup)
    {
    }

    public void OnDeviceInfoChanged(DeviceInfo deviceInfo)
    {
    }

    public void OnDeviceVolumeChanged(int volume, bool muted)
    {
    }

    public void OnEvents(IPlayer player, IPlayer.Events events)
    {
    }

    public void OnIsLoadingChanged(bool isLoading)
    {
    }

    public void OnIsPlayingChanged(bool isPlaying)
    {
    }

    public void OnLoadingChanged(bool isLoading)
    {
    }

    public void OnMaxSeekToPreviousPositionChanged(long maxSeekToPreviousPositionMs)
    {
    }

    public void OnMediaItemTransition(MediaItem mediaItem, int reason)
    {
        _mediaPlayer.ReloadQueueIfNeeded(mediaItem);
        _mvxMessenger.Publish(new CurrentTrackWillChangeMessage(
            this,
            _lastPosition,
            _playerInstance.PlaybackParameters == null
                ? PlayerConstants.NormalPlaybackSpeed
                : (decimal)_playerInstance.PlaybackParameters.Speed));
    }

    public void OnMediaMetadataChanged(MediaMetadata mediaMetadata)
    {
    }

    public void OnMetadata(Metadata metadata)
    {
    }

    public void OnPlayWhenReadyChanged(bool playWhenReady, int reason)
    {
    }

    public void OnPlaybackParametersChanged(PlaybackParameters playbackParameters)
    {
    }

    public void OnPlaybackStateChanged(int playbackState)
    {
    }

    public void OnPlaybackSuppressionReasonChanged(int playbackSuppressionReason)
    {
    }

    public void OnPlayerError(PlaybackException error)
    {
    }

    public void OnPlayerErrorChanged(PlaybackException error)
    {
    }

    public void OnPlayerStateChanged(bool playWhenReady, int playbackState)
    {
    }

    public void OnPlaylistMetadataChanged(MediaMetadata mediaMetadata)
    {
    }

    public void OnPositionDiscontinuity(int reason)
    {
    }

    public void OnRenderedFirstFrame()
    {
    }

    public void OnRepeatModeChanged(int repeatMode)
    {
    }

    public void OnSeekBackIncrementChanged(long seekBackIncrementMs)
    {
    }

    public void OnSeekForwardIncrementChanged(long seekForwardIncrementMs)
    {
    }

    public void OnSeekProcessed()
    {
    }

    public void OnShuffleModeEnabledChanged(bool shuffleModeEnabled)
    {
    }

    public void OnSkipSilenceEnabledChanged(bool skipSilenceEnabled)
    {
    }

    public void OnSurfaceSizeChanged(int width, int height)
    {
    }

    public void OnTimelineChanged(Timeline timeline, int reason)
    {
    }

    public void OnTrackSelectionParametersChanged(TrackSelectionParameters parameters)
    {
    }

    public void OnTracksChanged(Tracks tracks)
    {
    }

    public void OnVideoSizeChanged(VideoSize videoSize)
    {
    }

    public void OnVolumeChanged(float volume)
    {
    }
}
using System;
using System.Collections.Generic;
using Android.Media;
using Android.OS;
using AndroidX.Media;
using BMM.Api.Framework;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using MvvmCross;
using MvvmCross.Plugin.Messenger;
using Object = Java.Lang.Object;

namespace BMM.UI.Droid.Application.NewMediaPlayer.AudioFocus
{
    public class AudioFocusExoPlayerDecorator : Object, IExoPlayer, AudioManager.IOnAudioFocusChangeListener, IPlayerEventListener
    {
        private const float MediaVolumeDefault = 1.0f;
        private const float MediaVolumeDuck = 0.2f;

        private readonly SimpleExoPlayer _player;

        private readonly ILogger _logger;

        private readonly AudioManager _audioManager;

        private bool _shouldPlayWhenReady;

        private readonly IList<IPlayerEventListener> _eventListeners;

        private readonly AudioAttributesCompat _audioAttributes = new AudioAttributesCompat.Builder()
            .SetContentType(AudioAttributesCompat.ContentTypeMusic)
            .SetUsage(AudioAttributesCompat.UsageMedia)
            .Build();

        private readonly Lazy<AudioFocusRequestClass> _audioFocusRequest;

        public AudioFocusExoPlayerDecorator(SimpleExoPlayer player, AudioManager audioManager, ILogger logger)
        {
            _audioFocusRequest = new Lazy<AudioFocusRequestClass>(
                () => new AudioFocusRequestClass.Builder(Android.Media.AudioFocus.Gain)
                    .SetAudioAttributes(_audioAttributes.Unwrap() as AudioAttributes)
                    .SetOnAudioFocusChangeListener(this)
                    .Build());

            _player = player;
            _audioManager = audioManager;
            _logger = logger;

            _eventListeners = new List<IPlayerEventListener>();

            _player.AddListener(this);
        }

        public bool PlayWhenReady
        {
            get => _player.PlayWhenReady || _shouldPlayWhenReady;
            set
            {
                if (value)
                {
                    RequestAudioFocus();
                }
                else
                {
                    if (_shouldPlayWhenReady)
                    {
                        _shouldPlayWhenReady = false;
                        OnPlayerStateChanged(false, _player.PlaybackState);
                    }
                    _player.PlayWhenReady = value;
                    AbandonAudioFocus();
                }
            }
        }

        private void RequestAudioFocus()
        {
            AudioFocusRequest result;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                result = _audioManager.RequestAudioFocus(_audioFocusRequest.Value);
            }
            else
            {
                #pragma warning disable 618
                result = _audioManager.RequestAudioFocus(this, Stream.Music, Android.Media.AudioFocus.Gain);
                #pragma warning restore 618
            }

            // Call the listener whenever focus is granted - even the first time!
            if (result == AudioFocusRequest.Granted)
            {
                _shouldPlayWhenReady = true;
                OnAudioFocusChange(Android.Media.AudioFocus.Gain);
            }
            else
            {
                _logger.Info("AudioFocusExoPlayerDecorator", "Playback not started: Audio focus request denied");
            }
        }

        private void AbandonAudioFocus()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                _audioManager.AbandonAudioFocusRequest(_audioFocusRequest.Value);
            }
            else
            {
                #pragma warning disable 618
                _audioManager.AbandonAudioFocus(this);
                #pragma warning restore 618
            }
        }

        public void OnAudioFocusChange(Android.Media.AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case Android.Media.AudioFocus.Gain:
                    if (_shouldPlayWhenReady || _player.PlayWhenReady)
                    {
                        _player.PlayWhenReady = true;
                        _player.Volume = MediaVolumeDefault;
                    }

                    _shouldPlayWhenReady = false;
                    break;
                case Android.Media.AudioFocus.LossTransientCanDuck:
                    // Audio focus was lost, but it's possible to duck (i.e. play quietly)
                    if (_player.PlayWhenReady)
                    {
                        _player.Volume = MediaVolumeDuck;
                    }
                    break;
                case Android.Media.AudioFocus.LossTransient:
                    // Lost audio focus, but will gain it back (shortly), so note whether playback should resume
                    _shouldPlayWhenReady = _player.PlayWhenReady;
                    _player.PlayWhenReady = false;
                    break;
                case Android.Media.AudioFocus.Loss:
                    // Lost audio focus, probably "permanently"
                    PlayWhenReady = false; // this will chain through to AbandonAudioFocus()
                    break;
                default:
                    _logger.Warn("AndroidMediaPlayerPlayback", "onAudioFocusChange: Ignoring unsupported focusChange: " + focusChange);
                    break;
            }
        }

        #region Default decorator implementations
        // This is not pretty and a lot of tedious, stupid code but allows for better decoupling.
        // The idea was originally copied from https://github.com/googlesamples/android-UniversalMusicPlayer/

        public int GetRendererType(int rendererType)
        {
            return _player.GetRendererType(rendererType);
        }

        public void Next()
        {
            _player.Next();
        }

        public void Previous()
        {
            _player.Previous();
        }

        public void Release()
        {
            _player.Release();
        }

        /// <summary>
        /// Triggered when seeking through the playback
        /// </summary>
        public void SeekTo(int p0, long p1)
        {
            Mvx.IoCProvider.Resolve<IMvxMessenger>().Publish(new PlaybackSeekedMessage(this)
            {
                CurrentPosition = CurrentPosition,
                SeekedPosition = GetCorrectSeekedPositionValue(p1)
            });

            _player.SeekTo(p0, p1);
        }

        /// <summary>
        /// Triggered when seeking through the playback
        /// </summary>
        public void SeekTo(long p0)
        {
            Mvx.IoCProvider.Resolve<IMvxMessenger>().Publish(new PlaybackSeekedMessage(this)
            {
                CurrentPosition = CurrentPosition,
                SeekedPosition = p0
            });

            _player.SeekTo(p0);
        }

        public void SeekToDefaultPosition()
        {
            _player.SeekToDefaultPosition();
        }

        public void SeekToDefaultPosition(int p0)
        {
            _player.SeekToDefaultPosition(p0);
        }

        public void Stop()
        {
            _player.Stop();
        }

        public void Stop(bool p0)
        {
            _player.Stop(p0);
        }

        public Looper ApplicationLooper => _player.ApplicationLooper;

        public IPlayerAudioComponent AudioComponent => _player.AudioComponent;

        public int BufferedPercentage => _player.BufferedPercentage;

        public long BufferedPosition => _player.BufferedPosition;

        public long ContentBufferedPosition => _player.ContentBufferedPosition;

        public long ContentDuration => _player.ContentDuration;

        public long ContentPosition => _player.ContentPosition;

        public int CurrentAdGroupIndex => _player.CurrentAdGroupIndex;

        public int CurrentAdIndexInAdGroup => _player.CurrentAdIndexInAdGroup;

        public Object CurrentManifest => _player.CurrentManifest;

        public int CurrentPeriodIndex => _player.CurrentPeriodIndex;

        public long CurrentPosition => _player.CurrentPosition;

        public Object CurrentTag => _player.CurrentTag;

        public Timeline CurrentTimeline => _player.CurrentTimeline;

        public TrackGroupArray CurrentTrackGroups => _player.CurrentTrackGroups;

        public TrackSelectionArray CurrentTrackSelections => _player.CurrentTrackSelections;

        public int CurrentWindowIndex => _player.CurrentWindowIndex;

        public long Duration => _player.Duration;

        public bool HasNext => _player.HasNext;

        public bool HasPrevious => _player.HasPrevious;

        public bool IsCurrentWindowDynamic => _player.IsCurrentWindowDynamic;

        public bool IsCurrentWindowLive => _player.IsCurrentWindowLive;

        public bool IsCurrentWindowSeekable => _player.IsCurrentWindowSeekable;

        public bool IsLoading => _player.IsLoading;

        public bool IsPlaying => _player.IsPlaying;

        public bool IsPlayingAd => _player.IsPlayingAd;

        public IPlayerMetadataComponent MetadataComponent => _player.MetadataComponent;

        public int NextWindowIndex => _player.NextWindowIndex;

        public ExoPlaybackException PlaybackError => _player.PlaybackError;

        public PlaybackParameters PlaybackParameters
        {
            get => _player.PlaybackParameters;
            set => _player.PlaybackParameters = value;
        }

        public int PlaybackState => _player.PlaybackState;

        public int PlaybackSuppressionReason => _player.PlaybackSuppressionReason;

        public int PreviousWindowIndex => _player.PreviousWindowIndex;

        public int RendererCount => _player.RendererCount;

        public int RepeatMode
        {
            get => _player.RepeatMode;
            set => _player.RepeatMode = value;
        }

        public bool ShuffleModeEnabled
        {
            get => _player.ShuffleModeEnabled;
            set => _player.ShuffleModeEnabled = value;
        }

        public IPlayerTextComponent TextComponent => _player.TextComponent;

        public long TotalBufferedDuration => _player.TotalBufferedDuration;

        public IPlayerVideoComponent VideoComponent => _player.VideoComponent;

        private static long GetCorrectSeekedPositionValue(long p1)
            => p1 < 0 ? 0 : p1;

        #endregion

        public void OnTracksChanged(TrackGroupArray trackGroups, TrackSelectionArray trackSelections)
        {
            // ToDo: read the cover from id3 tags to save us the extra download from the server: https://medium.com/google-exoplayer/exoplayer-2-1-whats-new-2832c09fedab

            PassEventThrough(l => l.OnTracksChanged(trackGroups, trackSelections));
        }

        #region IExoPlayer implementations

        public PlayerMessage CreateMessage(PlayerMessage.ITarget target)
        {
            return _player.CreateMessage(target);
        }

        public void Prepare(IMediaSource mediaSource)
        {
            _player.Prepare(mediaSource);
        }

        public void Prepare(IMediaSource mediaSource, bool resetPosition, bool resetState)
        {
            _player.Prepare(mediaSource, resetPosition, resetState);
        }

        public void Retry()
        {
            _player.Retry();
        }

        public void SetForegroundMode(bool foregroundMode)
        {
            _player.SetForegroundMode(foregroundMode);
        }

        public Looper PlaybackLooper => _player.PlaybackLooper;

        public SeekParameters SeekParameters
        {
            get => _player.SeekParameters;
            set => _player.SeekParameters = value;
        }

        #endregion

        public void AddListener(IPlayerEventListener listener)
        {
            if (listener != null && !_eventListeners.Contains(listener))
            {
                _eventListeners.Add(listener);
            }
        }

        public void RemoveListener(IPlayerEventListener listener)
        {
            if (listener != null && _eventListeners.Contains(listener))
            {
                _eventListeners.Remove(listener);
            }
        }

        /// <summary>
        /// Invoked when the value returned from either ExoPlayer.getPlayWhenReady() or ExoPlayer.getPlaybackState() changes.
        /// Also handles the case where the intention is to play (so [Player.getPlayWhenReady] should
        /// return `true`), but it's actually paused because the app had a temporary loss
        /// of audio focus; i.e.: [AudioManager.AUDIOFOCUS_LOSS_TRANSIENT].
        /// </summary>
        public void OnPlayerStateChanged(bool playWhenReady, int playbackState)
        {
            var reportPlayWhenReady = PlayWhenReady;
            PassEventThrough(l => l.OnPlayerStateChanged(reportPlayWhenReady, playbackState));

            if (playbackState == IPlayer.StateEnded)
            {
                // This is when a track finishes playing and is the last item in the queue
                // CurrentTag contains the new track that is playing. Not the one that just finished
                Mvx.IoCProvider.Resolve<IMvxMessenger>().Publish(new TrackCompletedMessage(this)
                {
                    NumberOfTracksInQueue = CurrentTimeline.WindowCount,
                    PlayStatus = PlayStatus.Ended
                });
            }
        }

        // This passes events through with the exception of OnPlayerStateChanged
        #region IPlayerEventListener implementation

        private void PassEventThrough(Action<IPlayerEventListener> action)
        {
            foreach (var eventListener in _eventListeners)
            {
                action(eventListener);
            }
        }

        public void OnLoadingChanged(bool isLoading)
        {
            PassEventThrough(listener => listener.OnLoadingChanged(isLoading));
        }

        public void OnPlaybackParametersChanged(PlaybackParameters playbackParameters)
        {
            PassEventThrough(l => l.OnPlaybackParametersChanged(playbackParameters));
        }

        public void OnPlayerError(ExoPlaybackException error)
        {
            PassEventThrough(l => l.OnPlayerError(error));
        }

        public void OnPositionDiscontinuity(int reason)
        {
            PassEventThrough(l => l.OnPositionDiscontinuity(reason));
            if (reason == IPlayer.DiscontinuityReasonPeriodTransition)
            {
                // This is when a track finishes playing by itself (with no user interaction involved)
                Mvx.IoCProvider.Resolve<IMvxMessenger>().Publish(new TrackCompletedMessage(this) { });
            }
        }

        public void OnRepeatModeChanged(int repeatMode)
        {
            PassEventThrough(l => l.OnRepeatModeChanged(repeatMode));
        }

        public void OnSeekProcessed()
        {
            PassEventThrough(l => l.OnSeekProcessed());
        }

        public void OnShuffleModeEnabledChanged(bool shuffleModeEnabled)
        {
            PassEventThrough(l => l.OnShuffleModeEnabledChanged(shuffleModeEnabled));
        }

        public void OnTimelineChanged(Timeline timeline, int reason)
        {
            PassEventThrough(l => l.OnTimelineChanged(timeline, reason));
        }

        public void OnIsPlayingChanged(bool isPlaying)
        {
            PassEventThrough(l => l.OnIsPlayingChanged(isPlaying));
        }

        public void OnPlaybackSuppressionReasonChanged(int playbackSuppressionReason)
        {
            PassEventThrough(l => l.OnPlaybackSuppressionReasonChanged(playbackSuppressionReason));
        }

        #endregion

    }
}
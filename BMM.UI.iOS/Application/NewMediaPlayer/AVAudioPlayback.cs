using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Analytics;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Constants;
using CoreMedia;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class AVAudioPlayback : AvAudioPlaybackBase, IAudioPlayback
    {
        // See https://nshipster.com/nserror/ for explanation of error codes
        // We might need to extend that list
        private readonly IList<nint> _internetProblemsErrorCodes = new List<nint> {-1009, -1018, -1019, -1020};

        private readonly IPlayerErrorHandler _playerErrorHandler;

        private readonly IPlayerAnalytics _playerAnalytics;
        private readonly AvPlayerItemFactory _playerItemFactory;

        private PlayStatus _status;

        private IMediaTrack _currentMediaTrack;
        private decimal? _desiredRate;

        private AVPlayerItem CurrentItem => Player.CurrentItem;

        public PlayStatus Status
        {
            get => _status;
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    OnStateChanged?.Invoke();
                }
            }
        }

        public decimal DesiredRate
        {
            get => _desiredRate ?? PlayerConstants.NormalPlaybackSpeed;
            set
            {
                _desiredRate = value;

                if (Status != PlayStatus.Playing)
                    return;
                
                Rate = (float)value;
                OnStateChanged?.Invoke();
            }
        }

        public float Rate
        {
            get
            {
                if (_currentMediaTrack?.IsLivePlayback == true && Status == PlayStatus.Playing)
                {
                    // HLS streams only return a proper rate for some playlists. See https://stackoverflow.com/questions/11811062/avplayer-rate-property-for-http-live-streaming
                    // Therefore we need to use this hack to return a proper rate
                    return 1.0f;
                }

                return Player?.Rate ?? 0.0f;
            }
            set
            {
                Player.IfNotNull(p => p.Rate = value);
            }
        }

        public long Position => CurrentItem?.CurrentTime.Milliseconds() ?? 0;

        // We currently rely on the value stored in the API. But that may not always be accurate. Maybe we should use this value instead.
        public TimeSpan Duration
        {
            get
            {
                if (CurrentItem == null || CurrentItem.Duration.IsIndefinite || CurrentItem.Duration.IsInvalid)
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(CurrentItem.Duration.Seconds);
            }
        }

        public long Buffered
        {
            get
            {
                long buffered = 0;

                var loadedTimeRanges = CurrentItem?.LoadedTimeRanges;
                if (loadedTimeRanges != null && loadedTimeRanges.Any())
                {
                    var loadedSegments = loadedTimeRanges
                        .Select(timeRange =>
                        {
                            var timeRangeValue = timeRange.CMTimeRangeValue;

                            var start = timeRangeValue.Start.Milliseconds();
                            var duration = timeRangeValue.Duration.Milliseconds();

                            return start + duration;
                        });

                    buffered = loadedSegments.Max();
                }

                return buffered;
            }
        }

        private const int IncompletePlaybackThresholdInSeconds = 2;

        public AVAudioPlayback(IPlayerErrorHandler playerErrorHandler, IPlayerAnalytics playerAnalytics, AvPlayerItemFactory playerItemFactory)
        {
            _playerErrorHandler = playerErrorHandler;
            _playerAnalytics = playerAnalytics;
            _playerItemFactory = playerItemFactory;

            NSNotificationCenter.DefaultCenter.AddObserver(
                AVPlayerItem.DidPlayToEndTimeNotification,
                notification =>
                {
                    var positionInSeconds = Position / 1000;
                    // In case the track is only partially downloaded playback just stops without any way to detect that something is wrong. Therefore we have to detect it manually.
                    if (CurrentItem.Duration.Seconds - positionInSeconds > IncompletePlaybackThresholdInSeconds)
                    {
                        Status = PlayStatus.Stopped;
                        _playerErrorHandler.PlaybackError($"Track stopped too early. Duration: {CurrentItem.Duration.Seconds}, Position: {positionInSeconds}", _currentMediaTrack);
                        return;
                    }

                    _status = PlayStatus.Ended; // We want to bypass the OnPositionOrStateChanged() because OnMediaFinished indirectly also triggers it
                    OnMediaFinished?.Invoke();
                });
        }

        public void SeekTo(long newPositionInMs, bool playAutomatically = true)
        {
            CurrentItem?.Seek(CMTime.FromSeconds(newPositionInMs / 1000.0, 10), finished =>
            {
                if (playAutomatically && (Status == PlayStatus.Paused || Status == PlayStatus.Ended))
                {
                    _status = PlayStatus.Playing;
                    PlayAndSetRate();
                }

                OnStateChanged?.Invoke();
            });
        }

        public async Task Play(IMediaTrack mediaTrack = null)
        {
            var sameMediaTrack = mediaTrack == null || mediaTrack.Equals(_currentMediaTrack);

            if (Status == PlayStatus.Paused && sameMediaTrack && !_currentMediaTrack.IsLivePlayback)
            {
                // Live playback should be reinitialized since otherwise we don't detect if the transmission has been stopped in the meantime.

                Status = PlayStatus.Playing;
                // We are simply paused so just start again
                PlayAndSetRate();
                return;
            }
            if (sameMediaTrack && Status == PlayStatus.Ended)
            {
                SeekTo(0);
                return;
            }

            if (mediaTrack != null)
            {
                _currentMediaTrack = mediaTrack;
            }


            _playerAnalytics.LogIfDownloadedTrackHasDifferentAttributesThanTrackFromTheApi(_currentMediaTrack);

            try
            {
                _playerAnalytics.TrackPlaybackRequested(_currentMediaTrack);

                Status = PlayStatus.Buffering;

                RemoveObservers();

                // ToDo: is it really necessary to re-initialize the player?
                InitializePlayer();

                var playerItem = await _playerItemFactory.Create(_currentMediaTrack);

                Player.ReplaceCurrentItemWithPlayerItem(playerItem);
                AttachObservers();
                CurrentItem.SeekingWaitsForVideoCompositionRendering = true;

                PlayAndSetRate();
            }
            catch (Exception ex)
            {
                Status = PlayStatus.Stopped;
                _playerErrorHandler.StartError(ex);
            }
        }

        public async Task LoadTrackToPlay(IMediaTrack mediaTrack = null)
        {
            if (mediaTrack != null)
                _currentMediaTrack = mediaTrack;

            InitializePlayer();
            var playerItem = await _playerItemFactory.Create(_currentMediaTrack);
            Status = PlayStatus.Paused;
            Player.ReplaceCurrentItemWithPlayerItem(playerItem);
            AttachObservers();
        }
        
        private void PlayAndSetRate()
        {
            Player.Play();
            Rate = (float)DesiredRate;
        }

        private void AttachObservers()
        {
            CurrentItem.AddObserver(this, LoadedTimeRangesObserver, InitialAndNewObservingOptions, LoadedTimeRangesObservationContext.Handle);
            CurrentItem.AddObserver(this, StatusObserver, InitialAndNewObservingOptions, StatusObservationContext.Handle);
        }

        private void RemoveObservers()
        {
            CurrentItem?.RemoveObserver(this, StatusObserver, StatusObservationContext.Handle);
            CurrentItem?.RemoveObserver(this, LoadedTimeRangesObserver, LoadedTimeRangesObservationContext.Handle);
        }

        public async Task PlayPause()
        {
            if (Status == PlayStatus.Playing || Status == PlayStatus.Buffering || Status == PlayStatus.Loading)
            {
                Pause();
            }
            else
            {
                await Play();
            }
        }

        public void Stop()
        {
            CurrentItem?.RemoveObserver(this, StatusObserver, StatusObservationContext.Handle);
            CurrentItem?.RemoveObserver(this, LoadedTimeRangesObserver, LoadedTimeRangesObservationContext.Handle);

            AVAudioSession.SharedInstance().SetActive(false, AVAudioSessionSetActiveOptions.NotifyOthersOnDeactivation);

            DeinitializePlayer();

            Status = PlayStatus.Stopped;
        }

        public Action OnMediaFinished { get; set; }

        public Action<long, long> OnPositionChanged { private get; set; }

        public Action OnStateChanged { private get; set; }

        public void Pause()
        {
            Status = PlayStatus.Paused;

            if (CurrentItem == null)
                return;
            if (Player.Rate != 0.0)
                Player.Pause();
        }

        public override void ObserveStatus()
        {
            var isBuffering = Status == PlayStatus.Buffering;

            if (CurrentItem.Status == AVPlayerItemStatus.ReadyToPlay && isBuffering)
            {
                Status = PlayStatus.Playing;
                PlayAndSetRate();
                _playerAnalytics.TrackPlaybackStarted(_currentMediaTrack);
            }
            else if (CurrentItem.Status == AVPlayerItemStatus.Failed)
            {
                Status = PlayStatus.Stopped;
                var error = CurrentItem.Error;
                var technicalMessage = $"{error.Code} - {error.Domain} - {error.LocalizedDescription}";

                if (error.Code == -11866 && _currentMediaTrack.IsLivePlayback)
                    _playerErrorHandler.LiveRadioStopped();
                else if (_internetProblemsErrorCodes.Contains(error.Code))
                    _playerErrorHandler.InternetProblems(technicalMessage, error.LocalizedDescription);
                else
                    _playerErrorHandler.PlaybackError(technicalMessage, _currentMediaTrack, userReadableMessage: error.LocalizedDescription);
            }
        }

        public override void ObserveRate()
        {
            var stoppedPlaying = Player.Rate == 0.0;

            if (stoppedPlaying && Status == PlayStatus.Playing)
            {
                //Update the status because the system changed the rate.
                Status = PlayStatus.Paused;
            }
        }

        public override void ObserveLoadedTimeRanges()
        {
            OnPositionChanged?.Invoke(Position, Buffered);
        }

        public override void ObservePeriodicTimeEvent(CMTime obj)
        {
            OnPositionChanged?.Invoke(Position, Buffered);
        }
    }

    public static class CmTimeHelper
    {
        public static long Milliseconds(this CMTime time)
        {
            return Convert.ToInt64(time.Seconds * 1000);
        }
    }
}
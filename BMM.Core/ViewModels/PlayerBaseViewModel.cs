using BMM.Core.ViewModels.Base;
using System;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Utils;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels
{
    public class PlayerBaseViewModel : BaseViewModel
    {
        private const int CurrentTrackUpdateDebounceDelayInMillis = 100;

        protected readonly IMediaPlayer MediaPlayer;

        private MvxSubscriptionToken _updatePositionToken;
        private MvxSubscriptionToken _updateStateToken;
        private MvxSubscriptionToken _updateMetadataToken;
        
        private readonly DebounceDispatcher _currentTrackDebounceDispatcher;

        private ITrackModel _currentTrack;
        
        public MvxCommand PlayPauseCommand { get; }
        
        /// <summary>
        /// When changing the song on Android, exo player sends few current track updates, where few of them is just null.
        ///
        /// Exemplary sequence looks like that:
        /// Current Song => (next button clicked) => null => Current Song => null => null => Next Song
        ///
        /// This is changing in very short time, so adding debouncer with small slippage prevents plaver view from flickering,
        /// as finally we react for only one update.
        /// </summary>
        public ITrackModel CurrentTrack
        {
            get => _currentTrack;
            private set
            {
                _currentTrackDebounceDispatcher.Run(() =>
                {
                    SetProperty(ref _currentTrack, value);
                    RaisePropertyChanged(() => Downloaded); // Since Downloaded depends on CurrentTrack we need to raise PropertyChanged
                    RaisePropertyChanged(() => IsSeekingDisabled);
                    OnCurrentTrackChanged();
                });
            }
        }

        /// <summary>
        /// For a live playback it is not possible to skip ahead or backwards.
        /// In Theory it's possible to implement it but it turns out to be quite difficult to implement because
        /// - it's hard to get good information about the current position
        /// - BTV only provides the data for a short time window. I don't know how to get details about that time window.
        /// </summary>
        public bool IsSeekingDisabled => CurrentTrack.IsLivePlayback;

        private long _duration;
        public long Duration
        {
            get => _duration;
            private set => SetProperty(ref _duration, value);
        }

        private bool _isSeeking;

        public bool IsSeeking
        {
            get => _isSeeking;
            set
            {
                // ToDo: instead of using a setter create a Method FinishSeeking that takes care of that.

                // Put into an action so we can await the seek-command before we update the value. Prevents jumping of the progress-bar.
                var a = new Action(() =>
                    {
                        // When disable user-seeking, update the position with the position-value
                        if (value == false)
                        {
                            MediaPlayer.SeekTo(SliderPosition);
                        }

                        _isSeeking = value;
                    });
                a.Invoke();
            }
        }

        private long _sliderPosition;
        public long SliderPosition
        {
            get => IsSeeking ? _sliderPosition : _currentPosition;
            set => SetProperty(ref _sliderPosition, value);
        }

        private long _currentPosition;
        public long CurrentPosition
        {
            get => _currentPosition;
            set
            {
                SetProperty(ref _currentPosition, value);
                RaisePropertyChanged(() => SliderPosition);
            }
        }

        private long _downloaded;
        public long Downloaded
        {
            get => CurrentTrack?.Availability == ResourceAvailability.Local ? Duration : _downloaded;
            private set => SetProperty(ref _downloaded, value);
        }

        #region PlaybackState

        private bool _isPlaying;

        public bool IsPlaying
        {
            get => _isPlaying;
            private set => SetProperty(ref _isPlaying, value);
        }

        #endregion

        public readonly ITrackInfoProvider TrackInfoProvider = new PlayerTrackInfoProvider();

        // Caution: On Android this ViewModel lives the whole time, whereas iOS creates a new instance every time the player is opened
        public PlayerBaseViewModel(IMediaPlayer mediaPlayer)
        {
            _currentTrackDebounceDispatcher = new DebounceDispatcher(CurrentTrackUpdateDebounceDelayInMillis);
            
            MediaPlayer = mediaPlayer;

            PlayPauseCommand = new MvxCommand(MediaPlayer.PlayPause);

            CurrentTrack = MediaPlayer.CurrentTrack;
            Duration = MediaPlayer.CurrentTrack?.Duration ?? 0;
        }

        protected void SetupSubscriptions()
        {
            _updateStateToken = Messenger.Subscribe<PlaybackStatusChangedMessage>(
                msg => { UpdatePlaybackState(msg.PlaybackState); });
            _updatePositionToken = Messenger.Subscribe<PlaybackPositionChangedMessage>(message =>
            {
                CurrentPosition = message.CurrentPosition;
                Downloaded = message.BufferedPosition;
            });
            _updateMetadataToken = Messenger.Subscribe<CurrentTrackChangedMessage>(async message =>
            {
                CurrentTrack = message.CurrentTrack;
                Duration = message.CurrentTrack?.Duration ?? 0;
            });
        }

        protected virtual Task OnCurrentTrackChanged() => Task.CompletedTask;
        
        protected virtual void UpdatePlaybackState(IPlaybackState state)
        {
            IsPlaying = state.IsPlaying;
            CurrentPosition = state.CurrentPosition;
            Downloaded = state.BufferedPosition;
        }
    }
}
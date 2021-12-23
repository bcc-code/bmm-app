using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.LiveRadio;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Player
{
    public class ViewModelHandlingMediaPlayerDecorator : IMediaPlayer
    {
        private const double JumpStepsInSeconds = 15d;

        private readonly MvxSubscriptionToken _token;

        private readonly IDeviceInfo _deviceInfo;
        private readonly IMvxNavigationService _navigationService;
        private readonly IPlatformSpecificMediaPlayer _mediaPlayer;
        private readonly IMediaQueue _queue;

        private readonly IMediaPlayerInitializer _mediaPlayerInitializer;
        private readonly IPlayerErrorHandler _playerErrorHandler;

        private readonly ILiveTime _liveTime;
        private readonly IMvxMessenger _mvxMessenger;

        private bool _isViewmodelShown;

        public ViewModelHandlingMediaPlayerDecorator(
            IDeviceInfo deviceInfo,
            IMvxNavigationService navigationService,
            IPlatformSpecificMediaPlayer mediaPlayer,
            IMediaQueue queue,
            IMediaPlayerInitializer mediaPlayerInitializer,
            IPlayerErrorHandler playerErrorHandler,
            ILiveTime liveTime,
            IMvxMessenger mvxMessenger)
        {
            _deviceInfo = deviceInfo;
            _navigationService = navigationService;
            _mediaPlayer = mediaPlayer;
            _queue = queue;
            _mediaPlayerInitializer = mediaPlayerInitializer;
            _playerErrorHandler = playerErrorHandler;
            _liveTime = liveTime;
            _mvxMessenger = mvxMessenger;

            _mediaPlayer.ContinuingPreviousSession = () => { ShowViewmodelIfNecessary(); };
        }

        private void ShowViewmodelIfNecessary(bool showPlayer = false)
        {
            if (_isViewmodelShown || _queue.Tracks.Count == 0)
            {
                return;
            }

            _navigationService.Navigate<MiniPlayerViewModel>();

            if (_deviceInfo.IsAndroid)
            {
                _navigationService.Navigate<PlayerViewModel, bool>(showPlayer);
            }

            _mediaPlayerInitializer.Initialize();

            _isViewmodelShown = true;
        }

        private void HideViewmodelIfNecessary()
        {
            if (!_isViewmodelShown)
                return;

            _mediaPlayerInitializer.Deinitialize();

            // we don't hide miniplayer because logout does it
            // mark view model as hidden so that iOS can open player after login
            _isViewmodelShown = false;
        }

        public async Task Play(IList<IMediaTrack> mediaFiles, IMediaTrack currentTrack, long startTimeInMs = 0)
        {
            await ExecuteWithUpdatingQueue(
                mediaFiles,
                currentTrack,
                () => _mediaPlayer.Play(mediaFiles, currentTrack, startTimeInMs));
        }

        public async Task RecoverQueue(IList<IMediaTrack> mediaTracks, IMediaTrack currentTrack, long startTimeInMs = 0)
        {
            await ExecuteWithUpdatingQueue(
                mediaTracks,
                currentTrack,
                () => _mediaPlayer.RecoverQueue(mediaTracks, currentTrack, startTimeInMs));
        }

        private async Task ExecuteWithUpdatingQueue(
            IList<IMediaTrack> mediaTracks,
            IMediaTrack currentTrack,
            Func<Task> taskToExecute)
        {
            if (!ShowErrorIfOutdatedLiveRadio(currentTrack))
            {
                _mvxMessenger.Publish(new CurrentQueueChangedMessage(mediaTracks, this));
                await taskToExecute.Invoke();
                ShowViewmodelIfNecessary();
            }
        }

        public Task Play(IList<IMediaTrack> mediaTracks, IMediaTrack currentTrack, string playbackOrigin, long startTimeInMs = 0)
        {
            var enrichedTracks = EnrichTracksWithPlaybackOrigin(mediaTracks, playbackOrigin);
            return Play(enrichedTracks, currentTrack, startTimeInMs);
        }

        public void ViewHasBeenDestroyed(bool themeChanged)
        {
            if (themeChanged)
                return;

            _isViewmodelShown = false;
        }

        public void ToggleShuffle()
        {
            _mediaPlayer.SetShuffle(!IsShuffleEnabled);
            ShowViewmodelIfNecessary();
        }

        public void JumpForward()
        {
            SkipForward(JumpStepsInSeconds);
        }

        public void JumpBackward()
        {
            SkipBackward(JumpStepsInSeconds);
        }

        public void SkipForward(double timeInSeconds)
        {
            var newPosition = PlaybackState.CurrentPosition + timeInSeconds * 1000;
            SeekTo((long)newPosition);
        }

        public void SkipBackward(double timeInSeconds)
        {
            var newPosition = PlaybackState.CurrentPosition - timeInSeconds * 1000;
            SeekTo((long)newPosition);
        }

        public void ToggleRepeatType()
        {
            RepeatType newRepeat = RepeatType.None;
            if (RepeatType == RepeatType.None)
                newRepeat = RepeatType.RepeatAll;
            else if (RepeatType == RepeatType.RepeatAll)
                newRepeat = RepeatType.RepeatOne;

            _mediaPlayer.SetRepeatType(newRepeat);
            ShowViewmodelIfNecessary();
        }

        public void Play()
        {
            if (!_mediaPlayer.IsPlaying)
                PlayPause();
        }

        public void Pause()
        {
            if (_mediaPlayer.IsPlaying)
                PlayPause();
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
            HideViewmodelIfNecessary();
            _queue.Clear();
        }

        public Task<bool> AddToEndOfQueue(IMediaTrack track, string playbackOrigin)
        {
            var enrichedTrack = EnrichTrackWithPlaybackOrigin(track, playbackOrigin);
            return QueueAndShowPlayer(t => _mediaPlayer.AddToEndOfQueue(t, playbackOrigin), enrichedTrack);
        }

        public Task<bool> QueueToPlayNext(IMediaTrack track, string playbackOrigin)
        {
            var enrichedTrack = EnrichTrackWithPlaybackOrigin(track, playbackOrigin);
            return QueueAndShowPlayer(t => _mediaPlayer.QueueToPlayNext(t, playbackOrigin), enrichedTrack);
        }

        private async Task<bool> QueueAndShowPlayer(Func<IMediaTrack, Task<bool>> queueFunc, IMediaTrack track)
        {
            if (_queue.Tracks.Any())
            {
                var result = await queueFunc(track);
                ShowViewmodelIfNecessary();
                return result;
            }

            await Play(new List<IMediaTrack> {track}, track);
            ShowViewmodelIfNecessary();

            return true;
        }

        public void PlayPause()
        {
            if (!ShowErrorIfOutdatedLiveRadio())
            {
                _mediaPlayer.PlayPause();
                ShowViewmodelIfNecessary();
            }
        }

        #region simply Pass action through

        public void PlayNext()
        {
            _mediaPlayer.PlayNext();
            ShowViewmodelIfNecessary();
        }

        public void PlayPrevious()
        {
            _mediaPlayer.PlayPrevious();
            ShowViewmodelIfNecessary();
        }

        public void PlayPreviousOrSeekToStart()
        {
            _mediaPlayer.PlayPreviousOrSeekToStart();
            ShowViewmodelIfNecessary();
        }

        public void SeekTo(long newPosition)
        {
            _mediaPlayer.SeekTo(newPosition);
            ShowViewmodelIfNecessary();
        }

        public async Task ShuffleList(IList<IMediaTrack> tracks, string playbackOrigin)
        {
            var enrichedTracks = EnrichTracksWithPlaybackOrigin(tracks, playbackOrigin);
            await _mediaPlayer.ShuffleList(enrichedTracks, playbackOrigin);
            ShowViewmodelIfNecessary();
        }

        public ITrackModel CurrentTrack => _mediaPlayer.CurrentTrack;

        public bool IsPlaying => _mediaPlayer.IsPlaying;

        public RepeatType RepeatType => _mediaPlayer.RepeatType;

        public bool IsShuffleEnabled => _mediaPlayer.IsShuffleEnabled;

        public IPlaybackState PlaybackState => _mediaPlayer.PlaybackState;

        #endregion

        public bool ShowErrorIfOutdatedLiveRadio(ITrackModel trackToPlay = null)
        {
            var track = trackToPlay ?? CurrentTrack;
            if (track?.IsLivePlayback == true && (trackToPlay != null || !IsPlaying) && track.RecordedAt != DateTime.MinValue)
            {
                var broadcastEnd = track.RecordedAt.AddMilliseconds(track.Duration);
                if (_liveTime.TimeOnServer > broadcastEnd)
                {
                    _playerErrorHandler.LiveRadioStopped();
                    return true;
                }
            }

            return false;
        }

        private IMediaTrack EnrichTrackWithPlaybackOrigin(IMediaTrack track, string playbackOrigin)
        {
            track.PlaybackOrigin = playbackOrigin;
            return track;
        }

        private IList<IMediaTrack> EnrichTracksWithPlaybackOrigin(IList<IMediaTrack> tracks, string playbackOrigin)
        {
            var enrichedTracks = new List<IMediaTrack>();

            foreach (var track in tracks)
            {
                if (track != null)
                {
                    EnrichTrackWithPlaybackOrigin(track, playbackOrigin);
                    enrichedTracks.Add(track);
                }
            }

            return enrichedTracks;
        }
    }
}
using BMM.Api.Abstraction;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Messages.MediaPlayer;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.TrackListenedObservation
{
    public class TrackListenedObserver
    {
        private const int WhenTrackIsListenedInMilliseconds = 20_000;
        private readonly IListenedTracksStorage _tracksStorage;
        private readonly IExceptionHandler _exceptionHandler;
        private MvxSubscriptionToken _currentTrackChangedToken;
        private MvxSubscriptionToken _playbackPositionChangedToken;
        private ITrackModel _currentTrack;
        private bool _currentTrackIsCheckedForSaving;

        public TrackListenedObserver(IMvxMessenger messenger, IListenedTracksStorage tracksStorage, IExceptionHandler exceptionHandler)
        {
            _tracksStorage = tracksStorage;
            _exceptionHandler = exceptionHandler;
            _currentTrackChangedToken = messenger.Subscribe<CurrentTrackChangedMessage>(OnCurrentTrackChanged);
            _playbackPositionChangedToken = messenger.Subscribe<PlaybackPositionChangedMessage>(OnPlaybackPositionChanged);
        }

        private void OnPlaybackPositionChanged(PlaybackPositionChangedMessage message)
        {
            var millisecondsListened = message.CurrentPosition;
            if (!ShouldSaveTrackAsListened(millisecondsListened))
                return;

            _exceptionHandler.FireAndForgetWithoutUserMessages(() => _tracksStorage.AddTrackToListenedTracks(_currentTrack));
            _currentTrackIsCheckedForSaving = true;
        }

        private void OnCurrentTrackChanged(CurrentTrackChangedMessage message)
        {
            _currentTrack = message.CurrentTrack;
            _currentTrackIsCheckedForSaving = false;
        }

        private bool ShouldSaveTrackAsListened(long millisecondsListened)
        {
            return !CurrentTrackCheckedForSaving() && CurrentTrackIsTreatedAsListened(millisecondsListened);
        }

        private bool CurrentTrackCheckedForSaving()
        {
            return _currentTrackIsCheckedForSaving;
        }

        private bool CurrentTrackIsTreatedAsListened(long millisecondsListened)
        {
            if (_currentTrack == null)
                return false;

            return millisecondsListened >= WhenTrackIsListenedInMilliseconds;
        }
    }
}
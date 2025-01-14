using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Implementations.TrackListenedObservation;
using BMM.Core.Models.Enums;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Commands;

namespace BMM.Core.Implementations.Factories.Tracks
{
    public class TrackPOFactory : ITrackPOFactory
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IStorageManager _storageManager;
        private readonly IConnection _connection;
        private readonly IDownloadQueue _downloadQueue;
        private readonly IShowTrackInfoAction _showTrackInfoAction;
        private readonly IListenedTracksStorage _listenedTracksStorage;
        private readonly IFirebaseRemoteConfig _config;
        private readonly IPlayNextAction _playNextAction;
        private readonly IAddToPlaylistAction _addToPlaylistAction;

        public TrackPOFactory(
            IMediaPlayer mediaPlayer,
            IStorageManager storageManager,
            IConnection connection,
            IDownloadQueue downloadQueue,
            IShowTrackInfoAction showTrackInfoAction,
            IListenedTracksStorage listenedTracksStorage,
            IFirebaseRemoteConfig config,
            IPlayNextAction playNextAction,
            IAddToPlaylistAction addToPlaylistAction)
        {
            _mediaPlayer = mediaPlayer;
            _storageManager = storageManager;
            _connection = connection;
            _downloadQueue = downloadQueue;
            _showTrackInfoAction = showTrackInfoAction;
            _listenedTracksStorage = listenedTracksStorage;
            _config = config;
            _playNextAction = playNextAction;
            _addToPlaylistAction = addToPlaylistAction;
        }
        
        public ITrackPO Create(
            ITrackInfoProvider trackInfoProvider,
            IMvxAsyncCommand<Document> optionsClickedCommand,
            Track track,
            TrackSwipeType trackSwipeType = TrackSwipeType.PlayNextAndAddToPlaylist)
        {
            if (track == null)
                return null;

            return new TrackPO(
                trackSwipeType,
                _mediaPlayer,
                _storageManager,
                _connection,
                _downloadQueue,
                _showTrackInfoAction,
                optionsClickedCommand,
                trackInfoProvider,
                _listenedTracksStorage,
                track,
                _config,
                _playNextAction,
                _addToPlaylistAction);
        }
    }
}
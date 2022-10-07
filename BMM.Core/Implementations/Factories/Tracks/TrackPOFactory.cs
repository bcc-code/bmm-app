using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Tracks;
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

        public TrackPOFactory(
            IMediaPlayer mediaPlayer,
            IStorageManager storageManager,
            IConnection connection,
            IDownloadQueue downloadQueue,
            IShowTrackInfoAction showTrackInfoAction)
        {
            _mediaPlayer = mediaPlayer;
            _storageManager = storageManager;
            _connection = connection;
            _downloadQueue = downloadQueue;
            _showTrackInfoAction = showTrackInfoAction;
        }
        
        public TrackPO Create(
            ITrackInfoProvider trackInfoProvider,
            
            IMvxAsyncCommand<Document> optionsClickedCommand,
            Track track)
        {
            if (track == null)
                return null;
            
            return new TrackPO(
                _mediaPlayer,
                _storageManager,
                _connection,
                _downloadQueue,
                _showTrackInfoAction,
                optionsClickedCommand,
                trackInfoProvider,
                track);
        }
    }
}
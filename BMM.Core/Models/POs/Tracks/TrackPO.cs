using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Implementations.TrackListenedObservation;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Tracks
{
    public class TrackPO 
        : DocumentPO,
          ITrackPO
    {
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IStorageManager _storageManager;
        private readonly IConnection _connection;
        private readonly IDownloadQueue _downloadQueue;
        private readonly ITrackInfoProvider _trackInfoProvider;
        private readonly IListenedTracksStorage _listenedTracksStorage;
        private readonly IFirebaseRemoteConfig _config;
        private TrackState _trackState;
        private string _trackSubtitle;
        private string _trackTitle;
        private string _trackMeta;

        public TrackPO(
            IMediaPlayer mediaPlayer,
            IStorageManager storageManager,
            IConnection connection,
            IDownloadQueue downloadQueue,
            IShowTrackInfoAction showTrackInfoAction,
            IMvxAsyncCommand<Document> optionsClickedCommand,
            ITrackInfoProvider trackInfoProvider,
            IListenedTracksStorage listenedTracksStorage,
            Track track,
            IFirebaseRemoteConfig config) : base(track)
        {
            Track = track;
            _config = config;
            _mediaPlayer = mediaPlayer;
            _storageManager = storageManager;
            _connection = connection;
            _downloadQueue = downloadQueue;
            _trackInfoProvider = trackInfoProvider;
            _listenedTracksStorage = listenedTracksStorage;

            ShowTrackInfoCommand = new MvxAsyncCommand(async () =>
            {
                await showTrackInfoAction.ExecuteGuarded(Track);
            });
            
            OptionButtonClickedCommand = new MvxAsyncCommand(async () =>
            {
                await optionsClickedCommand.ExecuteAsync(Track);
            });

            DeleteFromQueueCommand = new ExceptionHandlingCommand(async () =>
            {
                await mediaPlayer.DeleteFromQueue(track);
            });
            
            RefreshState().FireAndForget();
            SetTrackInformation();
        }

        public IMvxAsyncCommand ShowTrackInfoCommand { get; }
        public IMvxAsyncCommand OptionButtonClickedCommand { get; }
        public IMvxAsyncCommand DeleteFromQueueCommand { get; }

        private void SetTrackInformation()
        {
            var info = _trackInfoProvider.GetTrackInformation(Track, CultureInfo.CurrentUICulture);
            TrackTitle = info.Label;
            TrackSubtitle = info.Subtitle;
            TrackMeta = info.Meta;
        }

        public async Task RefreshState()
        {
            bool isCurrentlySelected = _mediaPlayer.CurrentTrack != null && _mediaPlayer.CurrentTrack.Id.Equals(Id);
            bool isDownloaded = _storageManager.SelectedStorage.IsDownloaded(Track);
            bool isAvailable = _connection.GetStatus() == ConnectionStatus.Online || isDownloaded;
            bool isDownloading = _downloadQueue.IsDownloading(Track);
            bool isQueued = _downloadQueue.IsQueued(Track);
            bool isListened = await _listenedTracksStorage.TrackIsListened(Track);
            bool isSong = Track.Subtype == TrackSubType.Song || Track.Subtype == TrackSubType.Singsong;
            bool showBlueDot = !isListened && (TrackIsTeaserPodcast() ||
                                               (isSong && _config.ShowBlueDotForSongs) ||
                                               (!isSong && _config.ShowBlueDotForMessages));
            
            TrackState = new TrackState(isCurrentlySelected, isAvailable, isDownloaded, isDownloading, isQueued, showBlueDot, Track.HasListened);
        }

        public Track Track { get; }

        public string TrackTitle
        {
            get => _trackTitle;
            set => SetProperty(ref _trackTitle, value);
        }
        
        public string TrackSubtitle
        {
            get => _trackSubtitle;
            set => SetProperty(ref _trackSubtitle, value);
        }
        
        public string TrackMeta
        {
            get => _trackMeta;
            set => SetProperty(ref _trackMeta, value);
        }
        
        public TrackState TrackState
        {
            get => _trackState;
            set => SetProperty(ref _trackState, value);
        }
        
        private bool TrackIsTeaserPodcast()
        {
            return Track.Tags.Contains(PodcastsConstants.FromKaareTagName) ||
                   Track.Tags.Contains(PodcastsConstants.ForbildeTagName) ||
                   Track.Tags.Contains(PodcastsConstants.RomanPodcastTagName) ||
                   Track.Tags.Contains(PodcastsConstants.GibraltarPodcastTagName) ||
                   Track.Tags.Contains(AslaksenConstants.AsklaksenTagName) ||
                   Track.Tags.Contains(AslaksenConstants.HebrewTagName);
        }
    }

    public class TrackState
    {
        public TrackState(
            bool isCurrentlySelected,
            bool isAvailable,
            bool isDownloaded,
            bool isDownloading,
            bool isQueued,
            bool showBlueDot,
            bool isListened)
        {
            IsCurrentlySelected = isCurrentlySelected;
            IsAvailable = isAvailable;
            IsDownloaded = isDownloaded;
            IsDownloading = isDownloading;
            IsQueued = isQueued;
            ShowBlueDot = showBlueDot;
            IsListened = isListened;
        }
        
        public bool IsCurrentlySelected { get; } 
        public bool IsAvailable { get; }
        public bool IsDownloaded { get; }
        public bool IsDownloading { get; }
        public bool IsQueued { get; }
        public bool ShowBlueDot { get; }
        public bool IsListened { get; }

        public bool IsDownloadingVisible => IsDownloading && !IsListened;
        public bool IsDownloadedVisible => IsDownloaded && !IsListened;
        public bool IsQueuedVisible => IsQueued && !IsListened;
    }
}
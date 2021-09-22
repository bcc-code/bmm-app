using System.Collections.Generic;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using BMM.Core.Implementations.UI;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.NewMediaPlayer.Notification;
using BMM.UI.Droid.Application.NewMediaPlayer.Service;
using MvvmCross.Plugin.Messenger;
using MediaControllerCompat = Android.Support.V4.Media.Session.MediaControllerCompat;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Controller
{
    public class MediaControllerCallback : MediaControllerCompat.Callback
    {
        private readonly IMvxMessenger _messenger;
        private readonly IMetadataMapper _metadataMapper;
        private readonly PlaybackStateCompatMapper _mapper;
        private readonly IMediaQueue _mediaQueue;
        private readonly ILogger _logger;
        private readonly IToastDisplayer _toastDisplayer;
        private readonly IPlayerErrorHandler _playerErrorHandler;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IPlayerAnalytics _playerAnalytics;
        private const int TrackFinishedMarginInMs = 2000;

        // stores which track is being played and might be inaccurate. It's only used to report to analytics.
        private ITrackModel _latestTrackModel;

        public MediaControllerCallback(IMvxMessenger messenger, IMetadataMapper metadataMapper, PlaybackStateCompatMapper mapper, IMediaQueue mediaQueue,
            IPlayerAnalytics playerAnalytics, ILogger logger, IToastDisplayer toastDisplayer, IPlayerErrorHandler playerErrorHandler)
        {
            _messenger = messenger;
            _metadataMapper = metadataMapper;
            _mapper = mapper;
            _mediaQueue = mediaQueue;
            _playerAnalytics = playerAnalytics;
            _logger = logger;
            _toastDisplayer = toastDisplayer;
            _playerErrorHandler = playerErrorHandler;
        }

        public override void OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue)
        {
        }

        public override void OnRepeatModeChanged(int repeatMode)
        {
            _messenger.Publish(new RepeatModeChangedMessage(this) { RepeatType = _mapper.ConvertRepeatMode(repeatMode) });
        }

        public override void OnShuffleModeChanged(int shuffleMode)
        {
            _messenger.Publish(new ShuffleModeChangedMessage(this) { IsShuffleEnabled = shuffleMode != PlaybackStateCompat.ShuffleModeNone });
        }

        public override void OnPlaybackStateChanged(PlaybackStateCompat state)
        {
            if ((state.State == PlaybackStateCompat.StateStopped || state.State == PlaybackStateCompat.StatePaused) && _latestTrackModel != null &&
                state.Position > _latestTrackModel.Duration - TrackFinishedMarginInMs)
            {
                var completedEvent = new TrackCompletedMessage(this)
                {
                    NumberOfTracksInQueue = _mediaQueue.Tracks.Count,
                    PlayStatus = PlayStatus.Ended
                };
                _messenger.Publish(completedEvent);
            }

            _messenger.Publish(new PlaybackStatusChangedMessage(this)
            {
                PlaybackState = state.ToPlaybackState(_mediaQueue)
            });

            if (state.ErrorMessage != null)
            {
                var technicalMessage = $"{state.ErrorCode} - {state.ErrorMessage}";
                if (state.ErrorCode == CustomErrorMessageProvider.ErrorInternetProblems)
                {
                    _playerErrorHandler.InternetProblems(technicalMessage);
                }
                else if (state.ErrorCode == CustomErrorMessageProvider.SslProblemsBecauseOfOldAndroid)
                {
                    // We don't translate that message since it only might affect 1% and I don't want to bother the translators
                    _toastDisplayer.Error("Unfortunately, playing this item is not supported on your device because it runs on an old version of Android.");
                    _logger.Error("MediaControllerCallback", "Unplayable because of SSL problems on old Android");
                }
                else
                {
                    _playerErrorHandler.PlaybackError(technicalMessage, _latestTrackModel);
                }
            }

            if (state.State == PlaybackStateCompat.StateBuffering && state.Position == 0)
                _playerAnalytics.TrackPlaybackRequested(_latestTrackModel);
            if (state.State == PlaybackStateCompat.StatePlaying)
                _playerAnalytics.TrackPlaybackStarted(_latestTrackModel);
        }

        public override void OnMetadataChanged(MediaMetadataCompat metadata)
        {
            // when starting a new playlist the first metadata may be null
            if (metadata == null)
                return;

            var model = _metadataMapper.LookupTrackFromMetadata(metadata, _mediaQueue);
            _messenger.Publish(new CurrentTrackChangedMessage(model, this));
            _latestTrackModel = model;
        }
    }
}
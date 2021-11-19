using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.NewMediaPlayer;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Analytics
{
    public class PlayerAnalytics : IPlayerAnalytics
    {
        private readonly IAnalytics _analytics;
        private readonly IStopwatchManager _stopwatchManager;
        private readonly ILogger _logger;

        private MvxSubscriptionToken _updateTrackCompletedToken;

        private Stopwatch PlaybackDelayStopwatch => _stopwatchManager.GetStopwatch(StopwatchType.PlaybackDelay);

        public PlayerAnalytics(IAnalytics analytics, IStopwatchManager stopwatchManager, IMvxMessenger messenger, ILogger logger)
        {
            _analytics = analytics;
            _stopwatchManager = stopwatchManager;
            _updateTrackCompletedToken = messenger.Subscribe<TrackCompletedMessage>(OnTrackCompleted);
            _logger = logger;
        }

        protected virtual void OnTrackCompleted(TrackCompletedMessage message)
        {
            TrackFinished();
            if (message.PlayStatus == PlayStatus.Ended)
                QueueCompleted(message.NumberOfTracksInQueue);
        }

        public void TrackFinished()
        {
            _analytics.LogEvent(Event.TrackFinished);
        }

        public void QueueCompleted(int numberOfTracks)
        {
            _analytics.LogEvent(Event.QueueCompleted,
                new Dictionary<string, object>
                {
                    {"tracksInQueue", numberOfTracks}
                });
        }

        public void TrackPlaybackRequested(ITrackModel track)
        {
            var stopwatch = PlaybackDelayStopwatch;
            if (!stopwatch.IsRunning)
                stopwatch.Restart();
        }

        public void TrackPlaybackStarted(ITrackModel track)
        {
            if (track == null || !PlaybackDelayStopwatch.IsRunning || PlaybackDelayStopwatch.Elapsed.TotalSeconds >= 60)
                return;

            PlaybackDelayStopwatch.Stop();

            _analytics.LogEvent(Event.PlayTrack, new Dictionary<string, object>
            {
                {"track", track.Id},
                {"availability", track.Availability == ResourceAvailability.Remote ? "online" : "offline" },
                {"waitTimeInSeconds", PlaybackDelayStopwatch.Elapsed.TotalSeconds}
            });
        }

        public void MediaBrowserConnectionFailed()
        {
            _analytics.LogEvent(Event.MediaBrowserConnectionFailed);
        }

        public void MediaBrowserConnectionSuspended()
        {
            _analytics.LogEvent(Event.MediaBrowserConnectionSuspended);
        }

        public void LogIfDownloadedTrackHasDifferentAttributesThanTrackFromTheApi(IMediaTrack downloadedTrack)
        {
            try
            {
                if (downloadedTrack?.LocalPath == null) return;

                using (var mp3 = TagLib.File.Create(downloadedTrack.LocalPath))
                {
                    string uniqueKeyInFile = mp3?.Name?
                            .Split('/')
                            .Last()
                            .Replace("track_", "")
                            .Replace("_media", "")
                            .Replace(".mp3", "");

                   if (!mp3.Tag?.Title?.StartsWith(downloadedTrack.Title) == null ||
                        !mp3.Tag?.Album?.StartsWith(downloadedTrack.Album) == null ||
                        uniqueKeyInFile != downloadedTrack.GetUniqueKey)
                   {
                       _analytics.LogEvent("Played file was not the requested track",
                           new Dictionary<string, object>
                           {
                               {"Message", $"Requested {downloadedTrack.Title}, {downloadedTrack.Album} but Id3Tag's are {mp3.Tag?.Title}, {mp3.Tag?.Album}"},
                               {"TitleInFile", mp3.Tag?.Title},
                               {"AlbumInFile", mp3.Tag?.Album},
                               {"UniqueKeyInFile", uniqueKeyInFile},
                               {"TitleFromApi", downloadedTrack.Title},
                               {"AlbumFromApi", downloadedTrack.Album},
                               {"UniqueKeyFromApi", downloadedTrack.GetUniqueKey},
                               {"TrackId", downloadedTrack.Id},
                               {"Availability", downloadedTrack.Availability},
                               {"TrackType", downloadedTrack.Subtype}
                           });
                   }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error("PlayerAnalytics", "Unexpected error while creating uniqueKeyInFile", ex);
            }
        }
    }
}
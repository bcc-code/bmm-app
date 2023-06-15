using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.Startup;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.PlayObserver
{
    /// <summary>
    /// This class runs at startup checking if there is a persisted <see cref="TrackPlayedEvent"/> and logging it if it does.
    /// It should never find an event on Android since <see cref="IPlayStatistics.AppWasKilled" /> will be triggered.
    /// It's possible to test it though by debugging the app and stopping debugging while listening to a song. It might also be needed if the app crashes.
    /// </summary>
    public class PersistedEventWriter : IDelayedStartupTask
    {
        private readonly IPlayStatistics _playStatistics;
        private readonly IBlobCache _localStorage;
        private readonly ILogger _logger;
        private readonly IAnalytics _analytics;
        private readonly IPlaybackHistoryService _playbackHistoryService;

        public PersistedEventWriter(
            IPlayStatistics playStatistics,
            IBlobCache localStorage,
            ILogger logger,
            IAnalytics analytics,
            IPlaybackHistoryService playbackHistoryService)
        {
            _playStatistics = playStatistics;
            _localStorage = localStorage;
            _logger = logger;
            _analytics = analytics;
            _playbackHistoryService = playbackHistoryService;
        }

        public async Task RunAfterStartup()
        {
            await _playStatistics.TrySendSavedStreakPointsEvents();
            var storedEvent = AppSettings.UnfinishedTrackPlayedEvent;

            if (storedEvent == null)
                return;

            storedEvent.SentAfterStartup = true;

            _logger.Info("PersistedEventLogger", "Logging Track played after startup");
            _analytics.LogEvent("Log Track played after startup",
                new Dictionary<string, object>
                {
                    {"TrackId", storedEvent.TrackId},
                    {"TimestampStart", storedEvent.TimestampStart},
                    {"TimestampEnd", storedEvent.TimestampEnd},
                    {"SpentTime", storedEvent.SpentTime},
                    {"UniqueSecondsListened", storedEvent.UniqueSecondsListened},
                    {"LastPosition", storedEvent.LastPosition}
                });

            await _playbackHistoryService.AddPlayedTrack(storedEvent.Track, storedEvent.LastPosition, storedEvent.TimestampStart);
            await _playStatistics.WriteEvent(storedEvent);
            AppSettings.UnfinishedTrackPlayedEvent = null;
        }
    }
}

﻿using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.PlayObserver.Model;
using BMM.Core.Implementations.Storage;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.Models.Storage;

namespace BMM.Core.Implementations.PlayObserver
{
    /// <summary>
    /// Persists the latest state every few seconds when playing a song.
    /// At startup it checks for a persisted state and sends it to the server.
    /// </summary>
    public class PersistingPlayStatisticsDecorator : PlayStatisticsDecorator
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public const int IntervalInSeconds = 10;

        private readonly ILogger _logger;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IMeasurementCalculator _measurementCalculator;
        private readonly IFirebaseRemoteConfig _config;
        private readonly IAnalytics _analytics;
        private readonly IRememberedQueueService _rememberedQueueService;

        private Timer _timer;
        private readonly TimeSpan _interval = new TimeSpan(0, 0, IntervalInSeconds);

        private const string Tag = nameof(PersistingPlayStatisticsDecorator);

        public PersistingPlayStatisticsDecorator(
            IPlayStatistics playStatistics,
            ILogger logger,
            IExceptionHandler exceptionHandler,
            IMeasurementCalculator measurementCalculator,
            IFirebaseRemoteConfig config,
            IAnalytics analytics,
            IRememberedQueueService rememberedQueueService) : base(playStatistics)
        {
            _logger = logger;
            _exceptionHandler = exceptionHandler;
            _measurementCalculator = measurementCalculator;
            _config = config;
            _analytics = analytics;
            _rememberedQueueService = rememberedQueueService;
        }

        public override void OnPlaybackStateChanged(IPlaybackState state)
        {
            if (!IsPlaying && state.IsPlaying)
            {
                _logger.Info(Tag, "Playback started");
                _timer = new Timer(PersistCurrentState, null, _interval, _interval);
            }
            else if (IsPlaying && !state.IsPlaying)
            {
                _logger.Info(Tag, "Playback paused");
                _timer?.Dispose();
                _timer = null;
            }

            base.OnPlaybackStateChanged(state);
        }

        private void PersistCurrentState(object state)
        {
            RunStorageEditingTask(async () =>
            {
                if (!IsPlaying || CurrentTrack == null)
                    return;

                var endTime = DateTime.UtcNow;
                var elapsedTimeSinceLastPortion = endTime - StartTimeOfNextPortion;
                double endPosition = StartOfNextPortion + elapsedTimeSinceLastPortion.TotalMilliseconds * (double)DesiredPlaybackRate;

                var portions = PortionsListened.Clone();
                portions.Add(new ListenedPortion
                {
                    Start = StartOfNextPortion,
                    StartTime = StartTimeOfNextPortion,
                    End = endPosition,
                    EndTime = endTime,
                    PlaybackRate = DesiredPlaybackRate
                });

                var measurements = _measurementCalculator.Calculate(CurrentTrack.Duration, portions);

                if (measurements == null)
                    return;

                var playedEvent = ComposeEvent(measurements);

                _logger.Info(Tag, $"Persist state, Portions: ${portions.Count}, spentTime: {measurements.SpentTime}, seconds listened: {measurements.UniqueSecondsListened}");

                AppSettings.UnfinishedTrackPlayedEvent = playedEvent;
                await PersistCurrentQueue(playedEvent);
            });
        }

        private async Task PersistCurrentQueue(TrackPlayedEvent playedEvent)
        {
            if (CurrentQueue == null || CurrentTrack == null)
                return;
            
            AppSettings.CurrentTrackPosition = new CurrentTrackPositionStorage(CurrentTrack.Id, playedEvent.LastPosition);

            if (!IsCurrentQueueSaved)
            {
                await _rememberedQueueService.SaveQueue(CurrentQueue.OfType<Track>().ToList());
                IsCurrentQueueSaved = true;
            }
        }

        public override void Clear()
        {
            base.Clear();

            if (_config.UseExtendedStreakLogging)
                _analytics.LogEvent("remove unfinished track played event from storage");
            RunStorageEditingTask(async () => { AppSettings.UnfinishedTrackPlayedEvent = null; });
        }

        /// <summary>
        /// We want to make sure that only 1 instance can run at the same time which might end in editing of the localStorage.
        /// </summary>
        private void RunStorageEditingTask(Func<Task> task)
        {
            _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    await task.Invoke();
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.PlayObserver.Model;
using BMM.Core.Messages.MediaPlayer;

namespace BMM.Core.Implementations.PlayObserver
{
    public abstract class PlayStatisticsDecorator : IPlayStatistics
    {
        private readonly IPlayStatistics _playStatistics;

        public ITrackModel CurrentTrack => _playStatistics.CurrentTrack;

        public IList<IMediaTrack> CurrentQueue => _playStatistics.CurrentQueue;

        public bool IsCurrentQueueSaved
        {
            get => _playStatistics.IsCurrentQueueSaved;
            set => _playStatistics.IsCurrentQueueSaved = value;
        }

        public bool IsPlaying => _playStatistics.IsPlaying;
        public decimal DesiredPlaybackRate => _playStatistics.DesiredPlaybackRate;

        public IList<ListenedPortion> PortionsListened => _playStatistics.PortionsListened;

        public double StartOfNextPortion => _playStatistics.StartOfNextPortion;

        public DateTime StartTimeOfNextPortion => _playStatistics.StartTimeOfNextPortion;

        public Action TriggerClear
        {
            get => _playStatistics.TriggerClear;
            set => _playStatistics.TriggerClear = value;
        }

        public PlayStatisticsDecorator(IPlayStatistics playStatistics)
        {
            _playStatistics = playStatistics;
        }

        public virtual void OnCurrentTrackChanged(CurrentTrackChangedMessage message)
        {
            _playStatistics.OnCurrentTrackChanged(message);
        }

        public virtual void OnCurrentQueueChanged(CurrentQueueChangedMessage message)
        {
            _playStatistics.OnCurrentQueueChanged(message);
        }

        public virtual void OnSeeked(double currentPosition, double seekedPosition)
        {
            _playStatistics.OnSeeked(currentPosition, seekedPosition);
        }

        public virtual void OnTrackCompleted(TrackCompletedMessage message)
        {
            _playStatistics.OnTrackCompleted(message);
        }

        public virtual void OnPlaybackStateChanged(IPlaybackState state)
        {
            _playStatistics.OnPlaybackStateChanged(state);
        }

        public virtual void Clear()
        {
            _playStatistics.Clear();
        }

        public PlayMeasurements GetMeasurementForNewPosition(long position)
        {
            return _playStatistics.GetMeasurementForNewPosition(position);
        }

        public Task TrySendSavedStreakPointsEvents()
        {
            return _playStatistics.TrySendSavedStreakPointsEvents();
        }

        public Task PostStreakPoints(ITrackModel track, PlayMeasurements playMeasurements)
        {
            return _playStatistics.PostStreakPoints(track, playMeasurements);
        }

        public TrackPlayedEvent ComposeEvent(PlayMeasurements measurements, [CallerMemberName] string callerName = "")
        {
            return _playStatistics.ComposeEvent(measurements, callerName);
        }

        public Task WriteEvent(TrackPlayedEvent ev, [CallerMemberName] string callerName = "")
        {
           return  _playStatistics.WriteEvent(ev, callerName);
        }
    }
}
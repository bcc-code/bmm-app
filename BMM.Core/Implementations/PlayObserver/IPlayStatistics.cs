using System.Runtime.CompilerServices;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.PlayObserver.Model;
using BMM.Core.Messages.MediaPlayer;

namespace BMM.Core.Implementations.PlayObserver
{
    public interface IPlayStatistics
    {
        void OnCurrentTrackChanged(CurrentTrackChangedMessage message);

        void OnCurrentQueueChanged(CurrentQueueChangedMessage message);

        void OnSeeked(double currentPosition, double seekedPosition);

        void OnTrackCompleted(TrackCompletedMessage message);

        void OnPlaybackStateChanged(IPlaybackState state);

        ITrackModel CurrentTrack { get; }

        IList<IMediaTrack> CurrentQueue { get; }

        bool IsCurrentQueueSaved { get; set; }

        bool IsPlaying { get; }
        
        decimal DesiredPlaybackRate { get; }

        IList<ListenedPortion> PortionsListened { get; }

        double StartOfNextPortion { get; }
        DateTime StartTimeOfNextPortion { get; }

        Action TriggerClear { get; set; }

        TrackPlayedEvent ComposeEvent(PlayMeasurements measurements, [CallerMemberName] string callerName = "");

        Task WriteEvent(TrackPlayedEvent ev, [CallerMemberName] string callerName = "");

        void Clear();

        PlayMeasurements GetMeasurementForNewPosition(long position);

        Task TrySendSavedStreakPointsEvents();
        
        Task PostStreakPoints(ITrackModel track, PlayMeasurements playMeasurements);
        
        void OnCurrentTrackWillChange(double currentPosition, decimal playbackRate);
        Task PostListeningEvent(ITrackModel track, PlayMeasurements measurements);
    }
}
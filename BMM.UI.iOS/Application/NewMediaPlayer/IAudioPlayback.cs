using System;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.NewMediaPlayer;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public interface IAudioPlayback
    {
        Task Play(IMediaTrack mediaTrack = null);

        Task LoadTrackToPlay(IMediaTrack mediaTrack = null);

        Task PlayPause();

        void Stop();

        Action OnMediaFinished { get; set; }

        Action<long, long> OnPositionChanged { set; }

        Action OnStateChanged { set; }

        PlayStatus Status { get; }

        float Rate { get; set; }
        
        decimal DesiredRate { get; set; }

        long Position { get; }

        long Buffered { get; }

        void SeekTo(long newPositionInMs, bool playAutomatically = true);
    }
}
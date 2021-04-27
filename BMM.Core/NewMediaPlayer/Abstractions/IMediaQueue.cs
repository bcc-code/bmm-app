using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;

namespace BMM.Core.NewMediaPlayer.Abstractions
{
    public interface IMediaQueue
    {
        void Replace(IMediaTrack track);

        Task<bool> Replace(IEnumerable<IMediaTrack> tracks, IMediaTrack currentTrack);

        Task<bool> PlayNext(IMediaTrack track, IMediaTrack currentPlayedTrack);

        Task<bool> Append(IMediaTrack track);

        IList<IMediaTrack> Tracks { get; }

        bool IsSameQueue(IList<IMediaTrack> newMediaTracks);

        IMediaTrack GetTrackById(int id);

        void Clear();
    }
}
using System;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Abstraction
{
    /// <summary>
    /// A Track that is playable by the MediaPlayer
    /// </summary>
    public interface IMediaTrack : ITrackModel
    {
        TrackMediaType MediaType { get; }

        ITrackMetadata Metadata { get; }

        string GetUniqueKey { get; }

        long LastPosition { get; set; }

        DateTime? LastPlayedAt { get; set; }

        TrackMediaFile TrackMediaFile { get; }
    }
}
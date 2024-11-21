using System;
using System.Collections.Generic;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Abstraction
{
    public interface ITrackMetadata
    {
        string Title { get; }

        string Album { get; }

        string Artist { get; }
    }

    public interface ITrackModel : ITrackMetadata
    {
        int Id { get; }

        /// <summary>
        /// The duration of the track in ms.
        /// </summary>
        long Duration { get; }

        string ArtworkUri { get; }

        TrackSubType Subtype { get; }

        string Url { get; }

        string LocalPath { get; set; }

        ResourceAvailability Availability { get; }

        int ParentId { get; }

        IEnumerable<string> Tags { get; set; }

        bool IsLivePlayback { get; }

        DateTime RecordedAt { get; }

        IEnumerable<TrackRelation> Relations { get; set; }

        string Language { get; set; }
        
        IEnumerable<string> AvailableLanguages { get; set; }

        string PlaybackOrigin { get; set; }
        
        long LastPosition { get; set; }

        TrackMetadata Meta { get; set; }
        
        bool HasTranscription { get; set; }
        
        bool IsLiked { get; set; }
        
        DateTime PublishedAt { get; set; }
    }
}

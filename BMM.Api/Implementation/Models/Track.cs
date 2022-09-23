using System;
using System.Collections.Generic;
using System.Linq;
using BMM.Api.Abstraction;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class Track : Document, IMediaTrack, IDownloadable
    {
        public Track()
        {
            DocumentType = DocumentType.Track;
        }

        [JsonProperty(PropertyName = "languages")]
        public IEnumerable<string> AvailableLanguages { get; set; }

        public string Comment { get; set; }

        public bool IsVisible { get; set; }

        public string Language { get; set; }

        public IEnumerable<TrackMedia> Media { get; set; }

        [JsonProperty(PropertyName = "_meta")]
        public TrackMetadata Meta { get; set; }

        public int Order { get; set; }

        public int ParentId { get; set; }

        public DateTime PublishedAt { get; set; }

        public DateTime RecordedAt { get; set; }

        [JsonProperty(PropertyName = "rel")]
        public IEnumerable<TrackRelation> Relations { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TrackSubType Subtype { get; set; }

        public IEnumerable<string> Tags { get; set; }

        [JsonProperty(PropertyName = "cover")]
        public string Thumbnail { get; set; }

        // contains the value that is stored in the database. But we should use the value from metadata instead.
        [JsonProperty(PropertyName = "title")]
        public string InternalTitle { get; set; }

        [JsonIgnore]
        public string GetUniqueKey => $"{Id}_{Language}";

        public long LastPosition { get; set; }

        public DateTime? LastPlayedAt { get; set; }

        public bool Equals(Track other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return GetUniqueKey == other.GetUniqueKey
                   && LastPosition == other.LastPosition
                   && LastPlayedAt == other.LastPlayedAt
                   && ArtworkUri == other.ArtworkUri;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((Track) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = GetUniqueKey.GetHashCode();
                hashCode = (hashCode * 397) ^ LastPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ (LastPlayedAt != null ? LastPlayedAt.Value.ToBinary().GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ArtworkUri != null ? ArtworkUri.GetHashCode() : 0);
                return hashCode;
            }
        }

        [JsonIgnore]
        public ITrackMetadata Metadata => Meta;

        [JsonIgnore]
        public string LocalPath { get; set; }

        [JsonIgnore]
        public ResourceAvailability Availability => LocalPath == null ? ResourceAvailability.Remote : ResourceAvailability.Local;

        #region track media

        public TrackMediaFile TrackMediaFile
        {
            get { return Media?.FirstOrDefault(e => e.Type == TrackMediaType)?.Files?.FirstOrDefault(); }
        }

        [JsonIgnore]
        public TrackMediaType MediaType => Subtype == TrackSubType.Video ? TrackMediaType.Video : TrackMediaType.Audio;

        private TrackMediaType TrackMediaType => Subtype == TrackSubType.Video ? TrackMediaType.Video : TrackMediaType.Audio;

        private string _url;

        public string Url
        {
            get => _url ?? TrackMediaFile?.Url;
            set => _url = value;
        }

        [JsonIgnore]
        public long Duration => Convert.ToInt64(TrackMediaFile?.Duration * 1000);

        #endregion

        #region extensions

        [JsonIgnore]
        public string Album => Meta.Album;

        [JsonIgnore]
        public string Artist => Meta.Artist;

        [JsonIgnore]
        public string ArtworkUri => Meta?.AttachedPicture;

        [JsonIgnore]
        public string Title => Meta.Title;

        [JsonIgnore]
        public bool IsLivePlayback => Subtype == TrackSubType.Live;

        [JsonIgnore]
        public bool IsListened { get; set; }

        public string PlaybackOrigin { get; set; }

        #endregion
    }
}
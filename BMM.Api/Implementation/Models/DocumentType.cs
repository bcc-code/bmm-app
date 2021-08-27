using System.Runtime.Serialization;

namespace BMM.Api.Implementation.Models
{
    public enum DocumentType
    {
        [EnumMember(Value = "track")]
        Track,

        [EnumMember(Value = "album")]
        Album,

        [EnumMember(Value = "contributor")]
        Contributor,

        [EnumMember(Value = "track_collection")]
        TrackCollection,

        [EnumMember(Value = "podcast")]
        Podcast,

        [EnumMember(Value = "pinned_item")]
        PinnedItem,

        [IgnoreDataMember]
        ChapterHeader,

        [IgnoreDataMember]
        PlaylistsCollection,

        [EnumMember(Value = "section_header")]
        DiscoverSectionHeader,

        [EnumMember(Value = "playlist")]
        Playlist,

        [EnumMember(Value = "listening_streak")]
        ListeningStreak,

        [EnumMember(Value = "unsupported")]
        Unsupported
    }
}
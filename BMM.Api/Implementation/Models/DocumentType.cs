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

        [EnumMember(Value = "chapter_header")]
        ChapterHeader,

        [EnumMember(Value = "Tile")]
        Tile,

        [IgnoreDataMember]
        PlaylistsCollection,

        [IgnoreDataMember]
        AslaksenTeaser,

        [IgnoreDataMember]
        FraKaareTeaser,

        [IgnoreDataMember]
        LiveRadio,

        [IgnoreDataMember]
        SimpleMargin,

        [IgnoreDataMember]
        ContinueListeningCollection,

        [EnumMember(Value = "section_header")]
        DiscoverSectionHeader,

        [EnumMember(Value = "playlist")]
        Playlist,

        [EnumMember(Value = "listening_streak")]
        ListeningStreak,

        [EnumMember(Value = "InfoMessage")]
        InfoMessage,

        [EnumMember(Value = "unsupported")]
        Unsupported
    }
}
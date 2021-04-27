using System.Runtime.Serialization;

namespace BMM.Api.Implementation.Models
{
    public enum TrackRelationType
    {
        [EnumMember(Value = "bible")] Bible,

        [EnumMember(Value = "composer")] Composer,

        [EnumMember(Value = "arranger")] Arranger,

        [EnumMember(Value = "lyricist")] Lyricist,

        [EnumMember(Value = "interpret")] Interpret,

        [EnumMember(Value = "songbook")] Songbook,

        [EnumMember(Value = "external")] External
    }
}
using System.Runtime.Serialization;

namespace BMM.Api.Implementation.Models
{
    public enum TrackSubType
    {
        [EnumMember(Value = "song")]
        Song,

        [EnumMember(Value = "speech")]
        Speech,

        [EnumMember(Value = "audiobook")]
        Audiobook,

        [EnumMember(Value = "singsong")]
        Singsong,

        [EnumMember(Value = "exegesis")]
        Exegesis,

        [EnumMember(Value = "video")]
        Video,

        [EnumMember(Value = "live")]
        Live
    }
}
using System.Runtime.Serialization;

namespace BMM.Api.Implementation.Models
{
    public enum TrackMediaType
    {
        [EnumMember(Value = "audio")] Audio,

        [EnumMember(Value = "video")] Video
    }
}
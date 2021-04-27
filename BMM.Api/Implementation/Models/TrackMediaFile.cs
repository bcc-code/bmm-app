using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class TrackMediaFile
    {
        public int Duration { get; set; }

        public string MimeType { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public string Url { get; set; }

        public int Id { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public virtual bool ShouldSerializeUrl()
        {
            return Url != default(string);
        }
    }
}
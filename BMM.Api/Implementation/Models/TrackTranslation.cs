using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class TrackTranslation
    {
        public bool IsVisible { get; set; }

        public string Language { get; set; }

        public IEnumerable<TrackMedia> Media { get; set; }

        public string Title { get; set; }
    }
}
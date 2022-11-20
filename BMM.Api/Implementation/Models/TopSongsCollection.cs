using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class TopSongsCollection : Document
    {
        public string Name { get; set; }
        public string PageTitle { get; set; }
        public ICollection<TopSong> Tracks { get; set; }
    }
}
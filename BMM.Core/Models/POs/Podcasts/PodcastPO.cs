using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs.Podcasts
{
    public class PodcastPO : DocumentPO, ITrackListHolderPO
    {
        public PodcastPO(Podcast podcast) : base(podcast)
        {
            Podcast = podcast;
        }
        
        public Podcast Podcast { get; }
        public string Title => Podcast.Title;
        public string Cover => Podcast.Cover;
    }
}
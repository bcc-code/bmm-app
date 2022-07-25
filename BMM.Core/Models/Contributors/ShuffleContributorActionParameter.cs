using BMM.Core.Models.Contributors.Interfaces;

namespace BMM.Core.Models.Contributors
{
    public class ShuffleContributorActionParameter : IShuffleContributorActionParameter
    {
        public ShuffleContributorActionParameter(int contributorId, string playbackOrigin)
        {
            ContributorId = contributorId;
            PlaybackOrigin = playbackOrigin;
        }
        
        public int ContributorId { get; }
        public string PlaybackOrigin { get; }
    }
}
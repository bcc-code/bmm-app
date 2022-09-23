using BMM.Core.Models.Contributors.Interfaces;

namespace BMM.Core.Models.Contributors
{
    public class ShuffleActionParameter : IShuffleActionParameter
    {
        public ShuffleActionParameter(int id, string playbackOrigin)
        {
            Id = id;
            PlaybackOrigin = playbackOrigin;
        }
        
        public int Id { get; }
        public string PlaybackOrigin { get; }
    }
}
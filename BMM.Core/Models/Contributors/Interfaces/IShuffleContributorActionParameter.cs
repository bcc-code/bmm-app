namespace BMM.Core.Models.Contributors.Interfaces
{
    public interface IShuffleContributorActionParameter
    {
        int ContributorId { get; }
        string PlaybackOrigin { get; }
    }
}
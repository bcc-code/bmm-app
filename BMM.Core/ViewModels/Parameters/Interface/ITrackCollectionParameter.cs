namespace BMM.Core.ViewModels.Parameters.Interface
{
    public interface ITrackCollectionParameter
    {
        int TrackCollectionId { get; }
        string Name { get; }
        bool UseLikeIcon { get; }
    }
}
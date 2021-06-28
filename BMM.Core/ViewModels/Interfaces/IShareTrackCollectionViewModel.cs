using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IShareTrackCollectionViewModel : IMvxViewModel<ITrackCollectionParameter>
    {
        string TrackCollectionName { get; }
        string TrackCollectionShareType { get; }
    }
}
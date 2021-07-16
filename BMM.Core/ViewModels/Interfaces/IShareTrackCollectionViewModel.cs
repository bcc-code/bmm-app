using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IShareTrackCollectionViewModel : IMvxViewModel<ITrackCollectionParameter>
    {
        string TrackCollectionName { get; }
        int FollowersCount { get; }
        IMvxAsyncCommand ShareCommand { get; }
        IMvxAsyncCommand MakePrivateCommand { get; }
    }
}
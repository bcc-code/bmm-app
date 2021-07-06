using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface ISharedTrackCollectionViewModel : IMvxViewModel<ITrackCollectionParameter>
    {
        IMvxAsyncCommand AddToMyPlaylistCommand { get; }
    }
}
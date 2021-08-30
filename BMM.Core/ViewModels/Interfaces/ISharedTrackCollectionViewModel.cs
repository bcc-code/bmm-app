using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface ISharedTrackCollectionViewModel : IBaseViewModel, IMvxViewModel<ISharedTrackCollectionParameter>
    {
        IMvxAsyncCommand AddToMyPlaylistCommand { get; }
        TrackCollection MyCollection { get; }
        string SharingSecret { get; }
    }
}
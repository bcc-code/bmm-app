using BMM.Api.Abstraction;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IPlayerViewModel : IBaseViewModel
    {
        IMvxAsyncCommand NavigateToLanguageChangeCommand { get; }
        ITrackModel CurrentTrack { get; }
    }
}
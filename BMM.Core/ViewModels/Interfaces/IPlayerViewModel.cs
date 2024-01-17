using BMM.Api.Abstraction;
using BMM.Core.Models.Enums;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IPlayerViewModel : IBaseViewModel
    {
        IMvxAsyncCommand NavigateToLanguageChangeCommand { get; }
        ITrackModel CurrentTrack { get; }
        bool HasLeftButton { get; }
        bool HasExternalRelations { get; set; }
        MvxCommand LeftButtonClickedCommand { get; }
        string LeftButtonLink { get; set; }
        string TrackLanguage { get; set; }
        PlayerLeftButtonType? LeftButtonType { get; set; }
        bool HasTranscription { get; set; }
    }
}
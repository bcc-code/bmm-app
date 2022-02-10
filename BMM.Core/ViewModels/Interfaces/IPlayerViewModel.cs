using BMM.Api.Abstraction;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IPlayerViewModel : IBaseViewModel
    {
        IMvxAsyncCommand NavigateToLanguageChangeCommand { get; }
        ITrackModel CurrentTrack { get; }
        bool HasLyrics { get; }
        bool HasExternalRelations { get; set; }
        MvxCommand OpenLyricsCommand { get; }
        string SongTreasureLink { get; set; }
        string TrackLanguage { get; set; }
    }
}
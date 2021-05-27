namespace BMM.Core.ViewModels
{
    public interface ITrackListViewModel
    {
        bool ShowSharingInfo { get; }

        bool ShowDownloadButtons { get; }

        bool IsDownloaded { get; }

        string Title { get; }

        bool ShowPlaylistIcon { get; }
        bool ShowImage { get; }
        string Image { get; }
        bool UseCircularImage { get; }

        bool ShowFollowButtons { get; }

        bool ShowShuffleButton { get; }

        bool ShowTrackCount { get; }
    }
}
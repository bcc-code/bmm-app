namespace BMM.Core.ViewModels
{
    public interface ITrackListViewModel
    {
        bool ShowSharingInfo { get; }

        bool ShowImage { get; }

        bool ShowDownloadButtons { get; }

        bool IsDownloaded { get; }

        string Title { get; }

        string Image { get; }

        bool ShowFollowButtons { get; }
    }
}
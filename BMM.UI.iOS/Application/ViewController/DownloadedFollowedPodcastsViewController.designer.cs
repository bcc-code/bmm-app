// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace BMM.UI.iOS
{
    [Register ("DownloadedFollowedPodcastsViewController")]
    partial class DownloadedFollowedPodcastsViewController
    {
        [Outlet]
        UIKit.UICollectionView PodcastCollectionView { get; set; }


        [Outlet]
        UIKit.UIView EmptyStateView { get; set; }


        [Outlet]
        UIKit.UILabel PlaylistEmptyHeadlineLabel { get; set; }


        [Outlet]
        UIKit.UILabel PlaylistEmptyTextLabel { get; set; }

        [Outlet]
        UIKit.UIView OfflineBannerView { get; set; }

        [Outlet]
        UIKit.UILabel OfflineBannerLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (EmptyStateView != null) {
                EmptyStateView.Dispose ();
                EmptyStateView = null;
            }

            if (OfflineBannerLabel != null) {
                OfflineBannerLabel.Dispose ();
                OfflineBannerLabel = null;
            }

            if (OfflineBannerView != null) {
                OfflineBannerView.Dispose ();
                OfflineBannerView = null;
            }

            if (PlaylistEmptyHeadlineLabel != null) {
                PlaylistEmptyHeadlineLabel.Dispose ();
                PlaylistEmptyHeadlineLabel = null;
            }

            if (PlaylistEmptyTextLabel != null) {
                PlaylistEmptyTextLabel.Dispose ();
                PlaylistEmptyTextLabel = null;
            }

            if (PodcastCollectionView != null) {
                PodcastCollectionView.Dispose ();
                PodcastCollectionView = null;
            }
        }
    }
}
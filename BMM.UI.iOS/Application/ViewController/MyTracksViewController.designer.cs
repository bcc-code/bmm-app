// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BMM.UI.iOS
{
    [Register ("MyTracksViewController")]
    partial class MyTracksViewController
    {
        [Outlet]
        UIKit.UITableView CollectionTable { get; set; }


        [Outlet]
        UIKit.UILabel DownloadingStatusLabel { get; set; }


        [Outlet]
        UIKit.UIView DownloadingStatusView { get; set; }


        [Outlet]
        UIKit.UIView EmptyStateView { get; set; }


        [Outlet]
        UIKit.UIButton OfflineAvailableButton { get; set; }


        [Outlet]
        UIKit.UIProgressView OfflineAvailableProgress { get; set; }


        [Outlet]
        UIKit.UILabel OfflineAvailableSubtitleLabel { get; set; }


        [Outlet]
        UIKit.UISwitch OfflineAvailableSwitch { get; set; }


        [Outlet]
        UIKit.UILabel OfflineAvailableTitleLabel { get; set; }


        [Outlet]
        UIKit.UIView OfflineAvailableView { get; set; }


        [Outlet]
        UIKit.UILabel PlaylistEmptyHeadlineLabel { get; set; }


        [Outlet]
        UIKit.UILabel PlaylistEmptyTextLabel { get; set; }


        [Outlet]
        UIKit.UIView PlaylistHeaderView { get; set; }


        [Outlet]
        UIKit.UIButton ShuffleButton { get; set; }

        [Outlet]
        UIKit.UIView OfflineBannerView { get; set; }

        [Outlet]
        UIKit.UILabel OfflineBannerLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint OfflineBannerViewHeightConstraint { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CollectionTable != null) {
                CollectionTable.Dispose ();
                CollectionTable = null;
            }

            if (DownloadingStatusLabel != null) {
                DownloadingStatusLabel.Dispose ();
                DownloadingStatusLabel = null;
            }

            if (DownloadingStatusView != null) {
                DownloadingStatusView.Dispose ();
                DownloadingStatusView = null;
            }

            if (EmptyStateView != null) {
                EmptyStateView.Dispose ();
                EmptyStateView = null;
            }

            if (OfflineAvailableButton != null) {
                OfflineAvailableButton.Dispose ();
                OfflineAvailableButton = null;
            }

            if (OfflineAvailableProgress != null) {
                OfflineAvailableProgress.Dispose ();
                OfflineAvailableProgress = null;
            }

            if (OfflineAvailableSubtitleLabel != null) {
                OfflineAvailableSubtitleLabel.Dispose ();
                OfflineAvailableSubtitleLabel = null;
            }

            if (OfflineAvailableSwitch != null) {
                OfflineAvailableSwitch.Dispose ();
                OfflineAvailableSwitch = null;
            }

            if (OfflineAvailableTitleLabel != null) {
                OfflineAvailableTitleLabel.Dispose ();
                OfflineAvailableTitleLabel = null;
            }

            if (OfflineBannerLabel != null) {
                OfflineBannerLabel.Dispose ();
                OfflineBannerLabel = null;
            }

            if (OfflineBannerView != null) {
                OfflineBannerView.Dispose ();
                OfflineBannerView = null;
            }

            if (OfflineBannerViewHeightConstraint != null) {
                OfflineBannerViewHeightConstraint.Dispose ();
                OfflineBannerViewHeightConstraint = null;
            }

            if (PlaylistEmptyHeadlineLabel != null) {
                PlaylistEmptyHeadlineLabel.Dispose ();
                PlaylistEmptyHeadlineLabel = null;
            }

            if (PlaylistEmptyTextLabel != null) {
                PlaylistEmptyTextLabel.Dispose ();
                PlaylistEmptyTextLabel = null;
            }

            if (PlaylistHeaderView != null) {
                PlaylistHeaderView.Dispose ();
                PlaylistHeaderView = null;
            }

            if (ShuffleButton != null) {
                ShuffleButton.Dispose ();
                ShuffleButton = null;
            }
        }
    }
}
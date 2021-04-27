// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BMM.UI.iOS
{
	[Register ("CuratedPlaylistViewController")]
	partial class CuratedPlaylistViewController
	{
		[Outlet]
		UIKit.UIView blurView { get; set; }

		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView CuratedPlaylistBlurCoverImage { get; set; }

		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView CuratedPlaylistCoverImageView { get; set; }

		[Outlet]
		UIKit.UIView CuratedPlaylistHeaderView { get; set; }

		[Outlet]
		UIKit.UITableView CuratedPlaylistTable { get; set; }

		[Outlet]
		UIKit.UILabel DownloadingStatusLabel { get; set; }

		[Outlet]
		UIKit.UIView DownloadingStatusView { get; set; }

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
		UIKit.UIButton ShuffleButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (blurView != null) {
				blurView.Dispose ();
				blurView = null;
			}

			if (CuratedPlaylistBlurCoverImage != null) {
				CuratedPlaylistBlurCoverImage.Dispose ();
				CuratedPlaylistBlurCoverImage = null;
			}

			if (CuratedPlaylistCoverImageView != null) {
				CuratedPlaylistCoverImageView.Dispose ();
				CuratedPlaylistCoverImageView = null;
			}

			if (CuratedPlaylistHeaderView != null) {
				CuratedPlaylistHeaderView.Dispose ();
				CuratedPlaylistHeaderView = null;
			}

			if (CuratedPlaylistTable != null) {
				CuratedPlaylistTable.Dispose ();
				CuratedPlaylistTable = null;
			}

			if (DownloadingStatusLabel != null) {
				DownloadingStatusLabel.Dispose ();
				DownloadingStatusLabel = null;
			}

			if (DownloadingStatusView != null) {
				DownloadingStatusView.Dispose ();
				DownloadingStatusView = null;
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

			if (ShuffleButton != null) {
				ShuffleButton.Dispose ();
				ShuffleButton = null;
			}

		}
	}
}

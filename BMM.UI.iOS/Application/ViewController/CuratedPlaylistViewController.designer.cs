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
		FFImageLoading.Cross.MvxCachedImageView CuratedPlaylistCoverImageView { get; set; }

		[Outlet]
		UIKit.UIView CuratedPlaylistHeaderView { get; set; }

		[Outlet]
		UIKit.UITableView CuratedPlaylistTable { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		BMM.UI.iOS.DownloadButton DownloadButton { get; set; }

		[Outlet]
		UIKit.UILabel DurationLabel { get; set; }

		[Outlet]
		UIKit.UIButton ShuffleButton { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TrackCountLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
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

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DownloadButton != null) {
				DownloadButton.Dispose ();
				DownloadButton = null;
			}

			if (ShuffleButton != null) {
				ShuffleButton.Dispose ();
				ShuffleButton = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TrackCountLabel != null) {
				TrackCountLabel.Dispose ();
				TrackCountLabel = null;
			}

			if (DurationLabel != null) {
				DurationLabel.Dispose ();
				DurationLabel = null;
			}

		}
	}
}

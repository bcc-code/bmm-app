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
	[Register ("AlbumViewController")]
	partial class AlbumViewController
	{
		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView AlbumCoverImageView { get; set; }

		[Outlet]
		UIKit.UIView AlbumHeaderView { get; set; }

		[Outlet]
		UIKit.UITableView AlbumTable { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ButtonStackViewHeight { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ButtonsToDurationConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ButtonTopConstraint { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		BMM.UI.iOS.DownloadButton DownloadButton { get; set; }

		[Outlet]
		UIKit.UILabel DurationLabel { get; set; }

		[Outlet]
		BMM.UI.iOS.CustomViews.ProgressBarView DurationProgressBar { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint DurationToSeparatorConstraint { get; set; }

		[Outlet]
		UIKit.UIButton PlayButton { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TrackCountLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AlbumCoverImageView != null) {
				AlbumCoverImageView.Dispose ();
				AlbumCoverImageView = null;
			}

			if (AlbumHeaderView != null) {
				AlbumHeaderView.Dispose ();
				AlbumHeaderView = null;
			}

			if (AlbumTable != null) {
				AlbumTable.Dispose ();
				AlbumTable = null;
			}

			if (ButtonStackViewHeight != null) {
				ButtonStackViewHeight.Dispose ();
				ButtonStackViewHeight = null;
			}

			if (ButtonsToDurationConstraint != null) {
				ButtonsToDurationConstraint.Dispose ();
				ButtonsToDurationConstraint = null;
			}

			if (ButtonTopConstraint != null) {
				ButtonTopConstraint.Dispose ();
				ButtonTopConstraint = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DownloadButton != null) {
				DownloadButton.Dispose ();
				DownloadButton = null;
			}

			if (DurationLabel != null) {
				DurationLabel.Dispose ();
				DurationLabel = null;
			}

			if (DurationProgressBar != null) {
				DurationProgressBar.Dispose ();
				DurationProgressBar = null;
			}

			if (DurationToSeparatorConstraint != null) {
				DurationToSeparatorConstraint.Dispose ();
				DurationToSeparatorConstraint = null;
			}

			if (PlayButton != null) {
				PlayButton.Dispose ();
				PlayButton = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TrackCountLabel != null) {
				TrackCountLabel.Dispose ();
				TrackCountLabel = null;
			}

		}
	}
}

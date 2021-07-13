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
	[Register ("TrackCollectionTableViewCell")]
	partial class TrackCollectionTableViewCell
	{
		[Outlet]
		UIKit.UIImageView DownloadStatusImageView { get; set; }

		[Outlet]
		UIKit.UIImageView SharedPlaylistIcon { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint SharedPlaylistIconWidthConstraint { get; set; }

		[Outlet]
		UIKit.UILabel SubtitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (DownloadStatusImageView != null) {
				DownloadStatusImageView.Dispose ();
				DownloadStatusImageView = null;
			}

			if (SubtitleLabel != null) {
				SubtitleLabel.Dispose ();
				SubtitleLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (SharedPlaylistIconWidthConstraint != null) {
				SharedPlaylistIconWidthConstraint.Dispose ();
				SharedPlaylistIconWidthConstraint = null;
			}

			if (SharedPlaylistIcon != null) {
				SharedPlaylistIcon.Dispose ();
				SharedPlaylistIcon = null;
			}

		}
	}
}

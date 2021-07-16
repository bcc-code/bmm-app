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
	[Register ("TrackCollectionViewController")]
	partial class TrackCollectionViewController
	{
		[Outlet]
		UIKit.UITableView CollectionTable { get; set; }

		[Outlet]
		BMM.UI.iOS.DownloadButton DownloadButton { get; set; }

		[Outlet]
		UIKit.UILabel NameLabel { get; set; }

		[Outlet]
		UIKit.UILabel OfflineBannerLabel { get; set; }

		[Outlet]
		UIKit.UIView OfflineBannerView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint OfflineBannerViewHeightConstraint { get; set; }

		[Outlet]
		UIKit.UIView PlaylistHeaderView { get; set; }

		[Outlet]
		UIKit.UIImageView PlaylistIcon { get; set; }

		[Outlet]
		UIKit.UILabel PlaylistState { get; set; }

		[Outlet]
		UIKit.UIButton ShuffleButton { get; set; }

		[Outlet]
		UIKit.UILabel TrackCountLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CollectionTable != null) {
				CollectionTable.Dispose ();
				CollectionTable = null;
			}

			if (DownloadButton != null) {
				DownloadButton.Dispose ();
				DownloadButton = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
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

			if (PlaylistHeaderView != null) {
				PlaylistHeaderView.Dispose ();
				PlaylistHeaderView = null;
			}

			if (ShuffleButton != null) {
				ShuffleButton.Dispose ();
				ShuffleButton = null;
			}

			if (TrackCountLabel != null) {
				TrackCountLabel.Dispose ();
				TrackCountLabel = null;
			}

			if (PlaylistState != null) {
				PlaylistState.Dispose ();
				PlaylistState = null;
			}

			if (PlaylistIcon != null) {
				PlaylistIcon.Dispose ();
				PlaylistIcon = null;
			}

		}
	}
}

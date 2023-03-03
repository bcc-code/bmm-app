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
	[Register ("PodcastViewController")]
	partial class PodcastViewController
	{
		[Outlet]
		UIKit.UIButton FollowButton { get; set; }

		[Outlet]
		UIKit.UIButton FollowingButton { get; set; }

		[Outlet]
		UIKit.UILabel OfflineBannerLabel { get; set; }

		[Outlet]
		UIKit.UIView OfflineBannerView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint OfflineBannerViewHeightConstraint { get; set; }

		[Outlet]
		UIKit.UIButton ShuffleButton { get; set; }

		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView PodcastCoverImageView { get; set; }

		[Outlet]
		UIKit.UIView PodcastHeaderView { get; set; }

		[Outlet]
		UIKit.UITableView PodcastTable { get; set; }

		[Outlet]
		UIKit.UILabel TitelLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (FollowButton != null) {
				FollowButton.Dispose ();
				FollowButton = null;
			}

			if (FollowingButton != null) {
				FollowingButton.Dispose ();
				FollowingButton = null;
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

			if (ShuffleButton != null) {
				ShuffleButton.Dispose ();
				ShuffleButton = null;
			}
			//
			// if (PodcastCoverImageView != null) {
			// 	PodcastCoverImageView.Dispose ();
			// 	PodcastCoverImageView = null;
			// }

			if (PodcastHeaderView != null) {
				PodcastHeaderView.Dispose ();
				PodcastHeaderView = null;
			}

			if (PodcastTable != null) {
				PodcastTable.Dispose ();
				PodcastTable = null;
			}

			if (TitelLabel != null) {
				TitelLabel.Dispose ();
				TitelLabel = null;
			}

		}
	}
}

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
		UIKit.UIView blurView { get; set; }

		[Outlet]
		UIKit.UILabel ButtonLabel { get; set; }

		[Outlet]
		UIKit.UIButton FollowButton { get; set; }

		[Outlet]
		UIKit.UIButton FollowingButton { get; set; }

		[Outlet]
		UIKit.UIImageView FollowingTickImageView { get; set; }

		[Outlet]
		UIKit.UILabel FollowSubtitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel FollowTitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel OfflineBannerLabel { get; set; }

		[Outlet]
		UIKit.UIView OfflineBannerView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint OfflineBannerViewHeightConstraint { get; set; }

		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView PodcastBlurCoverImage { get; set; }

		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView PodcastCoverImageView { get; set; }

		[Outlet]
		UIKit.UIView PodcastHeaderView { get; set; }

		[Outlet]
		UIKit.UITableView PodcastTable { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (blurView != null) {
				blurView.Dispose ();
				blurView = null;
			}

			if (ButtonLabel != null) {
				ButtonLabel.Dispose ();
				ButtonLabel = null;
			}

			if (FollowButton != null) {
				FollowButton.Dispose ();
				FollowButton = null;
			}

			if (FollowingButton != null) {
				FollowingButton.Dispose ();
				FollowingButton = null;
			}

			if (FollowingTickImageView != null) {
				FollowingTickImageView.Dispose ();
				FollowingTickImageView = null;
			}

			if (FollowSubtitleLabel != null) {
				FollowSubtitleLabel.Dispose ();
				FollowSubtitleLabel = null;
			}

			if (FollowTitleLabel != null) {
				FollowTitleLabel.Dispose ();
				FollowTitleLabel = null;
			}

			if (PodcastBlurCoverImage != null) {
				PodcastBlurCoverImage.Dispose ();
				PodcastBlurCoverImage = null;
			}

			if (PodcastCoverImageView != null) {
				PodcastCoverImageView.Dispose ();
				PodcastCoverImageView = null;
			}

			if (PodcastHeaderView != null) {
				PodcastHeaderView.Dispose ();
				PodcastHeaderView = null;
			}

			if (PodcastTable != null) {
				PodcastTable.Dispose ();
				PodcastTable = null;
			}

			if (OfflineBannerView != null) {
				OfflineBannerView.Dispose ();
				OfflineBannerView = null;
			}

			if (OfflineBannerLabel != null) {
				OfflineBannerLabel.Dispose ();
				OfflineBannerLabel = null;
			}

			if (OfflineBannerViewHeightConstraint != null) {
				OfflineBannerViewHeightConstraint.Dispose ();
				OfflineBannerViewHeightConstraint = null;
			}

		}
	}
}

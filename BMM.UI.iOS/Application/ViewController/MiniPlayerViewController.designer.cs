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
	[Register ("MiniPlayerViewController")]
	partial class MiniPlayerViewController
	{
		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView CoverView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint CoverViewBottomConstraint { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint CoverViewLeadingConstraint { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint CoverViewTopConstraint { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView MiniPlayerView { get; set; }

		[Outlet]
		UIKit.UIButton PlayerStatusButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint PlayerStatusButtonTopConstraint { get; set; }

		[Outlet]
		BMM.UI.iOS.CustomProgressBar ProgressBar { get; set; }

		[Outlet]
		UIKit.UIButton ShowPlayerButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint ShowPlayerButtonBottomConstraint { get; set; }

		[Outlet]
		UIKit.UILabel TrackSubtitleLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint TrackSubtitleLabelTopConstraint { get; set; }

		[Outlet]
		UIKit.UILabel TrackTitleLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint TrackTitleLabelTopConstraint { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CoverView != null) {
				CoverView.Dispose ();
				CoverView = null;
			}

			if (CoverViewBottomConstraint != null) {
				CoverViewBottomConstraint.Dispose ();
				CoverViewBottomConstraint = null;
			}

			if (CoverViewLeadingConstraint != null) {
				CoverViewLeadingConstraint.Dispose ();
				CoverViewLeadingConstraint = null;
			}

			if (CoverViewTopConstraint != null) {
				CoverViewTopConstraint.Dispose ();
				CoverViewTopConstraint = null;
			}

			if (MiniPlayerView != null) {
				MiniPlayerView.Dispose ();
				MiniPlayerView = null;
			}

			if (PlayerStatusButton != null) {
				PlayerStatusButton.Dispose ();
				PlayerStatusButton = null;
			}

			if (PlayerStatusButtonTopConstraint != null) {
				PlayerStatusButtonTopConstraint.Dispose ();
				PlayerStatusButtonTopConstraint = null;
			}

			if (ProgressBar != null) {
				ProgressBar.Dispose ();
				ProgressBar = null;
			}

			if (ShowPlayerButton != null) {
				ShowPlayerButton.Dispose ();
				ShowPlayerButton = null;
			}

			if (ShowPlayerButtonBottomConstraint != null) {
				ShowPlayerButtonBottomConstraint.Dispose ();
				ShowPlayerButtonBottomConstraint = null;
			}

			if (TrackSubtitleLabel != null) {
				TrackSubtitleLabel.Dispose ();
				TrackSubtitleLabel = null;
			}

			if (TrackSubtitleLabelTopConstraint != null) {
				TrackSubtitleLabelTopConstraint.Dispose ();
				TrackSubtitleLabelTopConstraint = null;
			}

			if (TrackTitleLabel != null) {
				TrackTitleLabel.Dispose ();
				TrackTitleLabel = null;
			}

			if (TrackTitleLabelTopConstraint != null) {
				TrackTitleLabelTopConstraint.Dispose ();
				TrackTitleLabelTopConstraint = null;
			}

		}
	}
}

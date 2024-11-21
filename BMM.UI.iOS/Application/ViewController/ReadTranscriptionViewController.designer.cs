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
	[Register ("ReadTranscriptionViewController")]
	partial class ReadTranscriptionViewController
	{
		[Outlet]
		UIKit.UIView CloseButtonContainer { get; set; }

		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView CoverView { get; set; }

		[Outlet]
		UIKit.UILabel Header { get; set; }

		[Outlet]
		UIKit.UIView HeaderContainerView { get; set; }

		[Outlet]
		UIKit.UIImageView ImageIcon { get; set; }

		[Outlet]
		UIKit.UIButton PlayerStatusButton { get; set; }

		[Outlet]
		UIKit.UIView PlayerView { get; set; }

		[Outlet]
		UIKit.UIView PlayerViewShadowContainer { get; set; }

		[Outlet]
		BMM.UI.iOS.CustomProgressBar ProgressBar { get; set; }

		[Outlet]
		UIKit.UILabel TrackSubtitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TrackTitleLAbel { get; set; }

		[Outlet]
		UIKit.UITableView TranscriptionsTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CloseButtonContainer != null) {
				CloseButtonContainer.Dispose ();
				CloseButtonContainer = null;
			}

			if (CoverView != null) {
				CoverView.Dispose ();
				CoverView = null;
			}

			if (Header != null) {
				Header.Dispose ();
				Header = null;
			}

			if (ImageIcon != null) {
				ImageIcon.Dispose ();
				ImageIcon = null;
			}

			if (PlayerStatusButton != null) {
				PlayerStatusButton.Dispose ();
				PlayerStatusButton = null;
			}

			if (PlayerView != null) {
				PlayerView.Dispose ();
				PlayerView = null;
			}

			if (PlayerViewShadowContainer != null) {
				PlayerViewShadowContainer.Dispose ();
				PlayerViewShadowContainer = null;
			}

			if (ProgressBar != null) {
				ProgressBar.Dispose ();
				ProgressBar = null;
			}

			if (TrackSubtitleLabel != null) {
				TrackSubtitleLabel.Dispose ();
				TrackSubtitleLabel = null;
			}

			if (TrackTitleLAbel != null) {
				TrackTitleLAbel.Dispose ();
				TrackTitleLAbel = null;
			}

			if (TranscriptionsTableView != null) {
				TranscriptionsTableView.Dispose ();
				TranscriptionsTableView = null;
			}

			if (HeaderContainerView != null) {
				HeaderContainerView.Dispose ();
				HeaderContainerView = null;
			}

		}
	}
}

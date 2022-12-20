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
	[Register ("ContinueListeningTileViewCell")]
	partial class ContinueListeningTileViewCell
	{
		[Outlet]
		UIKit.UIView BackgroundView { get; set; }

		[Outlet]
		UIKit.UIView ContentWidthHelper { get; set; }

		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView CoverImageView { get; set; }

		[Outlet]
		UIKit.UILabel DateLabel { get; set; }

		[Outlet]
		UIKit.UILabel DayOfWeekLabel { get; set; }

		[Outlet]
		UIKit.UIImageView DownloadedIcon { get; set; }

		[Outlet]
		UIKit.UILabel IsPlayingButton { get; set; }

		[Outlet]
		UIKit.UIButton OptionsButton { get; set; }

		[Outlet]
		LottieButton PlayButton { get; set; }

		[Outlet]
		BMM.UI.iOS.CustomViews.ProgressBarView ProgressBarView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ProgressBarWidthConstraint { get; set; }

		[Outlet]
		UIKit.UIButton ReferenceButton { get; set; }

		[Outlet]
		UIKit.UILabel RemainingLabel { get; set; }

		[Outlet]
		UIKit.UIButton ShuffleButton { get; set; }

		[Outlet]
		UIKit.UILabel SubtitleLabel { get; set; }

		[Outlet]
		UIKit.UIView TitleClickableArea { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (CoverImageView != null) {
				CoverImageView.Dispose ();
				CoverImageView = null;
			}

			if (DateLabel != null) {
				DateLabel.Dispose ();
				DateLabel = null;
			}

			if (DayOfWeekLabel != null) {
				DayOfWeekLabel.Dispose ();
				DayOfWeekLabel = null;
			}

			if (DownloadedIcon != null) {
				DownloadedIcon.Dispose ();
				DownloadedIcon = null;
			}

			if (IsPlayingButton != null) {
				IsPlayingButton.Dispose ();
				IsPlayingButton = null;
			}

			if (OptionsButton != null) {
				OptionsButton.Dispose ();
				OptionsButton = null;
			}

			if (PlayButton != null) {
				PlayButton.Dispose ();
				PlayButton = null;
			}

			if (ProgressBarView != null) {
				ProgressBarView.Dispose ();
				ProgressBarView = null;
			}

			if (ReferenceButton != null) {
				ReferenceButton.Dispose ();
				ReferenceButton = null;
			}

			if (RemainingLabel != null) {
				RemainingLabel.Dispose ();
				RemainingLabel = null;
			}

			if (ShuffleButton != null) {
				ShuffleButton.Dispose ();
				ShuffleButton = null;
			}

			if (SubtitleLabel != null) {
				SubtitleLabel.Dispose ();
				SubtitleLabel = null;
			}

			if (TitleClickableArea != null) {
				TitleClickableArea.Dispose ();
				TitleClickableArea = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (ContentWidthHelper != null) {
				ContentWidthHelper.Dispose ();
				ContentWidthHelper = null;
			}

			if (ProgressBarWidthConstraint != null) {
				ProgressBarWidthConstraint.Dispose ();
				ProgressBarWidthConstraint = null;
			}

		}
	}
}

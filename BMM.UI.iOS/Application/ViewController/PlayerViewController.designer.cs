// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;
using FFImageLoading.Cross;

namespace BMM.UI.iOS
{
	[Register ("PlayerViewController")]
	partial class PlayerViewController
	{
		[Outlet]
		UIKit.UIButton BtvLinkButton { get; set; }

		[Outlet]
		UIKit.UIStackView BtvLinkContainer { get; set; }

		[Outlet]
		UIKit.UISlider BufferedProgressSlider { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CoverViewTopConstraint { get; set; }

		[Outlet]
		UIKit.UISlider PlayingProgressSlider { get; set; }

		[Outlet]
		UIKit.UIButton QueueNextButton { get; set; }

		[Outlet]
		UIKit.UIButton QueuePreviousButton { get; set; }

		[Outlet]
		UIKit.UIButton QueueRandomButton { get; set; }

		[Outlet]
		UIKit.UIButton QueueRepeatButton { get; set; }

		[Outlet]
		UIKit.UIButton SkipBackwardButton { get; set; }

		[Outlet]
		UIKit.UIButton SkipForwardButton { get; set; }

		[Outlet ("SubtitleLabel")]
		UIKit.UILabel subtitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TimePlayedLabel { get; set; }

		[Outlet]
		UIKit.UILabel TimeTotalLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
		UIKit.UIView TrackCoverContainerView { get; set; }

		[Outlet]
		MvxCachedImageView TrackCoverImageView { get; set; }

		[Outlet]
		UIKit.UIButton TrackOptionsButton { get; set; }

		[Outlet]
		UIKit.UIButton TrackPlayPauseButton { get; set; }

		[Outlet]
		UIKit.UIButton TrackReferernceButton { get; set; }

		[Outlet]
		UIKit.UILabel TrackSubtitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TrackTitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BtvLinkButton != null) {
				BtvLinkButton.Dispose ();
				BtvLinkButton = null;
			}

			if (BtvLinkContainer != null) {
				BtvLinkContainer.Dispose ();
				BtvLinkContainer = null;
			}

			if (BufferedProgressSlider != null) {
				BufferedProgressSlider.Dispose ();
				BufferedProgressSlider = null;
			}

			if (PlayingProgressSlider != null) {
				PlayingProgressSlider.Dispose ();
				PlayingProgressSlider = null;
			}

			if (QueueNextButton != null) {
				QueueNextButton.Dispose ();
				QueueNextButton = null;
			}

			if (QueuePreviousButton != null) {
				QueuePreviousButton.Dispose ();
				QueuePreviousButton = null;
			}

			if (QueueRandomButton != null) {
				QueueRandomButton.Dispose ();
				QueueRandomButton = null;
			}

			if (QueueRepeatButton != null) {
				QueueRepeatButton.Dispose ();
				QueueRepeatButton = null;
			}

			if (SkipBackwardButton != null) {
				SkipBackwardButton.Dispose ();
				SkipBackwardButton = null;
			}

			if (SkipForwardButton != null) {
				SkipForwardButton.Dispose ();
				SkipForwardButton = null;
			}

			if (subtitleLabel != null) {
				subtitleLabel.Dispose ();
				subtitleLabel = null;
			}

			if (TimePlayedLabel != null) {
				TimePlayedLabel.Dispose ();
				TimePlayedLabel = null;
			}

			if (TimeTotalLabel != null) {
				TimeTotalLabel.Dispose ();
				TimeTotalLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TrackCoverContainerView != null) {
				TrackCoverContainerView.Dispose ();
				TrackCoverContainerView = null;
			}

			if (TrackCoverImageView != null) {
				TrackCoverImageView.Dispose ();
				TrackCoverImageView = null;
			}

			if (TrackOptionsButton != null) {
				TrackOptionsButton.Dispose ();
				TrackOptionsButton = null;
			}

			if (TrackPlayPauseButton != null) {
				TrackPlayPauseButton.Dispose ();
				TrackPlayPauseButton = null;
			}

			if (TrackReferernceButton != null) {
				TrackReferernceButton.Dispose ();
				TrackReferernceButton = null;
			}

			if (TrackSubtitleLabel != null) {
				TrackSubtitleLabel.Dispose ();
				TrackSubtitleLabel = null;
			}

			if (TrackTitleLabel != null) {
				TrackTitleLabel.Dispose ();
				TrackTitleLabel = null;
			}

			if (CoverViewTopConstraint != null) {
				CoverViewTopConstraint.Dispose ();
				CoverViewTopConstraint = null;
			}

		}
	}
}

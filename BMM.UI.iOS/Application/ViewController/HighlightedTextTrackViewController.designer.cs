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
	[Register ("HighlightedTextTrackViewController")]
	partial class HighlightedTextTrackViewController
	{
		[Outlet]
		BMM.UI.iOS.DownloadButton AddToButton { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint BottomFloatingLayoutToBottomConstraint { get; set; }

		[Outlet]
		UIKit.UIView BottomFloatingView { get; set; }

		[Outlet]
		UIKit.UIView CloseButtonContainer { get; set; }

		[Outlet]
		UIKit.UITableView HighlightedTextsTableView { get; set; }

		[Outlet]
		UIKit.UILabel MetaLabel { get; set; }

		[Outlet]
		UIKit.UIButton OptionsButton { get; set; }

		[Outlet]
		LottieButton PlayButton { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint SafeAreaCoveringView { get; set; }

		[Outlet]
		UIKit.UIView Separator { get; set; }

		[Outlet]
		UIKit.UILabel SubtitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AddToButton != null) {
				AddToButton.Dispose ();
				AddToButton = null;
			}

			if (BottomFloatingView != null) {
				BottomFloatingView.Dispose ();
				BottomFloatingView = null;
			}

			if (CloseButtonContainer != null) {
				CloseButtonContainer.Dispose ();
				CloseButtonContainer = null;
			}

			if (HighlightedTextsTableView != null) {
				HighlightedTextsTableView.Dispose ();
				HighlightedTextsTableView = null;
			}

			if (MetaLabel != null) {
				MetaLabel.Dispose ();
				MetaLabel = null;
			}

			if (OptionsButton != null) {
				OptionsButton.Dispose ();
				OptionsButton = null;
			}

			if (PlayButton != null) {
				PlayButton.Dispose ();
				PlayButton = null;
			}

			if (Separator != null) {
				Separator.Dispose ();
				Separator = null;
			}

			if (SubtitleLabel != null) {
				SubtitleLabel.Dispose ();
				SubtitleLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (BottomFloatingLayoutToBottomConstraint != null) {
				BottomFloatingLayoutToBottomConstraint.Dispose ();
				BottomFloatingLayoutToBottomConstraint = null;
			}

			if (SafeAreaCoveringView != null) {
				SafeAreaCoveringView.Dispose ();
				SafeAreaCoveringView = null;
			}

		}
	}
}

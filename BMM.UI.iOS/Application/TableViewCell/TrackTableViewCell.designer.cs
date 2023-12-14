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
	[Register ("TrackTableViewCell")]
	partial class TrackTableViewCell
	{
		[Outlet]
		UIKit.UILabel accessoryView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		FFImageLoading.Cross.MvxCachedImageView DownloadStatusImageView { get; set; }

		[Outlet]
		UIKit.UILabel metaLabel { get; set; }

		[Outlet]
		UIKit.UIButton OptionsButton { get; set; }

		[Outlet]
		UIKit.UIButton ReferenceButton { get; set; }

		[Outlet]
		UIKit.UIImageView StatusImage { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint StatusImageWidthConstraint { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (accessoryView != null) {
				accessoryView.Dispose ();
				accessoryView = null;
			}

			if (DownloadStatusImageView != null) {
				DownloadStatusImageView.Dispose ();
				DownloadStatusImageView = null;
			}

			if (metaLabel != null) {
				metaLabel.Dispose ();
				metaLabel = null;
			}

			if (OptionsButton != null) {
				OptionsButton.Dispose ();
				OptionsButton = null;
			}

			if (ReferenceButton != null) {
				ReferenceButton.Dispose ();
				ReferenceButton = null;
			}

			if (StatusImage != null) {
				StatusImage.Dispose ();
				StatusImage = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (StatusImageWidthConstraint != null) {
				StatusImageWidthConstraint.Dispose ();
				StatusImageWidthConstraint = null;
			}

		}
	}
}

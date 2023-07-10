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
	[Register ("RecommendationAlbumTableViewCell")]
	partial class RecommendationAlbumTableViewCell
	{
		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView AlbumImage { get; set; }

		[Outlet]
		UIKit.UILabel AlbumName { get; set; }

		[Outlet]
		UIKit.UILabel RemoteSubtitleLabel { get; set; }

		[Outlet]
		UIKit.UIView RemoteTitleContainer { get; set; }

		[Outlet]
		UIKit.UILabel RemoteTitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TitleToBottomViewConstraint { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AlbumImage != null) {
				AlbumImage.Dispose ();
				AlbumImage = null;
			}

			if (AlbumName != null) {
				AlbumName.Dispose ();
				AlbumName = null;
			}

			if (RemoteSubtitleLabel != null) {
				RemoteSubtitleLabel.Dispose ();
				RemoteSubtitleLabel = null;
			}

			if (RemoteTitleContainer != null) {
				RemoteTitleContainer.Dispose ();
				RemoteTitleContainer = null;
			}

			if (RemoteTitleLabel != null) {
				RemoteTitleLabel.Dispose ();
				RemoteTitleLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TitleToBottomViewConstraint != null) {
				TitleToBottomViewConstraint.Dispose ();
				TitleToBottomViewConstraint = null;
			}

		}
	}
}

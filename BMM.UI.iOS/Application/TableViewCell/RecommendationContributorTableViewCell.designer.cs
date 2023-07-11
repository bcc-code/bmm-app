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
	[Register ("RecommendationContributorTableViewCell")]
	partial class RecommendationContributorTableViewCell
	{
		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView ContributorImage { get; set; }

		[Outlet]
		UIKit.UILabel ContributorName { get; set; }

		[Outlet]
		UIKit.UILabel RemoteSubitleLabel { get; set; }

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
			if (ContributorImage != null) {
				ContributorImage.Dispose ();
				ContributorImage = null;
			}

			if (ContributorName != null) {
				ContributorName.Dispose ();
				ContributorName = null;
			}

			if (RemoteSubitleLabel != null) {
				RemoteSubitleLabel.Dispose ();
				RemoteSubitleLabel = null;
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

			if (RemoteTitleContainer != null) {
				RemoteTitleContainer.Dispose ();
				RemoteTitleContainer = null;
			}

		}
	}
}

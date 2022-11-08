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
	[Register ("YearInReviewTeaserExpandedCell")]
	partial class YearInReviewTeaserExpandedCell
	{
		[Outlet]
		UIKit.UIButton CollapseButton { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		BMM.UI.iOS.LeftImageButton PlaylistButton { get; set; }

		[Outlet]
		UIKit.UIButton SeeReviewButton { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CollapseButton != null) {
				CollapseButton.Dispose ();
				CollapseButton = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (SeeReviewButton != null) {
				SeeReviewButton.Dispose ();
				SeeReviewButton = null;
			}

			if (PlaylistButton != null) {
				PlaylistButton.Dispose ();
				PlaylistButton = null;
			}

		}
	}
}

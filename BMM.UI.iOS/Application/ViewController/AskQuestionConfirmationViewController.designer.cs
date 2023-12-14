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
	[Register ("AskQuestionConfirmationViewController")]
	partial class AskQuestionConfirmationViewController
	{
		[Outlet]
		UIKit.UIView BottomView { get; set; }

		[Outlet]
		UIKit.UIView CloseButton { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.UIImageView IconImage { get; set; }

		[Outlet]
		UIKit.UIButton GotItButton { get; set; }

		[Outlet]
		UIKit.UILabel ThankYouLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BottomView != null) {
				BottomView.Dispose ();
				BottomView = null;
			}

			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}

			if (GotItButton != null) {
				GotItButton.Dispose ();
				GotItButton = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (ThankYouLabel != null) {
				ThankYouLabel.Dispose ();
				ThankYouLabel = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (IconImage != null) {
				IconImage.Dispose ();
				IconImage = null;
			}

		}
	}
}

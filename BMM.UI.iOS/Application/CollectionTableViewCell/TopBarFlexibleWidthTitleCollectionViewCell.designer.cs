// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using UIKit;

namespace BMM.UI.iOS.CollectionTableViewCell
{
	[Register ("TopBarFlexibleWidthTitleCollectionViewCell")]
	partial class TopBarFlexibleWidthTitleCollectionViewCell
	{
		[Outlet]
		UIKit.UIView MainView { get; set; }

		[Outlet]
		UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (MainView != null) {
				MainView.Dispose ();
				MainView = null;
			}

		}
	}
}

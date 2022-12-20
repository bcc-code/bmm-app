// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;
using BMM.UI.iOS.CustomViews;

namespace BMM.UI.iOS
{
	[Register ("VideoTileViewCell")]
	partial class VideoTileViewCell
	{
		[Outlet]
		VideoView VideoView { get; set; }

		[Outlet]
		UIKit.UIButton BottomButton { get; set; }

		[Outlet]
		UIKit.UILabel HeaderLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (VideoView != null) {
				VideoView.Dispose ();
				VideoView = null;
			}

			if (BottomButton != null) {
				BottomButton.Dispose ();
				BottomButton = null;
			}

			if (HeaderLabel != null) {
				HeaderLabel.Dispose ();
				HeaderLabel = null;
			}

		}
	}
}

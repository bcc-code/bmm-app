﻿// WARNING
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
	[Register ("MessageTileViewCell")]
	partial class MessageTileViewCell
	{
		[Outlet]
		UIKit.UIView BackgroundView { get; set; }

		[Outlet]
		UIKit.UIButton BottomButton { get; set; }

		[Outlet]
		UIKit.UILabel HeaderLabel { get; set; }

		[Outlet]
		UIKit.UILabel SubtitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (HeaderLabel != null) {
				HeaderLabel.Dispose ();
				HeaderLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (BottomButton != null) {
				BottomButton.Dispose ();
				BottomButton = null;
			}

			if (SubtitleLabel != null) {
				SubtitleLabel.Dispose ();
				SubtitleLabel = null;
			}

		}
	}
}

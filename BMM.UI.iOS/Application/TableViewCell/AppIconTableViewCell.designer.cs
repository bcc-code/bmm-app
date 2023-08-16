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
	[Register ("AppIconTableViewCell")]
	partial class AppIconTableViewCell
	{
		[Outlet]
		FFImageLoading.Cross.MvxCachedImageView IconImageView { get; set; }

		[Outlet]
		UIKit.UIImageView IsSelectedImage { get; set; }

		[Outlet]
		UIKit.UILabel TextLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (IsSelectedImage != null) {
				IsSelectedImage.Dispose ();
				IsSelectedImage = null;
			}

			if (TextLabel != null) {
				TextLabel.Dispose ();
				TextLabel = null;
			}

			if (IconImageView != null) {
				IconImageView.Dispose ();
				IconImageView = null;
			}

		}
	}
}

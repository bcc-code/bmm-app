// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using FFImageLoading.Cross;
using UIKit;

namespace BMM.UI.iOS
{
    [Register ("PinnedItemTableViewCell")]
    partial class PinnedItemTableViewCell
    {
        [Outlet]
        UIKit.UILabel TitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MvxCachedImageView TypeImage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }

            if (TypeImage != null) {
                TypeImage.Dispose ();
                TypeImage = null;
            }
        }
    }
}
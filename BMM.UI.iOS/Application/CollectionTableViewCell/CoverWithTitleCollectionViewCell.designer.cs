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

namespace BMM.UI.iOS
{
    [Register ("CoverWithTitleCollectionViewCell")]
    partial class CoverWithTitleCollectionViewCell
    {
        [Outlet]
        MvxCachedImageView ImageView { get; set; }

        [Outlet]
        UIKit.UILabel TitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ImageView != null) {
                ImageView.Dispose ();
                ImageView = null;
            }

            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }
        }
    }
}
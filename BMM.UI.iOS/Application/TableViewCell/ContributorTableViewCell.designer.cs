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
    [Register ("ContributorTableViewCell")]
    partial class ContributorTableViewCell
    {
        [Outlet]
        MvxCachedImageView CoverImageView { get; set; }

        [Outlet]
        UIKit.UIButton OptionsButton { get; set; }


        [Outlet]
        UIKit.UILabel TitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CoverImageView != null) {
                CoverImageView.Dispose ();
                CoverImageView = null;
            }

            if (OptionsButton != null) {
                OptionsButton.Dispose ();
                OptionsButton = null;
            }

            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }
        }
    }
}
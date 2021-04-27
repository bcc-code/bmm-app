// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BMM.UI.iOS
{
    [Register ("OidcLoginViewController")]
    partial class OidcLoginViewController
    {
        [Outlet]
        UIKit.UIImageView LoadingSpinnerImageView { get; set; }


        [Outlet]
        UIKit.UIButton LoginButton { get; set; }


        [Outlet]
        UIKit.UIImageView logoImageView { get; set; }


        [Outlet]
        UIKit.UILabel SubtitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (LoadingSpinnerImageView != null) {
                LoadingSpinnerImageView.Dispose ();
                LoadingSpinnerImageView = null;
            }

            if (LoginButton != null) {
                LoginButton.Dispose ();
                LoginButton = null;
            }

            if (logoImageView != null) {
                logoImageView.Dispose ();
                logoImageView = null;
            }

            if (SubtitleLabel != null) {
                SubtitleLabel.Dispose ();
                SubtitleLabel = null;
            }
        }
    }
}
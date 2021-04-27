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
    [Register ("UserSetupViewController")]
    partial class UserSetupViewController
    {
        [Outlet] 
        UIKit.UILabel SettingUpMessage { get; set; }
        
        [Outlet]
        UIKit.UIImageView LoadingSpinnerImageView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (LoadingSpinnerImageView != null) {
                LoadingSpinnerImageView.Dispose ();
                LoadingSpinnerImageView = null;
            }
        }
    }
}
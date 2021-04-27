// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace BMM.UI.iOS
{
    [Register ("SupportEndedViewController")]
    partial class SupportEndedViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SupportEndedMessage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton UpdateBmmBtn { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (SupportEndedMessage != null) {
                SupportEndedMessage.Dispose ();
                SupportEndedMessage = null;
            }

            if (UpdateBmmBtn != null) {
                UpdateBmmBtn.Dispose ();
                UpdateBmmBtn = null;
            }
        }
    }
}
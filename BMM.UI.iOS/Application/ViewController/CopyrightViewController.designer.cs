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
    [Register ("CopyrightViewController")]
    partial class CopyrightViewController
    {
        [Outlet]
        UIKit.UITextView CopyrightTextView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CopyrightTextView != null) {
                CopyrightTextView.Dispose ();
                CopyrightTextView = null;
            }
        }
    }
}
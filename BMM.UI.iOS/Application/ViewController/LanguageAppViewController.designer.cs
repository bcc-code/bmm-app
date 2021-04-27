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
    [Register ("LanguageAppViewController")]
    partial class LanguageAppViewController
    {
        [Outlet]
        UIKit.UITableView LanguagesTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (LanguagesTableView != null) {
                LanguagesTableView.Dispose ();
                LanguagesTableView = null;
            }
        }
    }
}
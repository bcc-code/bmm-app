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
    [Register ("AutomaticDownloadViewController")]
    partial class AutomaticDownloadViewController
    {
        [Outlet]
        UIKit.UITableView AutomaticDownloadTableView { get; set; }


        [Outlet]
        UIKit.UILabel SubtitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AutomaticDownloadTableView != null) {
                AutomaticDownloadTableView.Dispose ();
                AutomaticDownloadTableView = null;
            }

            if (SubtitleLabel != null) {
                SubtitleLabel.Dispose ();
                SubtitleLabel = null;
            }
        }
    }
}
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
    [Register ("TrackInfoViewController")]
    partial class TrackInfoViewController
    {
        [Outlet]
        UIKit.UITableView TrackInfoTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (TrackInfoTableView != null) {
                TrackInfoTableView.Dispose ();
                TrackInfoTableView = null;
            }
        }
    }
}
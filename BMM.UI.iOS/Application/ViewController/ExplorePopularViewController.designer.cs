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
    [Register ("ExplorePopularViewController")]
    partial class ExplorePopularViewController
    {
        [Outlet]
        UIKit.UITableView TrackTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (TrackTableView != null) {
                TrackTableView.Dispose ();
                TrackTableView = null;
            }
        }
    }
}
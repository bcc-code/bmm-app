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
    [Register ("LibraryArchiveViewController")]
    partial class LibraryArchiveViewController
    {
        [Outlet]
        UIKit.UITableView ArchiveTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ArchiveTableView != null) {
                ArchiveTableView.Dispose ();
                ArchiveTableView = null;
            }
        }
    }
}
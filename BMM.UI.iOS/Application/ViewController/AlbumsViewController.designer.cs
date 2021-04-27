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
    [Register ("AlbumsViewController")]
    partial class AlbumsViewController
    {
        [Outlet]
        UIKit.UITableView AlbumsTable { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AlbumsTable != null) {
                AlbumsTable.Dispose ();
                AlbumsTable = null;
            }
        }
    }
}
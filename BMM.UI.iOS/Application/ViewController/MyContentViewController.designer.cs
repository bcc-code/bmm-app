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
    [Register ("MyContentViewController")]
    partial class MyContentViewController
    {
        [Outlet]
        UIKit.UITableView MyCollectionTable { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MyCollectionTable != null) {
                MyCollectionTable.Dispose ();
                MyCollectionTable = null;
            }
        }
    }
}
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
    [Register ("LibraryViewController")]
    partial class LibraryViewController
    {
        [Outlet]
        UIKit.UIView ContentView { get; set; }
        [Outlet]
        UIKit.UISegmentedControl TabsSegmentedControl { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ContentView != null) {
                ContentView.Dispose ();
                ContentView = null;
            }

            if (TabsSegmentedControl != null) {
                TabsSegmentedControl.Dispose ();
                TabsSegmentedControl = null;
            }
        }
    }
}
using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BMM.UI.iOS.CustomViews
{
    [Register(nameof(BmmSeachTextField)), DesignTimeVisible(true)]
    public class BmmSeachTextField : UITextField
    {
        private readonly int IconSize = 24;
        private readonly int Margin = 8;
        
        public BmmSeachTextField()
        {
        }

        public BmmSeachTextField(ObjCRuntime.NativeHandle handle) : base(handle)
        {
        }

        public BmmSeachTextField(CGRect frame) : base(frame)
        {
        }

        public override CGRect TextRect(CGRect forBounds)
        {
            return new CGRect(forBounds.X + IconSize + Margin * 2, forBounds.Y, forBounds.Width - (IconSize + Margin) * 2, forBounds.Height);
        }

        public override CGRect EditingRect(CGRect forBounds)
        {
            return new CGRect(forBounds.X + IconSize + Margin * 2, forBounds.Y, forBounds.Width - (IconSize + Margin) * 2, forBounds.Height);
        }
    }
}
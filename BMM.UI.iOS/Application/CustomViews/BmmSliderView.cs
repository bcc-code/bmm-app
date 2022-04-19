using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    [Register(nameof(BmmSliderView)), DesignTimeVisible(true)]
    public class BmmSliderView : UISlider
    {
        public BmmSliderView()
        {
        }

        public BmmSliderView(IntPtr handle) : base(handle)
        {
        }

        public BmmSliderView(CGRect frame) : base(frame)
        {
        }
        
        public override bool BeginTracking(UITouch uitouch, UIEvent? uievent) => true;
    }
}
using System;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using Foundation;
using MvvmCross.Commands;
using UIKit;

namespace BMM.UI.iOS
{
    [Register(nameof(BmmSliderView)), DesignTimeVisible(true)]
    public class BmmSliderView : UISlider
    {
        public BmmSliderView()
        {
        }

        public BmmSliderView(ObjCRuntime.NativeHandle handle) : base(handle)
        {
        }

        public BmmSliderView(CGRect frame) : base(frame)
        {
        }
        
        public IMvxCommand<long> SliderTouchedCommand { get; set; }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if (touches.Any())
            {
                var locationInView = ((UITouch)touches.First()).LocationInView(this);
                var touchedValue = locationInView.X / Frame.Width * MaxValue;
                SliderTouchedCommand?.Execute(Convert.ToInt64(touchedValue));
            }
            
            base.TouchesEnded(touches, evt);
        }
    }
}
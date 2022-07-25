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

        public BmmSliderView(IntPtr handle) : base(handle)
        {
        }

        public BmmSliderView(CGRect frame) : base(frame)
        {
        }
        
        public IMvxCommand<float> SliderTouchedCommand { get; set; }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if (touches.Any())
            {
                var locationInView = ((UITouch)touches.First()).LocationInView(this);
                var touchedValue = (locationInView.X / Frame.Width) * MaxValue;
                SliderTouchedCommand?.Execute((float)touchedValue);
            }
            
            base.TouchesEnded(touches, evt);
        }
    }
}
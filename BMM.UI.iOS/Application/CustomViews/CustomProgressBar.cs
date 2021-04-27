using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    /// <summary>
    /// We can't use the UIProgressBar because values lower than 0,1 are displayed as 0,1.
    /// We can't use the UISeekbar either because the start and end have a corner radius.
    /// </summary>
    [Register("CustomProgressBar")]
    public class CustomProgressBar : UIView
    {
        private float _progress;
        private UIColor _progressColor;

        public float Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                SetNeedsDisplay();
            }
        }

        [Export("ProgressColor"), Browsable(true)]
        public UIColor ProgressColor
        {
            get => _progressColor;
            set
            {
                _progressColor = value;
                SetNeedsDisplay();
            }
        }

        public CustomProgressBar(IntPtr handle) : base(handle)
        { }

        public override void Draw(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext();
            if (context == null)
                return;

            context.SetStrokeColor(ProgressColor.CGColor);
            context.BeginPath();
            context.SetLineWidth(rect.Height);
            context.MoveTo(0, rect.Height / 2);
            context.AddLineToPoint(rect.Width * Progress, rect.Height / 2);
            context.SetLineCap(CGLineCap.Butt);
            context.StrokePath();
        }
    }
}
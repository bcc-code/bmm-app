using System;
using System.Linq;
using BMM.UI.iOS.Constants;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    [Register(nameof(Separator))]
    public class Separator : UIView
    {
        public Separator()
        {
            InitUi();
        }

        public Separator(NSCoder coder) : base(coder)
        {
            InitUi();
        }

        protected Separator(NSObjectFlag t) : base(t)
        {
            InitUi();
        }

        protected internal Separator(IntPtr handle) : base(handle)
        {
            InitUi();
        }

        public Separator(CGRect frame) : base(frame)
        {
            InitUi();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            InitUi();
        }

        private void InitUi()
        {
            // Xcode/Interface Builder has some serious problems with constraint constants lower thant 1 therefore we set it here
            var heightConstraints = Constraints.Where(x => x.FirstAttribute == NSLayoutAttribute.Height);
            RemoveConstraints(heightConstraints.ToArray());
            HeightAnchor.ConstraintEqualTo(0.5f).Active = true;
            BackgroundColor = AppColors.SeparatorColor;
        }
    }
}
using System;
using System.ComponentModel;
using System.Drawing;
using BMM.Core.Constants;
using BMM.Core.Models.POs.BibleStudy;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using ObjCRuntime;
using UIKit;

namespace BMM.UI.iOS.CustomViews
{
    public partial class HvheHeaderTableViewCell : BaseBMMTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName(nameof(HvheHeaderTableViewCell), NSBundle.MainBundle);
        public static readonly NSString Key = new(nameof(HvheHeaderTableViewCell));

        public HvheHeaderTableViewCell(NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(Bind);
        }

        protected override bool HasHighlightEffect => false;

        private void Bind()
        {
            var set = this.CreateBindingSet<HvheHeaderTableViewCell, HvheHeaderPO>();
            
            set.Bind(LeftItemLabel)
                .To(po => po.LeftItemLabel);
            
            set.Bind(RightItemLabel)
                .To(po => po.RightItemLabel);
            
            set.Apply();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }

        private void SetThemes()
        {
            LeftItemLabel.ApplyTextTheme(AppTheme.Subtitle2Label3);
            RightItemLabel.ApplyTextTheme(AppTheme.Subtitle2Label3);
        }
    }
}
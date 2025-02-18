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
    public partial class HvheChurchesSelectorTableViewCell : BaseBMMTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName(nameof(HvheChurchesSelectorTableViewCell), NSBundle.MainBundle);
        public static readonly NSString Key = new(nameof(HvheChurchesSelectorTableViewCell));
        private bool _isLeftItemSelected;
        private bool _isRightItemSelected;

        public HvheChurchesSelectorTableViewCell(NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(Bind);
        }

        protected override bool HasHighlightEffect => false;

        private void Bind()
        {
            var set = this.CreateBindingSet<HvheChurchesSelectorTableViewCell, HvheChurchesSelectorPO>();

            set.Bind(LeftItemLabel)
                .To(po => po.LeftItemTitle);
            
            set.Bind(RightItemLabel)
                .To(po => po.RightItemTitle);

            set.Bind(LeftItemContainer)
                .For(v => v.BindTap())
                .To(po => po.LeftItemSelectedCommand);

            set.Bind(RightItemContainer)
                .For(v => v.BindTap())
                .To(po => po.RightItemSelectedCommand);
            
            set.Bind(this)
                .For(v => v.IsLeftItemSelected)
                .To(po => po.IsLeftItemSelected);
            
            set.Bind(this)
                .For(v => v.IsRightItemSelected)
                .To(po => po.IsRightItemSelected);
            
            set.Apply();
        }

        public bool IsRightItemSelected
        {
            get => _isRightItemSelected;
            set
            {
                _isRightItemSelected = value;
                Animate(ViewConstants.DefaultAnimationDuration,
                    () =>
                    {
                        RightIndicatorView.Hidden = !_isRightItemSelected;
                        UpdateTextColor();
                    });
            }
        }

        public bool IsLeftItemSelected
        {
            get => _isLeftItemSelected;
            set
            {
                _isLeftItemSelected = value;
                Animate(ViewConstants.DefaultAnimationDuration,
                    () =>
                    {
                        LeftIndicatorView.Hidden = !_isLeftItemSelected;
                        UpdateTextColor();
                    });
            }
        }
        
        private void UpdateTextColor()
        {
            LeftItemLabel.TextColor = GetTextColor(IsLeftItemSelected);
            RightItemLabel.TextColor = GetTextColor(IsRightItemSelected);
        }

        private UIColor GetTextColor(bool isSelected)
        {
            return isSelected
                ? AppColors.LabelOneColor
                : AppColors.LabelThreeColor;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }

        private void SetThemes()
        {
            LeftItemLabel.ApplyTextTheme(AppTheme.Title2);
            RightItemLabel.ApplyTextTheme(AppTheme.Title2);
        }
    }
}
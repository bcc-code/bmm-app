using System;
using System.ComponentModel;
using System.Drawing;
using BMM.Api.Implementation.Models;
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
using UIKit;

namespace BMM.UI.iOS.CustomViews
{
    [DesignTimeVisible(true)]
    public partial class HvheChurchesSelectorView : MvxView
    {
        private bool _isRightItemSelected;
        private bool _isLeftItemSelected;
        public static readonly UINib Nib = UINib.FromName(nameof(HvheChurchesSelectorView), NSBundle.MainBundle);
        public static readonly NSString Key = new NSString(nameof(HvheChurchesSelectorView));

        public HvheChurchesSelectorView()
        {
            Initialize();
        }
        
        public HvheChurchesSelectorView(ObjCRuntime.NativeHandle handle) : base(handle)
        {
            Initialize();
        }
        
        private void Initialize()
        {
            this.LoadXib(true);
            this.CreateBindingContext();
            this.DelayBind(Bind);
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<HvheChurchesSelectorView, HvheChurchesSelectorPO>();

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
            SetThemes();
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
        
        private void SetThemes()
        {
            LeftItemLabel.ApplyTextTheme(AppTheme.Title2);
            RightItemLabel.ApplyTextTheme(AppTheme.Title2);
            UpdateTextColor();
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
    }
}
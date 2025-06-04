using System;
using System.ComponentModel;
using System.Drawing;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS.CustomViews
{
    [DesignTimeVisible(true)]
    public partial class ProgressBarView : MvxView
    {
        public static readonly UINib Nib = UINib.FromName(nameof(ProgressBarView), NSBundle.MainBundle);
        public static readonly NSString Key = new NSString(nameof(ProgressBarView));

        private int _percentage;
        private UIView _foregroundView;
        private UIView _backgroundView;
        private UIColor _barBackgroundColor;
        private UIColor _barForegroundColor;

        public ProgressBarView(RectangleF bounds) : base(bounds)
        {
            Initialize();
        }

        public ProgressBarView(ObjCRuntime.NativeHandle handle) : base(handle)
        {
            Initialize();
        }

        public int Percentage
        {
            get => _percentage;
            set
            {
                _percentage = value;
                SetNeedsLayout();
            }
        }
        
        public UIColor BarBackgroundColor
        {
            get => _barBackgroundColor;
            set
            {
                _barBackgroundColor = value;
                SetNeedsLayout();
            }
        }
        
        public UIColor BarForegroundColor
        {
            get => _barForegroundColor;
            set
            {
                _barForegroundColor = value;
                SetNeedsLayout();
            }
        }

        private void Initialize()
        {
            BarBackgroundColor = AppColors.SeparatorColor;
            BarForegroundColor = AppColors.GlobalBlackOneColor;
            
            this.LoadXib(true);
            this.CreateBindingContext();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            
            _backgroundView?.RemoveFromSuperview();
            _foregroundView?.RemoveFromSuperview();
            
            _backgroundView = new UIView(new CGRect(0, 0, Frame.Width, Frame.Height))
            {
                BackgroundColor = BarBackgroundColor
            };
            
            Add(_backgroundView);
            
            var desiredWidth = Frame.Width * Percentage / 100f;
            
            _foregroundView = new UIView(new CGRect(0, 0, desiredWidth, Frame.Height))
            {
                BackgroundColor = BarForegroundColor
            };

            Add(_foregroundView);
        }
    }
}
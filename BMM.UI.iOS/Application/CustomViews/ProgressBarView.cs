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
        private CAShapeLayer _foregroundLayer;
        private CAShapeLayer _backgroundLayer;

        public ProgressBarView(RectangleF bounds) : base(bounds)
        {
            Initialize();
        }

        public ProgressBarView(IntPtr handle) : base(handle)
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

        private void Initialize()
        {
            this.LoadXib(true);
            this.CreateBindingContext();
        }

        private void SetDimensions()
        {
            HeightConstraint.Constant = 5;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _backgroundLayer?.RemoveFromSuperLayer();
            _foregroundLayer?.RemoveFromSuperLayer();

            var backgroundPath = UIBezierPath.FromRect(new CGRect(0, 0, Frame.Width, Frame.Height));

            _backgroundLayer = new CAShapeLayer
            {
                Path = backgroundPath.CGPath,
                FillColor = AppColors.OnColorFiveColor.CGColor,
                ContentsScale = UIScreen.MainScreen.Scale
            };

            Layer.AddSublayer(_backgroundLayer);

            var desiredWidth = Frame.Width * Percentage / 100f;
            var foregroundPath = UIBezierPath.FromRect(new CGRect(0, 0, desiredWidth, Frame.Height));

            _foregroundLayer = new CAShapeLayer
            {
                Path = foregroundPath.BezierPathByReversingPath().CGPath,
                FillColor = AppColors.OnColorOneColor.CGColor,
                ContentsScale = UIScreen.MainScreen.Scale
            };

            Layer.AddSublayer(_foregroundLayer);
        }
    }
}
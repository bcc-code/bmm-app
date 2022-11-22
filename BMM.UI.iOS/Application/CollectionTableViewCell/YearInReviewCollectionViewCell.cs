using System;
using BMM.Core.Models.POs.YearInReview;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Utils;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;
using Xamarin.Essentials;
using AppTheme = BMM.UI.iOS.Constants.AppTheme;

namespace BMM.UI.iOS
{
    public partial class YearInReviewCollectionViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new(nameof(YearInReviewCollectionViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(YearInReviewCollectionViewCell), NSBundle.MainBundle);
        private string _shadowColor;
        
        public YearInReviewCollectionViewCell(IntPtr handle): base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<YearInReviewCollectionViewCell, YearInReviewItemPO>();

                set.Bind(YearInReviewImageView)
                    .For(v => v.ImagePath)
                    .To(po => po.Url);
                    
                set.Bind(SubtitleLabel)
                    .To(po => po.Subtitle);

                set.Bind(this)
                    .For(v => v.ShadowColor)
                    .To(po => po.Color);
                
                set.Apply();
            });
        }
        
        public static nfloat ImageHeight { get; set; }
        public static nfloat ImageWidth { get; set; }

        public string ShadowColor
        {
            get => _shadowColor;
            set
            {
                _shadowColor = value;
                var systemColor = ColorConverters.FromHex(value);
                ImageContainerView.Layer.ShadowColor = systemColor.ToPlatformColor().CGColor;
                YearInReviewImageView.BackgroundColor = systemColor.ToPlatformColor();
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            ImageHeightConstraint.Constant = ImageHeight;
            ImageWidthConstraint.Constant = ImageWidth;
            SubtitleLabel.ApplyTextTheme(AppTheme.Paragraph3);
            
            if (ThemeUtils.IsUsingDarkMode)
                return;
            
            ImageContainerView.Layer.BorderWidth = 0;
            ImageContainerView.Layer.ShadowOffset = new CGSize(0, 8);
            ImageContainerView.Layer.ShadowRadius = 8;
            ImageContainerView.Layer.ShadowOpacity = 0.8f;
            ImageContainerView.Layer.MasksToBounds = false;
        }
    }
}
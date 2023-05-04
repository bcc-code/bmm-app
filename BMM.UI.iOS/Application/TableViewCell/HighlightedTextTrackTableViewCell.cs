using MvvmCross.Binding.BindingContext;
using BMM.Core.Models.POs.Tracks;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using CoreAnimation;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class HighlightedTextTrackTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(HighlightedTextTrackTableViewCell));
        private CAGradientLayer _leftGradientLayer;
        private CAGradientLayer _rightGradientLayer;

        public HighlightedTextTrackTableViewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<HighlightedTextTrackTableViewCell, HighlightedTextTrackPO>();

                set.Bind(HighlightLabel)
                    .For(v => v.StyledTextContainer)
                    .To(po => po.StyledTextContainer);
                
                set.Bind(this)
                    .For(v => v.BindTap())
                    .To(po => po.ItemClickedCommand);
                
                set.Bind(this)
                    .For(v => v.RatioOfFirstHighlightPositionToFullText)
                    .To(po => po.RatioOfFirstHighlightPositionToFullText);
                
                set.Bind(this)
                    .For(v => v.RatioOfFirstHighlightLengthToFullText)
                    .To(po => po.RatioOfFirstHighlightLengthToFullText);
                
                set.Apply();
            });
        }

        public float RatioOfFirstHighlightPositionToFullText { get; set; }
        public float RatioOfFirstHighlightLengthToFullText { get; set; }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            AdjustLabelPosition();
        }

        private void AdjustLabelPosition()
        {
            var scrollContentWidth = LabelScrollView.ContentSize.Width;
            if (scrollContentWidth == 0)
                return;
            
            var scrollFrameWidth = LabelScrollView.Frame.Width;
            bool contentFitsInScrollView = scrollContentWidth <= scrollFrameWidth;
            
            if (contentFitsInScrollView)
                return;

            var maxOffsetToEnd = scrollContentWidth - scrollFrameWidth;
            var highlightWidth = RatioOfFirstHighlightLengthToFullText * scrollContentWidth;
            var desiredCenterOffset = scrollContentWidth * RatioOfFirstHighlightPositionToFullText - scrollFrameWidth / 2 + highlightWidth;
            desiredCenterOffset = (float)Math.Min(desiredCenterOffset, maxOffsetToEnd);
            LabelScrollView.ContentOffset = new CGPoint(Math.Max(0, desiredCenterOffset), 0);
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            LeftGradientView.ClipsToBounds = true;
            RightGradientView.ClipsToBounds = true;
            LabelScrollView.ScrollEnabled = false;
            SetGradientLayers();
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            SetGradientLayers();
        }

        private void SetGradientLayers()
        {
            _leftGradientLayer?.RemoveFromSuperLayer();
            _rightGradientLayer?.RemoveFromSuperLayer();
            
            _leftGradientLayer = CreateGradient(LeftGradientView.Bounds, false);
            _rightGradientLayer = CreateGradient(RightGradientView.Bounds, true);

            LeftGradientView.Layer.InsertSublayer(_leftGradientLayer, 0);
            RightGradientView.Layer.InsertSublayer(_rightGradientLayer, 0);
        }
        
        private CAGradientLayer CreateGradient(CGRect bounds, bool reverse)
        {
            var gradientColor = AppColors
                .HighlightningsGradientColor
                .GetResolvedColorSafe();
            
            var gradientColorWithAlpha = AppColors
                .HighlightningsGradientWithAlphaColor
                .GetResolvedColorSafe();
            
            var gradient = new CAGradientLayer();
            gradient.Frame = bounds;
            gradient.Colors = new[]
            {
                gradientColor.CGColor,
                gradientColorWithAlpha.CGColor,
            };

            if (reverse)
                gradient.Colors = gradient.Colors.Reverse().ToArray();

            gradient.StartPoint = new CGPoint(0f, 1f);
            gradient.EndPoint = new CGPoint(1f, 1f);
            return gradient;
        }
    }
}
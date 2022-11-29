using BMM.Api.Implementation.Models;
using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using UIKit;

namespace BMM.UI.iOS
{
    [Register(nameof(OrderingTrackTableViewCell))]
    public class OrderingTrackTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(OrderingTrackTableViewCell));
        private UILabel _titleLabel;
        private UILabel _subtitleLabel;
        private UILabel _metaLabel;

        public OrderingTrackTableViewCell(IntPtr handle)
            : base(handle)
        {
            InitUi();
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<OrderingTrackTableViewCell, TrackPO>();
                set.Bind(_titleLabel).To(vm => vm.TrackTitle);
                set.Bind(_subtitleLabel).To(vm => vm.TrackSubtitle);
                set.Bind(_metaLabel).To(vm => vm.TrackMeta);
                set.Apply();
            });
        }

        private void InitUi()
        {
            _titleLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _titleLabel.ApplyTextTheme(AppTheme.Title2);

            _subtitleLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Left
            };
            _subtitleLabel.ApplyTextTheme(AppTheme.Subtitle3Label2);

            _metaLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Left
            };
            _metaLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);

            ContentView.AddSubview(_titleLabel);
            ContentView.AddSubview(_subtitleLabel);
            ContentView.AddSubview(_metaLabel);

            _subtitleLabel.SetContentHuggingPriority(252, UILayoutConstraintAxis.Horizontal);
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                _titleLabel.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 10),
                _titleLabel.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 16),
                ContentView.TrailingAnchor.ConstraintEqualTo(_titleLabel.TrailingAnchor, 16),

                _subtitleLabel.HeightAnchor.ConstraintEqualTo(20),
                _subtitleLabel.TopAnchor.ConstraintEqualTo(_titleLabel.BottomAnchor),
                _subtitleLabel.LeadingAnchor.ConstraintEqualTo(_titleLabel.LeadingAnchor),
                ContentView.BottomAnchor.ConstraintEqualTo(_subtitleLabel.BottomAnchor, 10),

                _metaLabel.HeightAnchor.ConstraintEqualTo(_subtitleLabel.HeightAnchor),
                _metaLabel.FirstBaselineAnchor.ConstraintEqualTo(_subtitleLabel.FirstBaselineAnchor),
                _metaLabel.LeadingAnchor.ConstraintEqualTo(_subtitleLabel.TrailingAnchor, 4),
                ContentView.TrailingAnchor.ConstraintEqualTo(_metaLabel.TrailingAnchor, 16),
            });

            BackgroundColor = AppColors.BackgroundPrimaryColor;
        }
    }
}
using Airbnb.Lottie;
using BMM.Core.Constants;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Translation;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.NewMediaPlayer;
using BMM.UI.iOS.Utils;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class ExternalRelationsPlayWithSubtitleTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(ExternalRelationsPlayWithSubtitleTableViewCell));
        private bool _shouldShouldShowPlayAnimation;

        public ExternalRelationsPlayWithSubtitleTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ExternalRelationsPlayWithSubtitleTableViewCell, IBibleStudyExternalRelationPO>();
                
                set.Bind(PlayLabel)
                    .To(po => po.Title);
                
                set.Bind(SubtitleLabel)
                    .To(po => po.Subtitle);
                
                set.Bind(PlayButton)
                    .For(v => v.BindTap())
                    .To(po => po.ClickedCommand);

                set.Bind(this)
                    .For(v => v.ShouldShowPlayAnimation)
                    .To(v => v.ShouldShowPlayAnimation);
                
                set.Apply();
            });
        }

        public bool ShouldShowPlayAnimation
        {
            get => _shouldShouldShowPlayAnimation;
            set
            {
                _shouldShouldShowPlayAnimation = value;
                
                if (_shouldShouldShowPlayAnimation)
                    ShowPlayAnimation();
            }
        }

        private void ShowPlayAnimation()
        {
            PlayIcon.Hidden = true;
            var animation = ThemeUtils.GetLottieAnimationFor(LottieAnimationsNames.PlayAnimationIconDark, LottieAnimationsNames.PlayAnimationIcon);
            animation.BackgroundColor = UIColor.Clear;
            animation.LoopAnimation = true;
            
            animation.TranslatesAutoresizingMaskIntoConstraints = false;
            AnimationView.AddSubview(animation);

            NSLayoutConstraint.ActivateConstraints(
                new[]
                {
                    animation.LeadingAnchor.ConstraintEqualTo(AnimationView.LeadingAnchor),
                    animation.TrailingAnchor.ConstraintEqualTo(AnimationView.TrailingAnchor),
                    animation.TopAnchor.ConstraintEqualTo(AnimationView.TopAnchor),
                    animation.BottomAnchor.ConstraintEqualTo(AnimationView.BottomAnchor)
                });
            
            animation.Play();
        }

        private void SetThemes()
        {
            PlayLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
            SubtitleLabel.ApplyTextTheme(AppTheme.Paragraph2Label3);
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }
    }
}
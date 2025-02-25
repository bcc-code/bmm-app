using Airbnb.Lottie;
using BMM.Core.Constants;
using BMM.Core.Models.Enums;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.CustomViews;
using BMM.UI.iOS.Utils;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class AchievementDetailsViewController : BaseViewController<AchievementDetailsViewModel>
    {
        private const int CloseButtonHeight = 36;
        private bool _shouldShowConfetti;
        private bool _confettiShown;
        private LOTAnimationView _animationView;
        private bool _isCurrentlyPlaying;

        public AchievementDetailsViewController() : base(null)
        {
        }

        public AchievementDetailsViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            var set = this.CreateBindingSet<AchievementDetailsViewController, AchievementDetailsViewModel>();

            set.Bind(CloseIconView)
                .For(v => v.BindTap())
                .To(vm => vm.CloseCommand);

            set.Bind(IconImage)
                .For(v => v.ImagePath)
                .To(po => po.AchievementPO.ImagePath);
            
            set.Bind(StatusLabel)
                .To(vm => vm.TextSource[Translations.AchievementDetailsViewModel_Unlocked]);
            
            set.Bind(StatusLabel)
                .For(v => v.BindVisible())
                .To(vm => vm.AchievementPO.IsActive);
            
            set.Bind(NameLabel)
                .To(vm => vm.AchievementPO.Title);
            
            set.Bind(DescriptionLabel)
                .To(vm => vm.AchievementPO.Description);
            
            set.Bind(RewardLabel)
                .To(vm => vm.TextSource[Translations.AchievementDetailsViewModel_Reward]);
            
            set.Bind(SecondRewardLabel)
                .To(vm => vm.TextSource[Translations.AchievementDetailsViewModel_Reward]);
            
            set.Bind(RewardDescriptionLabel)
                .To(vm => vm.AchievementPO.RewardDescription);
            
            set.Bind(SecondRewandDescriptionLabel)
                .To(vm => vm.AchievementPO.RewardDescription);

            set.Bind(RewardDescriptionView)
                .For(v => v.BindVisible())
                .To(vm => vm.AchievementPO.ShouldShowRewardDescription);
            
            set.Bind(BottomRewardView)
                .For(v => v.BindVisible())
                .To(vm => vm.AchievementPO.ShouldShowSecondRewardDescription);
            
            set.Bind(ActivateButton)
                .For(po => po.BindTitle())
                .To(vm => vm.ButtonTitle);
            
            set.Bind(ActivateButton)
                .To(vm => vm.ButtonClickedCommand);
            
            set.Bind(SecondActivateButton)
                .For(po => po.BindTitle())
                .To(vm => vm.ButtonTitle);
            
            set.Bind(SecondActivateButton)
                .To(vm => vm.ButtonClickedCommand);
            
            set.Bind(this)
                .For(s => s.ShouldShowConfetti)
                .To(vm => vm.ShouldShowConfetti);

            set.Bind(ActionButtonTitle)
                .For(v => v.Text)
                .To(po => po.AchievementPO.ActionButtonTitle);
                
            set.Bind(ActionButton)
                .For(v => v.BindVisible())
                .To(po => po.ShouldShowActionButton);

            set.Bind(ActionButton)
                .For(v => v.BindTap())
                .To(vm => vm.ActionButtonClickedCommand);
            
            set.Bind(this)
                .For(v => v.IsCurrentlyPlaying)
                .To(v => v.IsCurrentlyPlaying);
            
            set.Apply();

            NavigationController!.NavigationBarHidden = true;
            NavigationController!.PresentationController!.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = HandleDismiss
            };

            SetThemes();
        }
        
        public bool IsCurrentlyPlaying
        {
            get => _isCurrentlyPlaying;
            set
            {
                _isCurrentlyPlaying = value;

                if (_isCurrentlyPlaying)
                    ShowPlayAnimation();
                else
                    HidePlayAnimation();
            }
        }
        
        public bool ShouldShowConfetti
        {
            get => _shouldShowConfetti;
            set
            {
                _shouldShowConfetti = value;

                if (_shouldShowConfetti && !_confettiShown)
                {
                    _confettiShown = true;
                    ShowConfetti();
                }
            }
        }

        private async Task ShowConfetti()
        {
            var confettiView = new ConfettiView(View!.Bounds);
            confettiView.UserInteractionEnabled = false;
            View.AddSubview(confettiView);
            confettiView.StartConfetti();

            await Task.Delay(5000);

            confettiView.StopConfetti();
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            CloseIconView.Layer.BorderColor = AppColors.SeparatorColor.CGColor;
        }
        
        private void SetThemes()
        {
            CloseIconView.Layer.BorderWidth = 0.5f;
            CloseIconView.Layer.ShadowRadius = 8;
            CloseIconView.Layer.ShadowOffset = CGSize.Empty;
            CloseIconView.Layer.ShadowOpacity = 0.1f;
            StatusLabel.ApplyTextTheme(AppTheme.Subtitle2Label2);
            NameLabel.ApplyTextTheme(AppTheme.Heading1);
            RewardLabel.ApplyTextTheme(AppTheme.Subtitle2Label2);
            SecondRewardLabel.ApplyTextTheme(AppTheme.Subtitle2Label2);
            RewardDescriptionLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
            SecondRewandDescriptionLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
            DescriptionLabel.ApplyTextTheme(AppTheme.Subtitle1Label2);
            ActivateButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
            SecondActivateButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
            ActionButtonTitle.ApplyTextTheme(AppTheme.Subtitle1Label1);
        }

        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }

        private void ShowPlayAnimation()
        {
            IconPlay.Hidden = true;
            _animationView = ThemeUtils.GetLottieAnimationFor(LottieAnimationsNames.PlayAnimationIconDark, LottieAnimationsNames.PlayAnimationIcon);
            _animationView.BackgroundColor = UIColor.Clear;
            _animationView.LoopAnimation = true;
            _animationView.TranslatesAutoresizingMaskIntoConstraints = false;
        
            AnimationView.AddSubview(_animationView);

            NSLayoutConstraint.ActivateConstraints(
                new[]
                {
                    _animationView.LeadingAnchor.ConstraintEqualTo(AnimationView.LeadingAnchor),
                    _animationView.TrailingAnchor.ConstraintEqualTo(AnimationView.TrailingAnchor),
                    _animationView.TopAnchor.ConstraintEqualTo(AnimationView.TopAnchor),
                    _animationView.BottomAnchor.ConstraintEqualTo(AnimationView.BottomAnchor)
                });
            
            _animationView.Play();
        }

        private void HidePlayAnimation()
        {
            IconPlay.Hidden = ViewModel?.AchievementPO?.ActionButtonType != AchievementActionButtonType.PlayNext;
            _animationView.Stop();
            _animationView.RemoveFromSuperview();
        }
    }
}
using BMM.Core.Constants;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.CustomViews;
using BMM.UI.iOS.Extensions;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Presenters;
using MvvmCross.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace BMM.UI.iOS
{
    public partial class AchievementDetailsViewController : BaseViewController<AchievementDetailsViewModel>, IMvxOverridePresentationAttribute
    {
        private const int CloseButtonHeight = 36;
        private bool _shouldShowConfetti;
        private bool _confettiShown;
        private bool _showAsModal;
        private string _imagePath;

        public AchievementDetailsViewController() : base(null)
        {
        }

        public AchievementDetailsViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            var set = this.CreateBindingSet<AchievementDetailsViewController, AchievementDetailsViewModel>();

            set.Bind(CloseIconView)
                .For(v => v.BindTap())
                .To(vm => vm.CloseCommand);

            set.Bind(this)
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

            set.Bind(this)
                .For(v => v.ShowAsModal)
                .To(vm => vm.NavigationParameter.ShowAsModal);
            
            set.Apply();

            var viewModel = (AchievementDetailsViewModel)DataContext;

            if (viewModel.NavigationParameter.ShowAsModal)
            {
                NavigationController!.NavigationBarHidden = true;
                NavigationController!.PresentationController!.Delegate = new CustomUIAdaptivePresentationControllerDelegate
                {
                    OnDidDismiss = HandleDismiss
                };
            }

            SetThemes();
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;

                if (_imagePath.IsNullOrEmpty())
                {
                    IconImage.ImagePath = ImageResourceNames.IconAchievementLocked.ToStandardIosImageName();
                    return;
                }

                IconImage.ImagePath = _imagePath;
            }
        }

        public bool ShowAsModal
        {
            get => _showAsModal;
            set
            {
                _showAsModal = value;
                CloseIconView.Hidden = !_showAsModal;
                CloseIconHeightConstraint.Constant = _showAsModal
                    ? CloseButtonHeight
                    : NumericConstants.Zero;
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
        }

        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }

        public MvxBasePresentationAttribute PresentationAttribute(MvxViewModelRequest request)
        {
            var viewModel = (AchievementDetailsViewModel)((MvxViewModelInstanceRequest)request).ViewModelInstance;

            if (!viewModel!.NavigationParameter.ShowAsModal)
                return new MvxChildPresentationAttribute();
            
            return new MvxModalPresentationAttribute
            {
                WrapInNavigationController = true,
                ModalPresentationStyle = UIModalPresentationStyle.PageSheet
            };
        }
    }
}
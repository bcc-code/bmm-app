using BMM.Core.Constants;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.CustomViews;
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

            set.Bind(IconImage)
                .For(v => v.ImagePath)
                .To(vm => vm.AchievementPO.ImagePath);

            set.Bind(IconImage)
                .For(v => v.Alpha)
                .To(po => po.AchievementPO.IsActive)
                .WithConversion<IsActiveToAlphaConverter>();

            set.Bind(StatusLabel)
                .To(vm => vm.TextSource[nameof(Translations.AchievementDetailsViewModel_Unlocked)]);
            
            set.Bind(StatusLabel)
                .For(v => v.BindVisible())
                .To(vm => vm.AchievementPO.IsActive);
            
            set.Bind(NameLabel)
                .To(vm => vm.AchievementPO.Title);
            
            set.Bind(DescriptionLabel)
                .To(vm => vm.AchievementPO.Description);
            
            set.Bind(ActivateButton)
                .For(po => po.BindTitle())
                .To(vm => vm.ButtonTitle);
            
            set.Bind(ActivateButton)
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
            DescriptionLabel.ApplyTextTheme(AppTheme.Subtitle1Label2);
            ActivateButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
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
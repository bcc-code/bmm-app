using Airbnb.Lottie;
using BMM.Core.Constants;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.NewMediaPlayer;
using CoreAnimation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class HighlightedTextTrackViewController : BaseViewController<HighlightedTextTrackViewModel>
    {
        private float _lastContentOffset;
        private HighlightedTextsTableViewSource _source;
        private bool _isCurrentlyPlaying;
        private float MinimumBottomFloatingConstraintValue => -60;
        
        public HighlightedTextTrackViewController() : base(null)
        {
        }

        public HighlightedTextTrackViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        public bool IsCurrentlyPlaying
        {
            get => _isCurrentlyPlaying;
            set
            {
                _isCurrentlyPlaying = value;
                SetAnimationState();
            }
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.PresentationController.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = HandleDismiss
            };
            NavigationController.NavigationBarHidden = true;

            SetThemes();
            Bind();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            SafeAreaCoveringView.Constant = BottomSafeArea;
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            _source.ScrolledEvent += HighlightedTextsTableViewOnScrolled;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            _source.ScrolledEvent -= HighlightedTextsTableViewOnScrolled;
        }

        private void HighlightedTextsTableViewOnScrolled(object sender, EventArgs e)
        {
            if ((float)HighlightedTextsTableView.ContentOffset.Y < 0)
                return;
            
            float currentContentOffset = (float)HighlightedTextsTableView.ContentOffset.Y;
            float difference = _lastContentOffset - (float)HighlightedTextsTableView.ContentOffset.Y;

            float newValue = Math.Max(MinimumBottomFloatingConstraintValue,
                (float)BottomFloatingLayoutToBottomConstraint.Constant + difference / 2);

            float valueToAssign;
            
            if (newValue < MinimumBottomFloatingConstraintValue)
                valueToAssign = MinimumBottomFloatingConstraintValue;
            else if (newValue > 0)
                valueToAssign = 0;
            else
                valueToAssign = newValue;

            BottomFloatingLayoutToBottomConstraint.Constant = valueToAssign;
            _lastContentOffset = currentContentOffset;
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<HighlightedTextTrackViewController, HighlightedTextTrackViewModel>();
            
            _source = new HighlightedTextsTableViewSource(HighlightedTextsTableView);
            set.Bind(_source).To(vm => vm.Documents);

            set.Bind(AddToButton)
               .For(v => v.BindTitle())
               .To(vm => vm.TextSource[Translations.HighlightedTextTrackViewModel_AddTo]);
            
            set.Bind(PlayButton)
                .For(v => v.BindTitle())
                .To(vm => vm.PlayButtonTitle);
            
            set.Bind(PlayButton)
                .To(vm => vm.PlayPauseCommand);
            
            set.Bind(AddToButton)
                .To(vm => vm.AddToCommand);
            
            set.Bind(this)
                .For(v => v.IsCurrentlyPlaying)
                .To(vm => vm.IsCurrentlyPlaying);
            
           set.Bind(TitleLabel)
               .To(vm => vm.TrackPO.TrackTitle);
           
           set.Bind(SubtitleLabel)
               .To(vm => vm.TrackPO.TrackSubtitle);
           
           set.Bind(MetaLabel)
               .To(vm =>  vm.TrackPO.TrackMeta);
           
           set.Bind(CloseButtonContainer)
               .For(v => v.BindTap())
               .To(vm => vm.CloseCommand);
           
           set.Bind(OptionsButton)
               .For(v => v.BindTap())
               .To(vm => vm.TrackPO.OptionButtonClickedCommand);
           
            set.Apply();
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            CloseButtonContainer.Layer.BorderColor = AppColors.SeparatorColor.CGColor;
            SetLottieAnimation();
        }

        private void SetLottieAnimation()
        {
            LOTAnimationView animation = null;

            if (VersionHelper.SupportsDarkMode && AppDelegate.MainWindow.TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark)
                animation = LOTAnimationView.AnimationNamed(LottieAnimationsNames.PlayAnimationIconDark);
            else
                animation = LOTAnimationView.AnimationNamed(LottieAnimationsNames.PlayAnimationIcon);
            
            animation.BackgroundColor = AppColors.LabelOneColor;
            animation.TintColor = UIColor.Red;
            animation.LoopAnimation = true;
            PlayButton!.AddAnimation(animation);
            SetAnimationState();
        }

        private void SetAnimationState()
        {
            if (_isCurrentlyPlaying)
                PlayButton?.PlayAnimation();
            else
                PlayButton?.StopAnimation();
        }

        private void SetThemes()
        {
            SetLottieAnimation(); 
            CloseButtonContainer.Layer.BorderWidth = 0.5f;
            CloseButtonContainer.Layer.ShadowRadius = 8;
            CloseButtonContainer.Layer.ShadowOffset = CGSize.Empty;
            CloseButtonContainer.Layer.ShadowOpacity = 0.1f;
            PlayButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
            AddToButton.ApplyButtonStyle(AppTheme.ButtonSecondaryMedium);
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
            SubtitleLabel.ApplyTextTheme(AppTheme.Subtitle3Label2);
            MetaLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
            
            BottomFloatingView.Layer.CornerRadius = 16;
            BottomFloatingView.Layer.MaskedCorners = CACornerMask.MinXMinYCorner | CACornerMask.MaxXMinYCorner;
            BottomFloatingView.Layer.ShadowOffset = CGSize.Empty;
            BottomFloatingView.Layer.ShadowRadius = 6;
            BottomFloatingView.Layer.ShadowOpacity = 0.1f;
            BottomFloatingView.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(0,
                -3,
                BottomFloatingView.Frame.Width,
                60)).CGPath;
        }

        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }
    }
}
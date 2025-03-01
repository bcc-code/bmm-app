using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BMM.Core.Constants;
using BMM.Core.Models.Enums;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Utils;
using BMM.UI.iOS.Utils.ColorPalette;
using CoreGraphics;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views.Gestures;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.PageSheet, WrapInNavigationController = true)]
    public partial class PlayerViewController : BaseViewController<PlayerViewModel>
    {
        private const int VideoButtonHeight = 36;
        private const int DefaultBottomMarginConstant = 24;
        private const float BackgroundShadowRadiusPercentageVolume = 0.2f;
        private const float BackgroundAccentAlpha = 0.5f;
        private const float BackgroundAccentColorPercentageVolume = 0.1f;
        private const float CoverSizeToWidthPercentage = 0.45f;
        private const float SliderThumbSize = 10;
        private const float SmallCoverTopMarginConstraint = 40;
        private const float MediumCoverTopMarginConstraint = 60;
        private const float BigCoverTopMarginConstraint = 100;

        private bool _isPlaying;
        private readonly UIImage _playIcon;
        private readonly UIImage _pauseIcon;
        private bool _canNavigateToLanguageChange;
        private bool _hasLeftButton;
        private bool _isShuffleEnabled;
        private RepeatType _repeatType;
        private int _bottomMargin = DefaultBottomMarginConstant;
        private UIStatusBarStyle _previousStatusBarStyle;
        private UIColor _lastMutedColor;
        private bool _isLiked;
        private bool _hasWatchButton;
        private PlayerLeftButtonType? _leftButtonType;

        public PlayerViewController()
            : base(nameof(PlayerViewController))
        {
            _playIcon = UIImage.FromBundle(ImageResourceNames.IconPlay.ToStandardIosImageName())!.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            _pauseIcon = UIImage.FromBundle(ImageResourceNames.IconPause.ToStandardIosImageName())!.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
        }

        private bool SupportsNotFullscreenPageSheetPresentation => UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
        private bool NeedsSetProgressBarThumbColorToBeForcedFromMainThread => !UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
        private bool DarkModeSupported => UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
        public override Type ParentViewControllerType => typeof(UINavigationController);
        protected override string GetTitle() => string.Empty;
        public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation() => UIInterfaceOrientation.Portrait;
        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations() => UIInterfaceOrientationMask.Portrait;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.SetNavigationBarHidden(true, false);

            var set = this.CreateBindingSet<PlayerViewController, PlayerViewModel>();
            set.Bind(PlayingProgressSlider).For(s => s.MaxValue).To(vm => vm.Duration);
            set.Bind(PlayingProgressSlider).For(s => s.Value).To(vm => vm.SliderPosition);
            set.Bind(PlayingProgressSlider).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);
            set.Bind(PlayingProgressSlider).For(b => b.SliderTouchedCommand).To(vm => vm.SeekToPositionCommand);

            set.Bind(BufferedProgressSlider).For(s => s.MaxValue).To(vm => vm.Duration);
            set.Bind(BufferedProgressSlider).For(s => s.Value).To(vm => vm.Downloaded);
            set.Bind(BufferedProgressSlider).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);

            set.Bind(SkipBackButton).To(vm => vm.SkipBackwardCommand);
            set.Bind(SkipBackButton).For(v => v.Enabled).To(vm => vm.IsSeekingDisabled).WithConversion<InvertedBoolConverter>();

            set.Bind(SkipForwardButton).To(vm => vm.SkipForwardCommand);
            set.Bind(SkipForwardButton).For(v => v.Enabled).To(vm => vm.IsSeekingDisabled).WithConversion<InvertedBoolConverter>();

            set.Bind(NextButtton).To(vm => vm.NextCommand);
            set.Bind(NextButtton).For(v => v.Enabled).To(vm => vm.IsSkipToNextEnabled);

            set.Bind(PreviousButton).To(vm => vm.PreviousOrSeekToStartCommand);
            set.Bind(PreviousButton).For(v => v.Enabled).To(vm => vm.IsSkipToPreviousEnabled);

            set.Bind(this).For(v => v.IsShuffleEnabled).To(vm => vm.IsShuffleEnabled);
            set.Bind(ShuffleButton).To(vm => vm.ToggleShuffleCommand);

            set.Bind(RepeatButton).To(vm => vm.ToggleRepeatCommand);
            set.Bind(this)
                .For(v => v.RepeatType)
                .To(vm => vm.RepeatType);

            TrackCoverImageView.ErrorAndLoadingPlaceholderImagePath(ImageResourceNames.NewPlaceholderCover.ToStandardIosImageName());

            set.Bind(TrackCoverImageView)
                .For(v => v.ImagePath)
                .To(vm => vm.CurrentTrack.ArtworkUri)
                .WithConversion<CoverUrlToFallbackImageValueConverter>(ImageResourceNames.NewPlaceholderCover.ToStandardIosImageName());

            set.Bind(SliderPositionTimeLabel).To(vm => vm.SliderPosition).WithConversion<MillisecondsToTimeValueConverter>();
            set.Bind(SliderPositionTimeLabel).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);

            set.Bind(EndTimeLabel).To(vm => vm.Duration).WithConversion<MillisecondsToTimeValueConverter>();
            set.Bind(EndTimeLabel).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);

            set.Bind(ClosePlayerButton)
                .To(vm => vm.CloseViewModelCommand);

            set.Bind(QueueButton)
                .To(vm => vm.OpenQueueCommand);

            set.Bind(LeftButton)
                .For(v => v.BindTitle())
                .To(vm => vm.LeftButtonType)
                .WithConversion<PlayerLeftButtonTypeToTitleConverter>();

            set.Bind(this)
                .For(v => v.LeftButtonType)
                .To(vm => vm.LeftButtonType);
            
            set.Bind(LeftButton)
                .To(vm => vm.LeftButtonClickedCommand);

            set.Bind(this)
                 .For(v => v.HasLeftButton)
                 .To(vm => vm.HasLeftButton);
            
            set.Bind(this)
                .For(v => v.HasWatchButton)
                .To(vm => vm.HasWatchButton);
            
            set.Bind(WatchButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource[Translations.PlayerViewModel_WatchOnBCCMedia]);
            
            set.Bind(WatchButton)
                .To(vm => vm.WatchButtonClickedCommand);

            set.Bind(MoreButton).To(vm => vm.OptionCommand);
            set.Bind(ChangeLanguageButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TrackLanguage);

            set.Bind(ChangeLanguageButton)
                .To(vm => vm.NavigateToLanguageChangeCommand);

            set.Bind(this)
                .For(v => v.CanNavigateToLanguageChange)
                .To(vm => vm.CanNavigateToLanguageChange);

            set.Bind(PlayPauseButton).To(vm => vm.PlayPauseCommand);
            set.Bind(this).For(v => v.IsPlaying).To(vm => vm.IsPlaying);

            set.Bind(TitleLabel).To(vm => vm.CurrentTrack).WithConversion<TrackToTitleValueConverter>(ViewModel);
           
            set.Bind(SubtitleLabel).To(vm => vm.CurrentTrack).WithConversion<TrackToSubtitleValueConverter>(ViewModel);
            SubtitleLabel.FadeLength = 10;
            SubtitleLabel.ScrollDuration = 6;

            set.Bind(ExternalRelationButton)
                .For(v => v.BindVisible())
                .To(vm => vm.HasExternalRelations);

            set.Bind(ExternalRelationButton)
                .To(vm => vm.ShowTrackInfoCommand);

            set.Bind(this)
                .For(v => v.IsLiked)
                .To(vm => vm.IsLiked);

            set.Bind(LikeButton)
                .To(vm => vm.LikeUnlikeCommand);
            
            set.Bind(TrackCoverImageView.Swipe(UISwipeGestureRecognizerDirection.Right)).For(v => v.Command).To(vm => vm.PreviousCommand);
            set.Bind(TrackCoverImageView.Swipe(UISwipeGestureRecognizerDirection.Left)).For(v => v.Command).To(vm => vm.NextCommand);
            set.Apply();

            PlayingProgressSlider.TouchDown += (sender, e) => { ViewModel.IsSeeking = true; };
            PlayingProgressSlider.TouchUpInside += (sender, e) => { ViewModel.IsSeeking = false; };
            PlayingProgressSlider.TouchUpOutside += (sender, e) => { ViewModel.IsSeeking = false; };

            var recognizer = new UISwipeGestureRecognizer(() => {ViewModel.CloseViewModelCommand.Execute();}) {Direction = UISwipeGestureRecognizerDirection.Down};
            View.AddGestureRecognizer(recognizer);

            SetThemes();

            NavigationController.PresentationController.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = HandleDismiss,
                OnDidAttemptToDismiss = HandleDismiss
            };

            SetViewMargins();
        }

        public PlayerLeftButtonType? LeftButtonType
        {
            get => _leftButtonType;
            set
            {
                _leftButtonType = value;
                var image = _leftButtonType == PlayerLeftButtonType.Transcription
                    ? UIImage.FromBundle(ImageResourceNames.IconInfo.ToStandardIosImageName())
                    : null;
                
                LeftButton.SetImage(image, UIControlState.Normal);
            }
        }

        public bool IsLiked
        {
            get => _isLiked;
            set
            {
                _isLiked = value;
                LikeButton.SetImage(_isLiked
                        ? UIImage.FromBundle(ImageResourceNames.IconLiked.ToStandardIosImageName())
                        : UIImage.FromBundle(ImageResourceNames.IconUnliked.ToStandardIosImageName()),
                    UIControlState.Normal);
            }
        }

        public override void TraitCollectionDidChange(UITraitCollection? previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            
            if (!DarkModeSupported)
                return;
            
            var backgroundColor = AppColors.BackgroundTwoColor.GetResolvedColor(AppDelegate.MainWindow.TraitCollection);

            var colorToSet = _lastMutedColor == null
                ? backgroundColor
                : BlendBackgroundColor(_lastMutedColor, backgroundColor);

            if (View != null)
                View.BackgroundColor = colorToSet;
            
            SetupProgressBarThumb();
            SetThemes();
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            TrackCoverImageView.ImageChanged += TrackCoverImageViewOnImageChanged;
        }
        
        protected override void DetachEvents()
        {
            base.DetachEvents();
            TrackCoverImageView.ImageChanged -= TrackCoverImageViewOnImageChanged;
        }
        
        private async void TrackCoverImageViewOnImageChanged(object? sender, EventArgs e)
        {
            string coverPlaceholderPath = ImageResourceNames.NewPlaceholderCover.ToStandardIosImageName();
            await Task.Delay(ViewConstants.QuickAnimationDurationInMilliseconds / 2);
            
            bool shouldTintBackground =
                !TrackCoverImageView.IsLoading
                && !TrackCoverImageView.IsError
                && TrackCoverImageView.Image?.AccessibilityIdentifier != coverPlaceholderPath
                && TrackCoverImageView.ImagePath != coverPlaceholderPath;
            
            SetCoverShadowLayer(shouldTintBackground);
        }

        private void SetViewMargins()
        {
            SetCoverTopMargin();

            if (UIScreen.MainScreen.Bounds.Height > iOSScreenHeight.iPhoneSE2)
                return;

            _bottomMargin = DefaultBottomMarginConstant / 2;
            SeparatorTopMarginConstraint.Constant = _bottomMargin;
            SeparatorBottomMarginConstraint.Constant = _bottomMargin;
            BottomMarginConstraint.Constant = _bottomMargin;
            SliderBottomMarginConstraint.Constant = _bottomMargin;
        }

        private void SetCoverTopMargin()
        {
            if (UIScreen.MainScreen.Bounds.Height <= iOSScreenHeight.iPhoneSE)
                CoverTopMarginConstraint.Constant = SmallCoverTopMarginConstraint;
            else if (UIScreen.MainScreen.Bounds.Height <= iOSScreenHeight.iPhoneSE2)
                CoverTopMarginConstraint.Constant = MediumCoverTopMarginConstraint;
            else
                CoverTopMarginConstraint.Constant = BigCoverTopMarginConstraint;
        }

        public RepeatType RepeatType
        {
            get => _repeatType;
            set
            {
                _repeatType = value;
                UpdateRepeatImage(_repeatType);
            }
        }

        public bool IsShuffleEnabled
        {
            get => _isShuffleEnabled;
            set
            {
                _isShuffleEnabled = value;

                if (_isShuffleEnabled)
                {
                    ShuffleButton.BackgroundColor = AppColors.LabelOneColor;
                    ShuffleButton.TintColor = AppColors.LabelOneColorReverted;
                }
                else
                {
                    ShuffleButton.TintColor = AppColors.LabelOneColor;
                    ShuffleButton.BackgroundColor = UIColor.Clear;
                }
            }
        }

        public bool CanNavigateToLanguageChange
        {
            get => _canNavigateToLanguageChange;
            set
            {
                _canNavigateToLanguageChange = value;
                ViewUtils.RunAnimation(ViewConstants.DefaultAnimationDuration, () =>
                {
                    ChangeLanguageButton.SetHiddenIfNeeded(!_canNavigateToLanguageChange);
                    SetBottomMarginConstant();
                    BottomButtonsStackLayout.LayoutIfNeeded();
                });
            }
        }
        
        public bool HasWatchButton
        {
            get => _hasWatchButton;
            set
            {
                _hasWatchButton = value;
                BccButtonHeightConstraint.Constant = _hasWatchButton
                    ? VideoButtonHeight
                    : NumericConstants.Zero;
            }
        }
        
        public bool HasLeftButton
        {
            get => _hasLeftButton;
            set
            {
                _hasLeftButton = value;
                ViewUtils.RunAnimation(ViewConstants.DefaultAnimationDuration, () =>
                {
                    LeftButton.SetHiddenIfNeeded(!_hasLeftButton);
                    SetBottomMarginConstant();
                    BottomButtonsStackLayout.LayoutIfNeeded();
                });
            }
        }

        private void SetBottomMarginConstant()
        {
            if (!_canNavigateToLanguageChange && !_hasLeftButton)
                BottomMarginConstraint.Constant = 0;
            else
                BottomMarginConstraint.Constant = _bottomMargin;
        }

        private void SetCoverShadowLayer(bool shouldTintBackground)
        {
            var backgroundColor = UIDevice.CurrentDevice.CheckSystemVersion(13, 0)
                ? AppColors.BackgroundTwoColor.GetResolvedColor(AppDelegate.MainWindow.TraitCollection)
                : AppColors.BackgroundTwoColor;

            if (!shouldTintBackground)
            {
                ViewUtils.RunAnimation(ViewConstants.LongAnimationDuration,
                    () =>
                    {
                        View!.BackgroundColor = backgroundColor;
                        ShadowView.Layer.ShadowColor = UIColor.Clear.CGColor;
                    });
                return;
            }

            var coverImage = TrackCoverImageView.Image;
            var palette = new ColorPaletteGenerator();

            Task.Run(() => { palette.Generate(coverImage); })
                .ContinueWith(t =>
                {
                    InvokeOnMainThread(() => ViewUtils.RunAnimation(ViewConstants.LongAnimationDuration, () => SetCoverShadow(palette, backgroundColor)));
                });
        }

        private void SetCoverShadow(ColorPaletteGenerator palette, UIColor backgroundColor)
        {
            _lastMutedColor = palette.MutedColor;
            ShadowView.Layer.ShadowColor = new CGColor(palette.MutedColor.CGColor, BackgroundAccentAlpha);
            ShadowView.Layer.ShadowOpacity = 1f;
            ShadowView.Layer.ShadowOffset = CGSize.Empty;
            ShadowView.Layer.ShadowRadius = View!.Frame.Width * BackgroundShadowRadiusPercentageVolume;
            ShadowView.Layer.ShadowPath = UIBezierPath.FromRect(TrackCoverImageView.Bounds).CGPath;
            View.BackgroundColor = BlendBackgroundColor(palette.MutedColor, backgroundColor);
        }

        private static UIColor BlendBackgroundColor(UIColor accentColor, UIColor backgroundColor)
        {
            return backgroundColor.Blend(
                accentColor,
                1f - BackgroundAccentColorPercentageVolume,
                BackgroundAccentColorPercentageVolume);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;

                var icon = _isPlaying
                    ? _pauseIcon
                    : _playIcon;

                PlayPauseButton.SetImage(icon, UIControlState.Normal);
            }
        }

        private void SetThemes()
        {
            LeftButton.ApplyButtonStyle(AppTheme.ButtonTertiaryMediumAutoSize);
            WatchButton.ApplyButtonStyle(AppTheme.IconButtonTertiarySmall);
            ChangeLanguageButton.ApplyButtonStyle(AppTheme.ButtonTertiaryMedium);
            TitleLabel.ApplyTextTheme(AppTheme.Heading3AutoSize);
            SubtitleLabel.ApplyTextTheme(AppTheme.Subtitle1Label2);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            var coverSize = View.Frame.Width * CoverSizeToWidthPercentage;

            CoverHeightConstraint.Constant = coverSize;
            CoverWidthConstraint.Constant = coverSize;
            SetupProgressBarThumb();
            RunIfDeviceSupportsNotFullscreenPageSheetPresentation(() =>
            {
                _previousStatusBarStyle = UIApplication.SharedApplication.StatusBarStyle;
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
            });
        }

        private void SetupProgressBarThumb()
        {
            if (NeedsSetProgressBarThumbColorToBeForcedFromMainThread)
                BeginInvokeOnMainThread(SetProgressBarThumb);
            else
                SetProgressBarThumb();
        }

        private void SetProgressBarThumb()
        {
            var circleImage = ImageUtils.MakePlayerThumbCircle(new CGSize(SliderThumbSize, SliderThumbSize), AppColors.LabelOneColor);
            PlayingProgressSlider.SetThumbImage(circleImage, UIControlState.Normal);
            PlayingProgressSlider.SetThumbImage(circleImage, UIControlState.Highlighted);
            BufferedProgressSlider.SetThumbImage(new UIImage(), UIControlState.Normal);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            RunIfDeviceSupportsNotFullscreenPageSheetPresentation(() => UIApplication.SharedApplication.SetStatusBarStyle(_previousStatusBarStyle, true));
        }

        protected override void SetNavigationBarAppearance() => Expression.Empty();

        private void RunIfDeviceSupportsNotFullscreenPageSheetPresentation(Action action)
        {
            if (!SupportsNotFullscreenPageSheetPresentation)
                return;

            action?.Invoke();
        }
        
        private void UpdateRepeatImage(RepeatType repeatType)
        {
            bool isSelected = repeatType != RepeatType.None;

            string iconName = repeatType == RepeatType.RepeatOne
                ? ImageResourceNames.IconRepeatOne
                : ImageResourceNames.IconRepeat;

            RepeatButton.SetImage(UIImage.FromBundle(iconName.ToStandardIosImageName()), UIControlState.Normal);

            if (isSelected)
            {
                RepeatButton.BackgroundColor = AppColors.LabelOneColor;
                RepeatButton.TintColor = AppColors.LabelOneColorReverted;
            }
            else
            {
                RepeatButton.BackgroundColor = UIColor.Clear;
                RepeatButton.TintColor = AppColors.LabelOneColor;
            }
        }

        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }
    }
}
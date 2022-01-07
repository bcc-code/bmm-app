using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BMM.Core.Constants;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;
using BMM.UI.iOS.Utils.ColorPalette;
using CoreGraphics;
using FFImageLoading.Args;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.PageSheet, WrapInNavigationController = true)]
    public partial class PlayerViewController : BaseViewController<PlayerViewModel>
    {
        private UIViewVisibilityController _trackReferenceButtonVisibilityController;
        private bool _isPlaying;
        private readonly UIImage _playIcon;
        private readonly UIImage _pauseIcon;
        private string _lastCoverUri;
        private bool _canNavigateToLanguageChange;
        private bool _hasLyrics;
        private int _defaultBottomMarginConstant = 24;

        public PlayerViewController()
            : base(nameof(PlayerViewController))
        {
            _playIcon = UIImage.FromBundle("PlayIcon")!.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            _pauseIcon = UIImage.FromBundle("PauseIcon")!.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
        }

        public override Type ParentViewControllerType => typeof(UINavigationController);

        protected override string GetTitle() => string.Empty;

        private UIImage MakeCircleWith(CGSize size, UIColor backgroundColor)
        {
            UIGraphics.BeginImageContextWithOptions(size, false, 0.0f);
            var context = UIGraphics.GetCurrentContext();
            context?.SetFillColor(backgroundColor.CGColor);
            context?.SetStrokeColor(UIColor.Clear.CGColor);
            var bounds = new CGRect(CGPoint.Empty, size);
            context?.AddEllipseInRect(bounds);
            context?.DrawPath(CGPathDrawingMode.Fill);
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return image;
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.SetNavigationBarHidden(true, false);

            var set = this.CreateBindingSet<PlayerViewController, PlayerViewModel>();
            set.Bind(PlayingProgressSlider).For(s => s.MaxValue).To(vm => vm.Duration);
            set.Bind(PlayingProgressSlider).For(s => s.Value).To(vm => vm.SliderPosition);
            set.Bind(PlayingProgressSlider).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);
            
            set.Bind(BufferedProgressSlider).For(s => s.MaxValue).To(vm => vm.Duration);
            set.Bind(BufferedProgressSlider).For(s => s.Value).To(vm => vm.Downloaded);
            set.Bind(BufferedProgressSlider).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);
            
            set.Bind(SkipBackButton).To(vm => vm.SkipBackwardCommand);
            set.Bind(SkipBackButton).For(v => v.Enabled).To(vm => vm.IsSeekingDisabled).WithConversion<InvertedVisibilityConverter>();
            
            set.Bind(SkipForwardButton).To(vm => vm.SkipForwardCommand);
            set.Bind(SkipForwardButton).For(v => v.Enabled).To(vm => vm.IsSeekingDisabled).WithConversion<InvertedVisibilityConverter>();
            
            set.Bind(NextButtton).To(vm => vm.NextCommand);
            set.Bind(NextButtton).For(v => v.Enabled).To(vm => vm.IsSkipToNextEnabled);
            
            set.Bind(PreviousButton).To(vm => vm.PreviousOrSeekToStartCommand);
            set.Bind(PreviousButton).For(v => v.Enabled).To(vm => vm.IsSkipToPreviousEnabled);
            
            // set.Bind(QueueRandomButton).For(v => v.Selected).To(vm => vm.IsShuffleEnabled);
            // set.Bind(QueueRandomButton).To(vm => vm.ToggleShuffleCommand);
            // set.Bind(QueueRandomButton).For(v => v.Enabled).To(vm => vm.IsSeekingDisabled).WithConversion<InvertedVisibilityConverter>();
            //
            // set.Bind(QueueRepeatButton).To(vm => vm.ToggleRepeatCommand);
            // set.Bind(QueueRepeatButton).For(v => v.Enabled).To(vm => vm.IsSeekingDisabled).WithConversion<InvertedVisibilityConverter>();
            
            TrackCoverImageView.ErrorAndLoadingPlaceholderImagePath("NewPlaceholderCover");
            
            set.Bind(TrackCoverImageView)
                .For(v => v.ImagePath)
                .To(vm => vm.CurrentTrack.ArtworkUri)
                .WithConversion<CoverUrlToFallbackImageValueConverter>("NewPlaceholderCover");
            TrackCoverImageView.OnFinish += TrackCoverImageViewOnOnFinish;
                
            set.Bind(SliderPositionTimeLabel).To(vm => vm.SliderPosition).WithConversion<MillisecondsToTimeValueConverter>();
            set.Bind(SliderPositionTimeLabel).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);
            
            set.Bind(EndTimeLabel).To(vm => vm.Duration).WithConversion<MillisecondsToTimeValueConverter>();
            set.Bind(EndTimeLabel).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);

            set.Bind(ClosePlayerButton)
                .To(vm => vm.CloseViewModelCommand);
            
            set.Bind(QueueButton)
                .To(vm => vm.OpenQueueCommand);
            
            set.Bind(ViewLyricsButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource[Translations.PlayerViewModel_ViewLyrics]);
            
            set.Bind(ViewLyricsButton)
                .To(vm => vm.OpenLyricsCommand);
            
            set.Bind(this)
                 .For(v => v.HasLyrics)
                 .To(vm => vm.HasLyrics);
            
            // set.Bind(TrackOptionsButton).To(vm => vm.OptionCommand);
            // set.Bind(TrackReferernceButton).To(vm => vm.ShowTrackInfoCommand);
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
            
            // set.Bind(BtvLinkButton).For(v => v.TitleLabel).To(vm => vm.BtvLinkTitle);
            // set.Bind(BtvLinkButton).To(vm => vm.OpenExternalReferenceCommand);
            // set.Bind(BtvLinkContainer).For(v => v.Hidden).To(vm => vm.HasBtvLink).WithConversion<InvertedVisibilityConverter>();
            //
            // set.Bind(TrackCoverContainerView.Swipe(UISwipeGestureRecognizerDirection.Right)).For(v => v.Command).To(vm => vm.PreviousCommand);
            // set.Bind(TrackCoverContainerView.Swipe(UISwipeGestureRecognizerDirection.Left)).For(v => v.Command).To(vm => vm.NextCommand);
            set.Apply();
            
            PlayingProgressSlider.TouchDown += (sender, e) => { ViewModel.IsSeeking = true; };
            PlayingProgressSlider.TouchUpInside += (sender, e) => { ViewModel.IsSeeking = false; };
            PlayingProgressSlider.TouchUpOutside += (sender, e) => { ViewModel.IsSeeking = false; };

            ViewModel.PropertyChanged += (sender, e) =>
            {
                var currentTrackChanged = e.PropertyName == nameof(ViewModel.CurrentTrack) || e.PropertyName == "";
                if (currentTrackChanged && ViewModel.CurrentTrack != null)
                {
                    UpdateTrackReferenceButtonVisibility();
                }

                if (e.PropertyName == nameof(ViewModel.RepeatType))
                {
                    UpdateRepeatImage();
                }
            };

            MoveTitleLabelsIntoNavigationBar();
            UpdateTrackReferenceButtonVisibility();
            UpdateRepeatImage();

            // Add swipe-down gesture to all modal ViewControllers, since you expect, that you can move them downwards, the direction they came from.
            var recognizer = new UISwipeGestureRecognizer(() => {ViewModel.CloseViewModelCommand.Execute();}) {Direction = UISwipeGestureRecognizerDirection.Down};
            View.AddGestureRecognizer(recognizer);

            SetThemes();
            
            NavigationController.PresentationController.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = HandleDismiss,
                OnDidAttemptToDismiss = HandleDismiss
            };

            if (UIScreen.MainScreen.Bounds.Height > 667)
                return;
            
            _defaultBottomMarginConstant = 12;
            SeparatorTopMarginConstraint.Constant = 12;
            SeparatorBottomMarginConstraint.Constant = 12;
            BottomMarginConstraint.Constant = _defaultBottomMarginConstant;
            SliderBottomMarginConstraint.Constant = 12;
            CoverTopMarginConstraint.Constant = 40;
        }

        public bool CanNavigateToLanguageChange
        {
            get => _canNavigateToLanguageChange;
            set
            {
                _canNavigateToLanguageChange = value;
                
                UIView.Animate(
                    ViewConstants.DefaultAnimationDuration,
                    0,
                    UIViewAnimationOptions.AllowUserInteraction,
                    () =>
                    {
                        ChangeLanguageButton.SetHiddenIfNeeded(!_canNavigateToLanguageChange);
                        SetBottomMarginConstant();
                        BottomButtonsStackLayout.LayoutIfNeeded();
                    }, null);
            }
        }

        public bool HasLyrics
        {
            get => _hasLyrics;
            set
            {
                _hasLyrics = value;
                
                UIView.Animate(
                    ViewConstants.DefaultAnimationDuration,
                    0,
                    UIViewAnimationOptions.AllowUserInteraction,
                    () =>
                    {
                        ViewLyricsButton.SetHiddenIfNeeded(!_hasLyrics);
                        SetBottomMarginConstant();
                        BottomButtonsStackLayout.LayoutIfNeeded();
                    }, null);
            }
        }

        private void SetBottomMarginConstant()
        {
            if (!_canNavigateToLanguageChange && !_hasLyrics)
                BottomMarginConstraint.Constant = 0;
            else
                BottomMarginConstraint.Constant = _defaultBottomMarginConstant;
        }
        
        private void TrackCoverImageViewOnOnFinish(object sender, FinishEventArgs e)
        {
            InvokeOnMainThread(() => SetCoverShadowLayer(!string.IsNullOrEmpty(ViewModel.CurrentTrack?.ArtworkUri)));
        }

        private void SetCoverShadowLayer(bool shouldTintBackground)
        {
            var backgroundColor = UIDevice.CurrentDevice.CheckSystemVersion(13, 0)
                ? AppColors.BackgroundSecondaryColor.GetResolvedColor(AppDelegate.MainWindow.TraitCollection)
                : AppColors.BackgroundSecondaryColor;

            if (!shouldTintBackground)
            {
                UIView.Animate(
                    ViewConstants.LongAnimationDuration,
                    0,
                    UIViewAnimationOptions.AllowUserInteraction,
                    () =>
                    {
                        View!.BackgroundColor = backgroundColor;
                        ShadowView.Layer.ShadowColor = UIColor.Clear.CGColor;
                    }, null);
                return; 
            }

            var coverImage = TrackCoverImageView.Image;
            var palette = new ColorPaletteGenerator();

            Task.Run(() => { palette.Generate(coverImage); })
                .ContinueWith(t =>
                {
                    InvokeOnMainThread(() =>
                    {
                        UIView.Animate(
                            ViewConstants.LongAnimationDuration,
                            0,
                            UIViewAnimationOptions.AllowUserInteraction,
                            () =>
                            {
                                ShadowView.Layer.ShadowColor = new CGColor(palette.MutedColor.CGColor, 0.5f);
                                ShadowView.Layer.ShadowOpacity = 1f;
                                ShadowView.Layer.ShadowOffset = CGSize.Empty;
                                ShadowView.Layer.ShadowRadius = View.Frame.Width * 0.2f;
                                ShadowView.Layer.ShadowPath = UIBezierPath.FromRect(TrackCoverImageView.Bounds).CGPath;

                                View.BackgroundColor = Blend(backgroundColor,
                                    palette.MutedColor,
                                    0.9f,
                                    0.1f);
                            },
                            null);
                    });
                });
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
            ViewLyricsButton.ApplyButtonStyle(AppTheme.ButtonTertiaryMedium);
            ChangeLanguageButton.ApplyButtonStyle(AppTheme.ButtonTertiaryMedium);
            TitleLabel.ApplyTextTheme(AppTheme.Heading3);
            SubtitleLabel.ApplyTextTheme(AppTheme.Subtitle1Label2);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            var coverSize = View.Frame.Width * 0.45f;
            
            CoverHeightConstraint.Constant = coverSize;
            CoverWidthConstraint.Constant = coverSize;
            
            var circleImage = MakeCircleWith(new CGSize(10, 10), AppColors.LabelPrimaryColor);
            
            PlayingProgressSlider.SetThumbImage(circleImage, UIControlState.Normal);
            PlayingProgressSlider.SetThumbImage(circleImage, UIControlState.Highlighted);
            BufferedProgressSlider.SetThumbImage(new UIImage(), UIControlState.Normal);
        }

        protected override void SetNavigationBarAppearance() => Expression.Empty();

        /// <summary>
        /// This method is needed because it's not possible to add the views
        /// directly into the navigation bar XIB while it's possible using storyboards
        /// </summary>
        private void MoveTitleLabelsIntoNavigationBar()
        {
            // TitleLabel.RemoveFromSuperview();
            // subtitleLabel.RemoveFromSuperview();
            var containerStack = new UIStackView
            {
                Axis = UILayoutConstraintAxis.Vertical,
                Spacing = 2
            };
            // containerStack.AddArrangedSubview(TitleLabel);
            // containerStack.AddArrangedSubview(subtitleLabel);
            containerStack.TranslatesAutoresizingMaskIntoConstraints = false;
            NavigationItem.TitleView = containerStack;
        }

        private void UpdateRepeatImage()
        {
            var iconPrefix = "icon_repeat_";
            var extension = ".png";

            string iconState;

            switch (ViewModel.RepeatType)
            {
                case RepeatType.RepeatAll:
                    iconState = "active";
                    break;

                case RepeatType.RepeatOne:
                    iconState = "one_active";
                    break;

                case RepeatType.None:
                    iconState = "static";
                    break;

                default:
                    iconState = "static";
                    break;
            }

            var imageUrl = iconPrefix + iconState + extension;

            var image = new UIImage(imageUrl);
          //  QueueRepeatButton.SetImage(image, UIControlState.Normal);
        }

        private void UpdateTrackReferenceButtonVisibility()
        {
            if (_trackReferenceButtonVisibilityController == null)
            {
               // _trackReferenceButtonVisibilityController = new UIViewVisibilityController(TrackReferernceButton);
            }

            var valueConverter = new TrackHasExternalRelationsValueConverter();
            var referenceButtonVisible = valueConverter.Convert(ViewModel.CurrentTrack, null, null, null);
            if (referenceButtonVisible is bool visible)
            {
//                _trackReferenceButtonVisibilityController.ViewIsVisible = visible;
            }
            else
            {
    //            _trackReferenceButtonVisibilityController.ViewIsVisible = false;
            }
        }

        public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
        {
            return UIInterfaceOrientation.Portrait;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.Portrait;
        }
        
        private UIColor Blend(UIColor color1, UIColor color2, float intensity1 = 0.5f, float intensity2 = 0.5f)
        {
            float total = intensity1 + intensity2;
            
            float l1 = intensity1 / total;
            float l2 = intensity2 / total;

            (nfloat R, nfloat G, nfloat B, nfloat A) colorsOne = (R: 0f, G: 0f, B: 0f, A: 0f);
            (nfloat R, nfloat G, nfloat B, nfloat A) colorsTwo = (R: 0f, G: 0f, B: 0f, A: 0f);

            color1.GetRGBA(out colorsOne.R, out colorsOne.G, out colorsOne.B, out colorsOne.A);
            color2.GetRGBA(out colorsTwo.R, out colorsTwo.G, out colorsTwo.B, out colorsTwo.A);

            return new UIColor(
                l1 * colorsOne.R + l2 * colorsTwo.R,
                l1 * colorsOne.G + l2 * colorsTwo.G,
                l1 * colorsOne.B + l2 * colorsTwo.B,
                l1 * colorsOne.A + l2 * colorsTwo.A);
        }
        
        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }
    }
}
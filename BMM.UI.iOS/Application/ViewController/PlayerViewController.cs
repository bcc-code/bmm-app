using System;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;
using BMM.UI.iOS.NewMediaPlayer;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding.Views.Gestures;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.FullScreen, WrapInNavigationController = true)]
    public partial class PlayerViewController : BaseViewController<PlayerViewModel>
    {
        private UIViewVisibilityController _trackReferenceButtonVisibilityController;

        public PlayerViewController()
            : base(nameof(PlayerViewController))
        { }

        public override Type ParentViewControllerType => typeof(UINavigationController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AddNavigationBarItemForQueue();

            // Update some stylings, we weren't able to set in the XIB resource file.
            PlayingProgressSlider.SetMaxTrackImage(UIImage.FromFile("progress_slider_background_transparent.png"), UIControlState.Normal);
            BufferedProgressSlider.SetMaxTrackImage(UIImage.FromFile("progress_slider_background_white_dark.png"), UIControlState.Normal);

            PlayingProgressSlider.SetThumbImage(UIImage.FromFile("slider_thumb_white.png"), UIControlState.Normal);
            BufferedProgressSlider.SetThumbImage(new UIImage(), UIControlState.Normal);

            var set = this.CreateBindingSet<PlayerViewController, PlayerViewModel>();
            set.Bind(PlayingProgressSlider).For(s => s.MaxValue).To(vm => vm.Duration);
            set.Bind(PlayingProgressSlider).For(s => s.Value).To(vm => vm.SliderPosition);
            set.Bind(PlayingProgressSlider).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);

            set.Bind(BufferedProgressSlider).For(s => s.MaxValue).To(vm => vm.Duration);
            set.Bind(BufferedProgressSlider).For(s => s.Value).To(vm => vm.Downloaded);
            set.Bind(BufferedProgressSlider).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);

            set.Bind(SkipBackwardButton).To(vm => vm.SkipBackwardCommand);
            set.Bind(SkipBackwardButton).For(v => v.Enabled).To(vm => vm.IsSeekingDisabled).WithConversion<InvertedVisibilityConverter>();

            set.Bind(SkipForwardButton).To(vm => vm.SkipForwardCommand);
            set.Bind(SkipForwardButton).For(v => v.Enabled).To(vm => vm.IsSeekingDisabled).WithConversion<InvertedVisibilityConverter>();

            set.Bind(QueueNextButton).To(vm => vm.NextCommand);
            set.Bind(QueueNextButton).For(v => v.Enabled).To(vm => vm.IsSkipToNextEnabled);

            set.Bind(QueuePreviousButton).To(vm => vm.PreviousOrSeekToStartCommand);
            set.Bind(QueuePreviousButton).For(v => v.Enabled).To(vm => vm.IsSkipToPreviousEnabled);

            set.Bind(QueueRandomButton).For(v => v.Selected).To(vm => vm.IsShuffleEnabled);
            set.Bind(QueueRandomButton).To(vm => vm.ToggleShuffleCommand);
            set.Bind(QueueRandomButton).For(v => v.Enabled).To(vm => vm.IsSeekingDisabled).WithConversion<InvertedVisibilityConverter>();

            set.Bind(QueueRepeatButton).To(vm => vm.ToggleRepeatCommand);
            set.Bind(QueueRepeatButton).For(v => v.Enabled).To(vm => vm.IsSeekingDisabled).WithConversion<InvertedVisibilityConverter>();

            set.Bind(TimePlayedLabel).To(vm => vm.SliderPosition).WithConversion<MillisecondsToTimeValueConverter>();
            set.Bind(TimePlayedLabel).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);

            set.Bind(TimeTotalLabel).To(vm => vm.Duration).WithConversion<MillisecondsToTimeValueConverter>();
            set.Bind(TimeTotalLabel).For(b => b.Hidden).To(vm => vm.IsSeekingDisabled);

            TrackCoverImageView.ErrorAndLoadingPlaceholderImagePathForCover();
            set.Bind(TrackCoverImageView)
                .For(v => v.ImagePath)
                .To(vm => vm.CurrentTrack.ArtworkUri)
                .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.CoverPlaceholderImage);
            set.Bind(TrackOptionsButton).To(vm => vm.OptionCommand);
            set.Bind(TrackReferernceButton).To(vm => vm.ShowTrackInfoCommand);

            set.Bind(TrackPlayPauseButton).To(vm => vm.PlayPauseCommand);
            set.Bind(TrackPlayPauseButton).For(v => v.Selected).To(vm => vm.IsPlaying);

            set.Bind(TrackTitleLabel).To(vm => vm.CurrentTrack).WithConversion<TrackToTitleValueConverter>(ViewModel);
            set.Bind(TrackSubtitleLabel).To(vm => vm.CurrentTrack).WithConversion<TrackToSubtitleUppercaseConverter>(ViewModel);
            set.Bind(TitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.PlayerViewModel_Title);
            set.Bind(subtitleLabel).To(vm => vm.PlayingText);

            set.Bind(BtvLinkButton).For(v => v.TitleLabel).To(vm => vm.BtvLinkTitle);
            set.Bind(BtvLinkButton).To(vm => vm.OpenExternalReferenceCommand);
            set.Bind(BtvLinkContainer).For(v => v.Hidden).To(vm => vm.HasBtvLink).WithConversion<InvertedVisibilityConverter>();

            set.Bind(TrackCoverContainerView.Swipe(UISwipeGestureRecognizerDirection.Right)).For(v => v.Command).To(vm => vm.PreviousCommand);
            set.Bind(TrackCoverContainerView.Swipe(UISwipeGestureRecognizerDirection.Left)).For(v => v.Command).To(vm => vm.NextCommand);
            set.Apply();

            PlayingProgressSlider.TouchDown += (object sender, EventArgs e) => { ViewModel.IsSeeking = true; };

            PlayingProgressSlider.TouchUpInside += (object sender, EventArgs e) => { ViewModel.IsSeeking = false; };

            PlayingProgressSlider.TouchUpOutside += (object sender, EventArgs e) => { ViewModel.IsSeeking = false; };

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

            UpdateCoverViewTopConstraintIfNeeded();
            MoveTitleLabelsIntoNavigationBar();
            UpdateTrackReferenceButtonVisibility();
            UpdateRepeatImage();
            AddGradientToTrackCoverImage();

            NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem(
                    UIImage.FromFile("icon_down_static.png"),
                    UIBarButtonItemStyle.Plain,
                    (sender, eventArgs) => { ViewModel.CloseViewModelCommand.Execute();}
                ),
                false);

            // Add swipe-down gesture to all modal ViewControllers, since you expect, that you can move them downwards, the direction they came from.
            var recognizer = new UISwipeGestureRecognizer(() => {ViewModel.CloseViewModelCommand.Execute();}) {Direction = UISwipeGestureRecognizerDirection.Down};
            View.AddGestureRecognizer(recognizer);
        }

        /// <summary>
        /// We need to adjust this constraint manually because "safe area" is not supported on iOS lower than version 11.
        /// Normally Interface builder would create top and bottom layout guides to make it backwards compatible.
        /// However XIBs do not support that and therefore we have to adjust it manually here.
        /// <see href="https://forums.developer.apple.com/thread/87329#267357"/> for more information
        /// This method can be removed when dropping support for iOS lower than 11.
        /// </summary>
        private void UpdateCoverViewTopConstraintIfNeeded()
        {
            if (!VersionHelper.SupportsSafeAreaLayoutGuide)
                CoverViewTopConstraint.Constant = 64;
        }

        /// <summary>
        /// This method is needed because it's not possible to add the views
        /// directly into the navigation bar XIB while it's possible using storyboards
        /// </summary>
        private void MoveTitleLabelsIntoNavigationBar()
        {
            TitleLabel.RemoveFromSuperview();
            subtitleLabel.RemoveFromSuperview();
            var containerStack = new UIStackView
            {
                Axis = UILayoutConstraintAxis.Vertical,
                Spacing = 2
            };
            containerStack.AddArrangedSubview(TitleLabel);
            containerStack.AddArrangedSubview(subtitleLabel);
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
            QueueRepeatButton.SetImage(image, UIControlState.Normal);
        }

        private void UpdateTrackReferenceButtonVisibility()
        {
            if (_trackReferenceButtonVisibilityController == null)
            {
                _trackReferenceButtonVisibilityController = new UIViewVisibilityController(TrackReferernceButton);
            }

            var valueConverter = new TrackHasExternalRelationsValueConverter();
            var referenceButtonVisible = valueConverter.Convert(ViewModel.CurrentTrack, null, null, null);
            if (referenceButtonVisible is bool visible)
            {
                _trackReferenceButtonVisibilityController.ViewIsVisible = visible;
            }
            else
            {
                _trackReferenceButtonVisibilityController.ViewIsVisible = false;
            }
        }

        private void AddNavigationBarItemForQueue()
        {
            var sidebarButton = new UIBarButtonItem(
                UIImage.FromFile("icon_queue_static.png"),
                UIBarButtonItemStyle.Plain,
                (object sender, EventArgs e) => { ViewModel.OpenQueueCommand.Execute(); }
            );

            NavigationItem.SetRightBarButtonItem(sidebarButton, true);
        }

        private void AddGradientToTrackCoverImage()
        {
            TrackCoverImageView.SetGradientBackground(AppColors.TrackCoverGradientColors, new NSNumber[] {0, .28f, .6f, 1});
        }

        public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
        {
            return UIInterfaceOrientation.Portrait;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.Portrait;
        }
    }
}
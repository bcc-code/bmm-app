using System;
using Airbnb.Lottie;
using BMM.Core.Constants;
using BMM.Core.Models.POs.ContinueListening;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class ContinueListeningCollectionViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(ContinueListeningCollectionViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(ContinueListeningCollectionViewCell), NSBundle.MainBundle);
        private DateTime? _date;
        private string _subtitle;
        private bool _isCurrentlyPlaying;

        public ContinueListeningCollectionViewCell(IntPtr handle): base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ContinueListeningCollectionViewCell, ContinueListeningTilePO>();
                
                set.Bind(BackgroundView)
                    .For(v => v.BackgroundColor)
                    .To(po => po.ContinueListeningTile.BackgroundColor)
                    .WithConversion<HexStringToUiColorConverter>(AppColors.TileDefaultColor);

                set.Bind(BackgroundView)
                    .For(v => v.BindTap())
                    .To(po => po.TileClickedCommand);
                
                CoverImageView.ErrorAndLoadingPlaceholderImagePathForCover();
                set.Bind(CoverImageView)
                    .For(v => v.ImagePath)
                    .To(vm => vm.ContinueListeningTile.CoverUrl)
                    .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.CoverPlaceholderImage);

                set.Bind(TitleLabel)
                    .To(vm => vm.ContinueListeningTile.Label);
                
                set.Bind(SubtitleLabel)
                    .To(vm => vm.ContinueListeningTile.Title);

                set.Bind(this)
                    .For(v => v.Date)
                    .To(vm => vm.ContinueListeningTile.Date);
                
                set.Bind(DateLabel)
                    .To(vm => vm.ContinueListeningTile.Date)
                    .WithConversion<DateTimeToPodcastPublishDateLabelValueConverter>();
                
                set.Bind(DayOfWeekLabel)
                    .To(vm => vm.ContinueListeningTile.Date)
                    .WithConversion<DateTimeToPodcastPublishDayOfWeekLabelValueConverter>();

                set.Bind(this)
                    .For(v => v.Subtitle)
                    .To(vm => vm.ContinueListeningTile.Subtitle);
                
                set.Bind(ProgressBarView)
                    .For(v => v.Percentage)
                    .To(vm => vm.ContinueListeningTile.Percentage);
                
                set.Bind(ReferenceButton)
                    .For(v => v.BindVisible())
                    .To(vm => vm.ContinueListeningTile.Track)
                    .WithConversion<TrackHasExternalRelationsValueConverter>();

                set.Bind(ReferenceButton)
                    .To(po => po.ShowTrackInfoCommand);
                
                set.Bind(DownloadedIcon)
                    .For(v => v.BindVisible())
                    .To(vm => vm.ContinueListeningTile.Track)
                    .WithConversion<DownloadStatusDoneValueConverter>();
                
                set.Bind(OptionsButton)
                    .To(po => po.OptionButtonClickedCommand);

                set.Bind(ShuffleButton)
                    .To(po => po.ShuffleButtonClickedCommand);
                
                set.Bind(ShuffleButton)
                    .For(v => v.BindVisible())
                    .To(vm => vm.ContinueListeningTile.ShufflePodcastId)
                    .WithConversion<HasValueToVisibleValueConverter>();

                set.Bind(IsPlayingButton)
                    .For(v => v.BindVisible())
                    .To(po => po.IsCurrentlySelected);

                set.Bind(TitleClickableArea)
                    .For(v => v.BindTap())
                    .To(po => po.ContinueListeningCommand);

                set.Bind(PlayButton)
                    .To(po => po.ContinueListeningCommand);

                set.Bind(this)
                    .For(v => v.IsCurrentlyPlaying)
                    .To(po => po.IsCurrentlyPlaying);
                
                set.Apply();

                var animation = LOTAnimationView.AnimationNamed(LottieAnimationsNames.PlayAnimationIcon);
                animation.BackgroundColor = AppColors.OnColorOneColor;
                animation.LoopAnimation = true;

                PlayButton!.AddAnimation(animation);
            });
        }

        public bool IsCurrentlyPlaying
        {
            get => _isCurrentlyPlaying;
            set
            {
                _isCurrentlyPlaying = value;
                
                if (_isCurrentlyPlaying)
                    PlayButton?.PlayAnimation();
                else
                    PlayButton?.StopAnimation();
            }
        }

        public string Subtitle
        {
            get => _subtitle;
            set
            {
                _subtitle = value;
                
                if (_subtitle != null)
                    UpdateProgressBarSizeIfNeeded();

                RemainingLabel.Text = _subtitle;
            }
        }

        private void UpdateProgressBarSizeIfNeeded()
        {
            var newString = new NSString(_subtitle);
            var attribs = new UIStringAttributes
            {
                Font = RemainingLabel.Font
            };

            var size = newString.GetSizeUsingAttributes(attribs);
            UpdateSizeOfProgressBar(size.Width >= ContentWidthHelper.Frame.Width * 0.5f);
        }

        private void UpdateSizeOfProgressBar(bool isSmall)
        {
            float multipleFactor = isSmall
                ? 0.25f
                : 0.5f;

            ProgressBarWidthConstraint.Constant = ContentWidthHelper.Frame.Width * multipleFactor;
        }

        public DateTime? Date
        {
            get => _date;
            set
            {
                _date = value;
                bool shouldShowProgressBar = _date == null;
                ProgressBarView.Hidden = !shouldShowProgressBar;
                RemainingLabel.Hidden = !shouldShowProgressBar;
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            ProgressBarWidthConstraint.Constant = ContentWidthHelper.Frame.Width / 2;
            ShuffleButton.ApplyButtonStyle(AppTheme.ButtonTertiaryMediumOnColorFive);
            SubtitleLabel.ApplyTextTheme(AppTheme.Paragraph1OnColor2);
            TitleLabel.ApplyTextTheme(AppTheme.Title1OnColor1);
            DayOfWeekLabel.ApplyTextTheme(AppTheme.Subtitle3OnColor1);
            RemainingLabel.ApplyTextTheme(AppTheme.Subtitle3OnColor1);
            DateLabel.ApplyTextTheme(AppTheme.Subtitle3OnColor2);
        }
    }
}
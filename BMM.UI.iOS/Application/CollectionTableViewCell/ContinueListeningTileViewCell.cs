using System;
using Airbnb.Lottie;
using BMM.Core.Constants;
using BMM.Core.Models.POs.Tiles;
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
    public partial class ContinueListeningTileViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(ContinueListeningTileViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(ContinueListeningTileViewCell), NSBundle.MainBundle);
        private DateTime? _date;
        private string _subtitle;
        private bool _isCurrentlyPlaying;

        public ContinueListeningTileViewCell(IntPtr handle): base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ContinueListeningTileViewCell, ContinueListeningTilePO>();
                
                set.Bind(BackgroundView)
                    .For(v => v.BackgroundColor)
                    .To(po => po.Tile.BackgroundColor)
                    .WithConversion<HexStringToUiColorConverter>(AppColors.TileDefaultColor);

                set.Bind(BackgroundView)
                    .For(v => v.BindTap())
                    .To(po => po.TileClickedCommand);
                
                CoverImageView.ErrorAndLoadingPlaceholderImagePathForCover();
                
                set.Bind(CoverImageView)
                    .For(v => v.ImagePath)
                    .To(vm => vm.Tile.CoverUrl)
                    .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.CoverPlaceholderImage);

                set.Bind(TitleLabel)
                    .To(vm => vm.Tile.Label);
                
                set.Bind(SubtitleLabel)
                    .To(vm => vm.Tile.Title);

                set.Bind(this)
                    .For(v => v.Date)
                    .To(vm => vm.Tile.Date);
                
                set.Bind(DateLabel)
                    .To(vm => vm.Tile.Date)
                    .WithConversion<DateTimeToPodcastPublishDateLabelValueConverter>();
                
                set.Bind(DayOfWeekLabel)
                    .To(vm => vm.Tile.Date)
                    .WithConversion<DateTimeToPodcastPublishDayOfWeekLabelValueConverter>();

                set.Bind(this)
                    .For(v => v.Subtitle)
                    .To(vm => vm.Tile.Subtitle);
                
                set.Bind(ProgressBarView)
                    .For(v => v.Percentage)
                    .To(vm => vm.Tile.Percentage);
                
                set.Bind(ReferenceButton)
                    .For(v => v.BindVisible())
                    .To(vm => vm.Tile.Track)
                    .WithConversion<TrackHasExternalRelationsValueConverter>();

                set.Bind(ReferenceButton)
                    .To(po => po.ShowTrackInfoCommand);

                set.Bind(DownloadedIcon)
                    .For(v => v.BindVisible())
                    .To(vm => vm.IsDownloaded);
                
                set.Bind(OptionsButton)
                    .To(po => po.OptionButtonClickedCommand);

                set.Bind(ShuffleButton)
                    .To(po => po.ShuffleButtonClickedCommand);
                
                set.Bind(ShuffleButton)
                    .For(v => v.BindVisible())
                    .To(vm => vm.Tile.ShufflePodcastId)
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
                animation.BackgroundColor = AppColors.GlobalBlackOneColor;
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
            ShuffleButton.ApplyButtonStyle(AppTheme.ButtonTertiaryMediumSeparatorColorFive);
            SubtitleLabel.ApplyTextTheme(AppTheme.Paragraph1GlobalBlack2);
            TitleLabel.ApplyTextTheme(AppTheme.Title1GlobalBlack1);
            DayOfWeekLabel.ApplyTextTheme(AppTheme.Subtitle3GlobalBlack1);
            RemainingLabel.ApplyTextTheme(AppTheme.Subtitle3GlobalBlack1);
            DateLabel.ApplyTextTheme(AppTheme.Subtitle3GlobalBlack2);
        }
    }
}